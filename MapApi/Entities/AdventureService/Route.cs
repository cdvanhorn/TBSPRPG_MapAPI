using System;

namespace MapApi.Entities.AdventureService
{
    public class Route
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string Name { get; set; }
    }
}