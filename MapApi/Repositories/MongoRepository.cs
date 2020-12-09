using MongoDB.Driver;

using TbspRpgLib.Settings;

namespace MapApi.Repositories {
    public class MongoRepository {
        protected IMongoDatabase _mongoDatabase;

        public MongoRepository(IDatabaseSettings dbSettings) {
            var connectionString = $"mongodb+srv://{dbSettings.Username}:{dbSettings.Password}@{dbSettings.Url}/{dbSettings.Name}?retryWrites=true&w=majority";
            var client = new MongoClient(connectionString);
            _mongoDatabase = client.GetDatabase(dbSettings.Name);
        }
    }
}