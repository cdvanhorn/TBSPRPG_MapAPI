using TbspRpgLib.Repositories;

namespace MapApi.Repositories {
    public interface IMapRepository: IServiceTrackingRepository {

    }

    public class MapRepository : ServiceTrackingRepository, IMapRepository {
        public MapRepository(MapContext context) : base(context) {}
    }
}