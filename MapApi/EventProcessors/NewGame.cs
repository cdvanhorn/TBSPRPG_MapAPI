using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Services;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.Events;

using MapApi.Adapters;
using MapApi.Entities;
using MapApi.Services;
using MapApi.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace MapApi.EventProcessors
{
    public interface INewGameHandler {
        Task HandleNewGameEvent(GameAggregate gameAggregate);
    }

    public class NewGameHandler : INewGameHandler{
        private IGameService _gameService;
        private IServiceService _serviceService;
        private IAdventureServiceCom _adventureService;
        private IEventAdapter _eventAdapter;
        private IEventService _eventService;
        private IGameAggregateAdapter _gameAdapter;

        public NewGameHandler(IGameService gameService,
                IServiceService serviceService,
                IAdventureServiceCom adventureService,
                IEventService eventService) {
            _gameService = gameService;
            _serviceService = serviceService;
            _adventureService = adventureService;
            _eventService = eventService;
            _eventAdapter = new EventAdapter();
            _gameAdapter = new GameAggregateAdapter();
        }

        public async Task HandleNewGameEvent(GameAggregate gameAggregate) {
            //this will be our business logic, so we can do some testing
            Game game = _gameAdapter.ToEntity(gameAggregate);

            //get the initial location
            var responseTask = _adventureService.GetInitialLocation(
                game.AdventureId.ToString(),
                game.UserId.ToString()
            );

            // //if the game is missing fields or some fields are the same ignore it
            await _gameService.AddGame(game);

            var response = await responseTask;
            //game to get the location id from the response
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Response.Content);

            //create an enter_location event that contains this service id plus the new_game event id
            Event enterLocationEvent = _eventAdapter.NewEnterLocationEvent(new Location() {
                Id = new Guid(responseDict["id"]),
                GameId = game.Id
            });

            Console.WriteLine("Stream Position " + gameAggregate.StreamPosition);
            //if the aggregate already have a destination equal to the location we're setting,
            //don't send the event
            if(gameAggregate.Destination != null && gameAggregate.Destination == responseDict["id"])
                return;

            //send the event
            Console.WriteLine("Stream Position " + gameAggregate.StreamPosition);
            await _eventService.SendEvent(enterLocationEvent, gameAggregate.StreamPosition);
        }
    }
    public class NewGame : NewGameEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NewGame(IEventStoreSettings eventStoreSettings, IServiceScopeFactory scopeFactory) :
            base("map", eventStoreSettings){
            //create an adapter to convert game aggregate to a game object
            _scopeFactory = scopeFactory;
            IServiceScope scope  = _scopeFactory.CreateScope();
            MapContext context = scope.ServiceProvider.GetRequiredService<MapContext>();
            InitializeStartPosition(context);
        }

        protected override async Task HandleEvent(Aggregate aggregate, string eventId) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            
            Guid eventguid;
            if(!Guid.TryParse(eventId, out eventguid))
                return;

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<MapContext>();
                var mapService = scope.ServiceProvider.GetRequiredService<IMapService>();
                if(await mapService.HasBeenProcessed(eventguid))
                    return;

                using var transaction = context.Database.BeginTransaction();
                try {
                    //create a handler
                    var handler = scope.ServiceProvider.GetRequiredService<INewGameHandler>();

                    // //update the event type position
                    await mapService.UpdatePosition(_eventType.Id, gameAggregate.GlobalPosition);
                    // //update the processed events
                    await mapService.EventProcessed(eventguid);

                    //call the handler
                    await handler.HandleNewGameEvent(gameAggregate);

                    //save the changes
                    context.SaveChanges();
                    transaction.Commit();
                } catch(Exception e) {
                    //we need to do something with the exception
                    //there may be the potential for a rogue event to be out there
                    //may need to resubscribe so we attempt to process the event again
                    transaction.Rollback();
                    throw e;
                }
            }
        }
    }
}

//I don't think I can do this.
//bad things that could happen
//1. another map api already updated the database and sent out the enter location event
    // event insertion would throw an exception, and everything would be fine because
    //  the other service properly updated everything
    // still want to resubscribe, the event will end up being skipped
//2. database insertion fails but event creation succeeds (probably unlikely)
    //  we would not get the event again from the subscription, unless we resubscribe
    //  other services would begin reacting to the enter location event
    //  processing the entered location event would fail because we would not know about the game
    //  the player would be left in limbo, unable to enter the location

    // in this case we should try and resubscribe, and before we send an event make sure
    // destination in the aggregate is not set to the location we're trying to set it to
    // then the event would not be sent but the database would be updated

//3. What happens if another unrelated event sneaks in before we add our enter_location event?
    // Our event insertion will fail, and we would never process this new game event unless
    // we subscribe again and try to andle the event again

//Need to test if any exceptions thrown in the event handler triggers subscriptionDropped
//function to be called, it does
