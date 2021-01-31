using System;
using System.Threading.Tasks;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Events;
using TbspRpgLib.Entities;

using MapApi.Repositories;
using MapApi.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MapApi.EventProcessors
{
    public class EventProcessor : MultiEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(
            IEventStoreSettings eventStoreSettings,
            IServiceScopeFactory scopeFactory) :
                base(
                    "map",
                    new string[] {
                        Event.NEW_GAME_EVENT_TYPE,
                        Event.ENTER_LOCATION_CHECK_EVENT_TYPE
                    },
                    eventStoreSettings
                )
        {
            _scopeFactory = scopeFactory;
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MapContext>();
            InitializeStartPosition(context);
        }

        protected override async Task HandleEvent(Aggregate aggregate, Event evnt) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            EventType eventType = GetEventTypeByName(evnt.Type);

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<MapContext>();
                var service = scope.ServiceProvider.GetRequiredService<IMapService>();
                
                var transaction = context.Database.BeginTransaction();
                try {
                    //check if we've already processed the event
                    if(await service.HasBeenProcessed(evnt.EventId))
                        return;

                    //figure out what handler to call based on event type
                    IEventHandler handler = null;
                    if(eventType.TypeName == Event.NEW_GAME_EVENT_TYPE)
                        handler = scope.ServiceProvider.GetRequiredService<INewGameEventHandler>();
                    else if(eventType.TypeName == Event.ENTER_LOCATION_CHECK_EVENT_TYPE)
                        handler = scope.ServiceProvider.GetRequiredService<IEnterLocationCheckEventHandler>();
                    if(handler != null)
                        await handler.HandleEvent(gameAggregate, evnt);

                    //update the event type position and this event is processed
                    await service.UpdatePosition(eventType.Id, gameAggregate.GlobalPosition);
                    await service.EventProcessed(evnt.EventId);
                    //save the changes
                    context.SaveChanges();
                    transaction.Commit();
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    transaction.Rollback();
                    throw new Exception("event processor error");
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