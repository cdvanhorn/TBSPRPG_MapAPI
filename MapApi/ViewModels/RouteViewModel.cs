using System;
using MapApi.Entities;

namespace MapApi.ViewModels
{
    public class RouteViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public RouteViewModel(Route route)
        {
            Id = route.RouteId;
            Name = route.Name;
            Content = route.Content;
        }
    }
}