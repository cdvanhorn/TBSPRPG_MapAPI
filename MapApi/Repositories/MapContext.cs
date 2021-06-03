using Microsoft.EntityFrameworkCore;
using MapApi.Entities;
using TbspRpgLib.Repositories;

namespace MapApi.Repositories {
    public class MapContext : ServiceTrackingContext {
        public MapContext(DbContextOptions<MapContext> options) : base(options){}

        public DbSet<Game> Games { get; set; }

        public DbSet<Location> Locations { get; set; }
        
        public DbSet<Route> Routes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<Game>().ToTable("games");
            modelBuilder.Entity<Location>().ToTable("locations");
            modelBuilder.Entity<Route>().ToTable("routes");

            modelBuilder.Entity<Game>().HasKey(x => x.Id);
            modelBuilder.Entity<Game>().Property(x => x.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Location>().HasKey(x => x.Id);
            modelBuilder.Entity<Location>().Property(x => x.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();
            
            modelBuilder.Entity<Route>().HasKey(x => x.Id);
            modelBuilder.Entity<Route>().Property(x => x.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Location)
                .WithOne(l => l.Game)
                .HasForeignKey<Location>(l => l.GameId);

            modelBuilder.Entity<Location>()
                .HasMany(l => l.Routes)
                .WithOne(r => r.Location)
                .HasForeignKey(r => r.LocationId);
        }
    }
}