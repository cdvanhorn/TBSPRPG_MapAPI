using TbspRpgLib.Services;
using MapApi.Repositories;

namespace MapApi.Services {
    public interface IMapService : IServiceTrackingService {

    }

    public class MapService : ServiceTrackingService, IMapService {
        public MapService(IMapRepository repository) : base(repository) {}
    }
}