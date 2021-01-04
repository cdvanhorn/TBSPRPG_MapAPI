using Microsoft.EntityFrameworkCore;
using MapApi.Entities;

namespace MapApi.Repositories {
    public class MapContext : DbContext {
        public MapContext(DbContextOptions<MapContext> options) : base(options){}

        public DbSet<Game> Games { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().ToTable("MapService.Game");
            modelBuilder.Entity<Location>().ToTable("MapService.Location");

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Location)
                .WithOne(l => l.Game)
                .HasForeignKey<Location>(l => l.GameId);
        }
    }
}