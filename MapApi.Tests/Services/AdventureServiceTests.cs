using System;
using MapApi.Services;
using Xunit;

namespace MapApi.Tests.Services
{
    public class AdventureServiceTests
    {
        #region Setup

        private readonly Guid _testLocationId = Guid.NewGuid();
        private readonly Guid _testRouteId = Guid.NewGuid();

        public AdventureServiceTests()
        {
            
        }

        private AdventureService CreateService()
        {
            var adventureService = new AdventureService(
                Mocks.MockAdventureServiceLink(_testLocationId, _testRouteId));
            return adventureService;
        }

        #endregion

        #region GetInitialLocationId

        [Fact]
        public async void GetInitialLocationId_ReturnsLocationId()
        {
            //arrange
            var service = CreateService();
            
            //act
            var locationId = await service.GetInitialLocationId(Guid.NewGuid(), Guid.NewGuid());
            
            //assert
            Assert.Equal(_testLocationId, locationId);
        }

        #endregion

        #region GetRouteIdsForLocation

        [Fact]
        public async void GetRouteIdsForLocation_ReturnsRouteIds()
        {
            //arrange
            var service = CreateService();
            
            //act
            var routes = await service.GetRouteIdsForLocation(
                Guid.NewGuid(),
                _testLocationId,
                Guid.NewGuid());
            
            //assert
            Assert.Equal(2, routes.Count);
            Assert.Equal(_testRouteId, routes[0]);
        }

        #endregion

        #region GetRoutesForLocation

        [Fact]
        public async void GetRoutesForLocation_ReturnRoutes()
        {
            //arrange
            var service = CreateService();
            
            //act
            var routes = await service.GetRoutesForLocation(
                Guid.NewGuid(),
                _testLocationId,
                Guid.NewGuid());
            
            //assert
            Assert.Equal(2, routes.Count);
            Assert.Equal(_testRouteId, routes[0].RouteId);
            Assert.Equal(_testLocationId, routes[0].LocationId);
            Assert.Equal("r1", routes[0].Name);
        }

        #endregion
    }
}