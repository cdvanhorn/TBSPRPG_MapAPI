using System.Threading.Tasks;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using MapApi.Adapters;
using TbspRpgLib.InterServiceCommunication;

namespace MapApi.EventProcessors {
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler {
        protected readonly IGameAggregateAdapter _gameAdapter;
        protected readonly IEventAdapter _eventAdapter;
        protected readonly IAdventureServiceLink _adventureServiceLink;

        public EventHandler(IAdventureServiceLink adventureServiceLink) {
            _gameAdapter = new GameAggregateAdapter();
            _eventAdapter = new EventAdapter();
            _adventureServiceLink = adventureServiceLink;
        }
    }
}