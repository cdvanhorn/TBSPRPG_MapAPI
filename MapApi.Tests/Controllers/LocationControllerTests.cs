using MapApi.Controllers;
using Xunit;

namespace MapApi.Tests.Controllers
{
    public class LocationControllerTests : InMemoryTest
    {
        #region Setup

        public LocationControllerTests() : base("LocationControllerTests")
        {
            Seed();
        }

        public void Seed()
        {
            
        }

        public static LocationsController CreateController()
        {
            return null;
        }

        #endregion

        #region GetAll

        [Fact]
        public async void GetAll_ReturnAll()
        {
            
        }

        #endregion

        #region GetByGameId

        [Fact]
        public async void GetByGameId_Valid_ReturnOne()
        {
            
        }

        [Fact]
        public async void GetByGameId_Invalid_ReturnNone()
        {
            
        }

        #endregion
    }
}