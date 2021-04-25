using MapApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Tests
{
    public class InMemoryTest
    {
        protected readonly DbContextOptions<MapContext> _dbContextOptions;

        protected InMemoryTest(string dbName)
        {
            _dbContextOptions = new DbContextOptionsBuilder<MapContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options;
        }
    }
}