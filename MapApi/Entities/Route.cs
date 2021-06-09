using System;

namespace MapApi.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public string Name { get; set; }
        public Guid LocationId { get; set; }
        
        public Location Location { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var route = obj as Route;
            if (route == null) return false;
            return this.RouteId == route.RouteId;
        }
    }
}