﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace cinema.Migrations
{
    [DbContext(typeof(CinemaContext))]
    partial class CinemaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Cinema.Data.Administrator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("PriceEndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("PriceStartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("TempPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("Cinema.Data.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Cinema.Data.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<List<string>>("Cast")
                        .HasColumnType("jsonb");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<List<string>>("Directors")
                        .HasColumnType("jsonb");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<List<string>>("Genres")
                        .HasColumnType("jsonb");

                    b.Property<int>("MinAgeRating")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("ReleaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("Cinema.Data.Showtime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("MovieId")
                        .HasColumnType("integer");

                    b.Property<double>("Prices")
                        .HasColumnType("double precision");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("Showtimes");
                });

            modelBuilder.Entity("Cinema.Data.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset?>("CancelledAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("text");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<decimal>("PurchaseTotal")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("PurchasedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ShowtimeId")
                        .HasColumnType("integer");

                    b.Property<string>("TicketNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ShowtimeId");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("CinemaSeat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<bool>("IsReserved")
                        .HasColumnType("boolean");

                    b.Property<string>("Layout")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<char>("Row")
                        .HasColumnType("character(1)");

                    b.Property<int>("SeatNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("ShowtimeId")
                        .HasColumnType("integer");

                    b.Property<int?>("TicketId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShowtimeId");

                    b.HasIndex("TicketId");

                    b.ToTable("CinemaSeats");
                });

            modelBuilder.Entity("Voucher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("text");

                    b.Property<double>("Discount")
                        .HasColumnType("double precision");

                    b.Property<string>("DiscountType")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IsReward")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Vouchers");
                });

            modelBuilder.Entity("Cinema.Data.Showtime", b =>
                {
                    b.HasOne("Cinema.Data.Movie", "Movie")
                        .WithMany("Showtimes")
                        .HasForeignKey("MovieId");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("Cinema.Data.Ticket", b =>
                {
                    b.HasOne("Cinema.Data.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.HasOne("Cinema.Data.Showtime", "Showtime")
                        .WithMany()
                        .HasForeignKey("ShowtimeId");

                    b.Navigation("Customer");

                    b.Navigation("Showtime");
                });

            modelBuilder.Entity("CinemaSeat", b =>
                {
                    b.HasOne("Cinema.Data.Showtime", "Showtime")
                        .WithMany("CinemaSeats")
                        .HasForeignKey("ShowtimeId");

                    b.HasOne("Cinema.Data.Ticket", "Ticket")
                        .WithMany("Seats")
                        .HasForeignKey("TicketId");

                    b.Navigation("Showtime");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Cinema.Data.Movie", b =>
                {
                    b.Navigation("Showtimes");
                });

            modelBuilder.Entity("Cinema.Data.Showtime", b =>
                {
                    b.Navigation("CinemaSeats");
                });

            modelBuilder.Entity("Cinema.Data.Ticket", b =>
                {
                    b.Navigation("Seats");
                });
#pragma warning restore 612, 618
        }
    }
}
