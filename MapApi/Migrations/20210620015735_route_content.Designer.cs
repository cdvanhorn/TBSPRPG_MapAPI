﻿// <auto-generated />
using System;
using MapApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MapApi.Migrations
{
    [DbContext(typeof(MapContext))]
    [Migration("20210620015735_route_content")]
    partial class route_content
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("MapApi.Entities.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AdventureId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("games");
                });

            modelBuilder.Entity("MapApi.Entities.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("GameId")
                        .IsUnique();

                    b.ToTable("locations");
                });

            modelBuilder.Entity("MapApi.Entities.Route", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("RouteId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("routes");
                });

            modelBuilder.Entity("TbspRpgLib.Entities.EventTypePosition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("EventTypeId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Position")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.ToTable("event_type_positions");
                });

            modelBuilder.Entity("TbspRpgLib.Entities.ProcessedEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("processed_events");
                });

            modelBuilder.Entity("MapApi.Entities.Location", b =>
                {
                    b.HasOne("MapApi.Entities.Game", "Game")
                        .WithOne("Location")
                        .HasForeignKey("MapApi.Entities.Location", "GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("MapApi.Entities.Route", b =>
                {
                    b.HasOne("MapApi.Entities.Location", "Location")
                        .WithMany("Routes")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("MapApi.Entities.Game", b =>
                {
                    b.Navigation("Location");
                });

            modelBuilder.Entity("MapApi.Entities.Location", b =>
                {
                    b.Navigation("Routes");
                });
#pragma warning restore 612, 618
        }
    }
}
