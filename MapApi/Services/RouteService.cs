using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapApi.Entities;
using MapApi.Repositories;

namespace MapApi.Services
{
    public interface IRouteService
    {
        Task<List<Route>> GetRoutesForGame(Guid gameId);
    }
    
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repository;

        public RouteService(IRouteRepository repository)
        {
            _repository = repository;
        }
        
        public Task<List<Route>> GetRoutesForGame(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}