using System;

namespace MapApi.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid LocationId { get; set; }
        
        public Location Location { get; set; }
    }
}