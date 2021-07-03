using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapApi.Entities;
using MapApi.Services;
using Xunit;

namespace MapApi.Tests.Services
{
    public class AdventureServiceTests
    {
        #region Setup

        private readonly Guid _testLocationId = Guid.NewGuid();
        private readonly Guid _testRouteId = Guid.NewGuid();
        private readonly Guid _errorRouteId = Guid.NewGuid();

        private readonly List<Guid> _sourceKeys = new List<Guid>()
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        public AdventureServiceTests()
        {
            
        }

        private AdventureService CreateService()
        {
            var adventureService = new AdventureService(
                Mocks.MockAdventureServiceLink(_testLocationId, _testRouteId, _sourceKeys, _errorRouteId),
                Mocks.MockContentServiceLink(_sourceKeys, _errorRouteId));
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
                new Game()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                },
                _testLocationId);
            
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
                new Game()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                },
                _testLocationId);
            
            //assert
            Assert.Equal(2, routes.Count);
            Assert.IsType<Route>(routes[0]);
            Assert.Equal(_testRouteId, routes[0].RouteId);
            Assert.Equal(_testLocationId, routes[0].LocationId);
            Assert.Equal("r1", routes[0].Name);
            Assert.Equal("source content 0", routes[0].Content);
        }

        [Fact]
        public async void GetRoutesForLocation_ContentServiceFails_ThrowException()
        {
            //arrange
            var service = CreateService();
            
            //act
            Task Act() =>  service.GetRoutesForLocation(
                new Game()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                },
                _errorRouteId
            );

            var exception = Assert.ThrowsAsync<Exception>(Act);
        }

        #endregion
    }
}