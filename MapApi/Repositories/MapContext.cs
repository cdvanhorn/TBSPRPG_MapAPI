using Microsoft.EntityFrameworkCore;
using MapApi.Entities;
using TbspRpgLib.Repositories;

namespace MapApi.Repositories {
    public class MapContext : ServiceTrackingContext {
        public MapContext(DbContextOptions<MapContext> options) : base(options){}

        public DbSet<Game> Games { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<Game>().ToTable("games");
            modelBuilder.Entity<Location>().ToTable("locations");

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