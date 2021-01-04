

namespace MapApi.Entities {
    public class Game {
        public int Id { get; set; }
        public int AdventureId { get; set; }
        public int UserId { get; set; }

        public Location Location { get; set; }
    }
}