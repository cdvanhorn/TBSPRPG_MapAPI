// using System;
// using System.Threading.Tasks;

// using TbspRpgLib.Aggregates;
// using TbspRpgLib.Events;
// using TbspRpgLib.Events.Content;

// namespace GameSystemApi.EventProcessors {
//     public interface IEnterLocationEventHandler : IEventHandler {

//     }

//     public class EnterLocationEventHandler : EventHandler, IEnterLocationEventHandler {

//         private IEventService _eventService;

//         public EnterLocationEventHandler(IEventService eventService) : base() {
//             _eventService = eventService;
//         }

//         public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
//             //Game game = _gameAdapter.ToEntity(gameAggregate);
            
//             //get what checks need to be done from the adventure api
//             Console.WriteLine("handling new location event");

//             //create an enter_location_check event, default to success
//             Event enterLocationCheckEvent = new EnterLocationCheckEvent(
//                 new EnterLocationCheck() {
//                     Id = gameAggregate.Id,
//                     Result = true
//                 }
//             );

//             //oncomplete send enter_location_check event
//             await _eventService.SendEvent(enterLocationCheckEvent, gameAggregate.StreamPosition);
//         }
//     }
// }