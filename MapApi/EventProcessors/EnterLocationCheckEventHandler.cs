using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

using System.Threading.Tasks;

namespace MapApi.EventProcessors {
    public interface IEnterLocationCheckEventHandler : IEventHandler {}

    public class EnterLocationCheckEventHandler : IEnterLocationCheckEventHandler {
        public EnterLocationCheckEventHandler() : base() {

        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            //check the result
            if(gameAggregate.Checks.Location) {
                //we sucessfully entered the location
                gameAggregate.CurrentLocation = gameAggregate.Destination;
            }
            gameAggregate.Destination = "";

            //send an entered_location event or entered_location_failed event
        }
    }
}