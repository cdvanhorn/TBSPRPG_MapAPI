using System.Threading.Tasks;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using MapApi.Adapters;

namespace MapApi.EventProcessors {
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler {
        protected IGameAggregateAdapter _gameAdapter;
        protected IEventAdapter _eventAdapter;

        public EventHandler() {
            _gameAdapter = new GameAggregateAdapter();
            _eventAdapter = new EventAdapter();
        }
    }
}