using System.Threading.Tasks;
using MapApi.Adapters;
using MapApi.Services;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

namespace MapApi.EventProcessors {
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler {
        protected readonly IGameAggregateAdapter _gameAdapter;
        protected readonly IEventAdapter _eventAdapter;
        protected readonly IAdventureService _adventureService;

        public EventHandler(IAdventureService adventureService) {
            _gameAdapter = new GameAggregateAdapter();
            _eventAdapter = new EventAdapter();
            _adventureService = adventureService;
        }
    }
}