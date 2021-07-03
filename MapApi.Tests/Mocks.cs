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
        public static IAdventureServiceLink MockAdventureServiceLink(Guid testLocationId, Guid testRouteId, List<Guid> sourceIds, Guid errorSourceId)
        {
            var adventureServiceLink = new Mock<IAdventureServiceLink>();
            
            //get initial location
            adventureServiceLink.Setup(asl =>
                asl.GetInitialLocation(It.IsAny<AdventureRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((AdventureRequest adventureRequest, Credentials creds) => new IscResponse()
            {
                Content = "{\"id\": \"" + testLocationId + "\"}"
            });
            
            //get routes
            adventureServiceLink.Setup(asl =>
                asl.GetRoutesForLocation(It.IsAny<AdventureRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((AdventureRequest adventureRequest, Credentials creds) =>
            {
                if (adventureRequest.LocationId == errorSourceId)
                {
                    return new IscResponse()
                    {
                        Content = "[{\"id\": \"" + testRouteId + "\"" +
                                  ", \"locationId\": \"" + adventureRequest.LocationId + "\"" +
                                  ", \"name\": \"r1\"" +
                                  ", \"sourceId\": \"" + errorSourceId + "\"}]"
                    };
                }
                return new IscResponse()
                {
                    Content = "[{\"id\": \"" + testRouteId + "\"" +
                              ", \"locationId\": \"" + testLocationId + "\"" +
                              ", \"name\": \"r1\"" +
                              ", \"sourceId\": \"" + sourceIds[0] + "\"}" +
                              ", {\"id\": \"" + Guid.NewGuid() + "\"" +
                              ", \"locationId\": \"" + testLocationId + "\"" +
                              ", \"name\": \"r2\"" +
                              ", \"sourceId\": \"" + sourceIds[1] + "\"}]"
                };
            });
            return adventureServiceLink.Object;
        }

        public static IContentServiceLink MockContentServiceLink(List<Guid> sourceIds, Guid errorSourceId)
        {
            var contentServiceLink = new Mock<IContentServiceLink>();
            contentServiceLink.Setup(csl =>
                csl.GetSourceContent(It.IsAny<ContentRequest>(), It.IsAny<Credentials>())
            ).ReturnsAsync((ContentRequest contentRequest, Credentials creds) =>
            {
                if (contentRequest.SourceKey == sourceIds[0])
                {
                    return new IscResponse()
                    {
                        IsSuccessful = true,
                        Content = "{" +
                                  "\"Id\": \"" + contentRequest.SourceKey + "\"" +
                                  ", \"Language\": \"en\"" +
                                  ", \"Source\": \"source content 0\"" +
                                  "}"
                    };
                } 
                if (contentRequest.SourceKey == sourceIds[1])
                {
                    return new IscResponse()
                    {
                        IsSuccessful = true,
                        Content = "{" +
                                  "\"Id\": \"" + contentRequest.SourceKey + "\"" +
                                  ", \"Language\": \"en\"" +
                                  ", \"Source\": \"source content 1\"" +
                                  "}"
                    };
                }
                if (contentRequest.SourceKey == errorSourceId)
                {
                    return new IscResponse()
                    {
                        IsSuccessful = false,
                        ErrorMessage = "content api error"
                    };
                }
                return new IscResponse()
                {
                    Content = "{" +
                              "\"Id\": \"" + contentRequest.SourceKey + "\"" +
                              ", \"Language\": \"en\"" +
                              ", \"Source\": \"invalid source key " + contentRequest.SourceKey + "\"" +
                              "}"
                };
            });
            return contentServiceLink.Object;
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