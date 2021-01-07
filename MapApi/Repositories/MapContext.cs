using Microsoft.EntityFrameworkCore;
using MapApi.Entities;

namespace MapApi.Repositories {
    public class MapContext : DbContext {
        public MapContext(DbContextOptions<MapContext> options) : base(options){}

        public DbSet<Game> Games { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<Game>().ToTable("game");
            modelBuilder.Entity<Location>().ToTable("location");

            modelBuilder.Entity<Game>().HasKey(g => g.Id);
            modelBuilder.Entity<Game>().Property(g => g.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Location>().HasKey(a => a.Id);
            modelBuilder.Entity<Location>().Property(a => a.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Location)
                .WithOne(l => l.Game)
                .HasForeignKey<Location>(l => l.GameId);
        }
    }
}