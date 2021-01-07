using System;

namespace MapApi.Entities {
    public class Game {
        public Guid Id { get; set; }
        public Guid AdventureId { get; set; }
        public Guid UserId { get; set; }

        public Location Location { get; set; }
    }
}