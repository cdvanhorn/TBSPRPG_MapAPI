using System;
using System.Collections.Generic;
using Moq;
using RestSharp;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;

namespace MapApi.Tests
{
    public static class Mocks
    {
        public static IAdventureServiceLink MockAdventureServiceLink(Guid testLocationId, Guid testRouteId)
        {
            var adventureServiceLink = new Mock<IAdventureServiceLink>();
            
            //get initial location
            adventureServiceLink.Setup(asl =>
                asl.GetInitialLocation(It.IsAny<AdventureRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((AdventureRequest adventureRequest, Credentials creds) => new IscResponse()
            {
                Response = new RestResponse()
                {
                    Content = "{\"id\": \"" + testLocationId + "\"}"
                }
            });
            
            //get routes
            adventureServiceLink.Setup(asl =>
                asl.GetRoutesForLocation(It.IsAny<AdventureRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((AdventureRequest adventureRequest, Credentials creds) => new IscResponse()
            {
                Response = new RestResponse()
                {
                    Content = "[{\"id\": \"" + testRouteId + "\"}, {\"id\": \"" + Guid.NewGuid() + "\"}]"
                }
            });
            return adventureServiceLink.Object;
        }

        public static IAggregateService MockAggregateService(ICollection<Event> events)
        {
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service =>
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<ulong>())
            ).Callback<string, Event, ulong>((type, evnt, n) =>
            {
                if (n <= 100)
                    events.Add(evnt);
            });
            return aggregateService.Object;
        }
    }
}