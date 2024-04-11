using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Cinema.Migrations
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

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("Cinema.Data.Hall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Columns")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Rows")
                        .HasColumnType("integer");

                    b.Property<List<Seat>>("Seats")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("Halls");
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

                    b.Property<string>("Duration")
                        .HasColumnType("text");

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

                    b.Property<List<Seat>>("Prices")
                        .HasColumnType("jsonb");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CancelledAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("text");

                    b.Property<string>("CustomerName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastChangedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("PurchaseTotal")
                        .HasColumnType("numeric");

                    b.Property<DateTimeOffset>("PurchasedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SeatNumber")
                        .HasColumnType("integer");

                    b.Property<int>("SeatRow")
                        .HasColumnType("integer");

                    b.Property<int?>("ShowtimeId")
                        .HasColumnType("integer");

                    b.Property<string>("TicketNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ShowtimeId");

                    b.ToTable("Tickets");
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
                    b.HasOne("Cinema.Data.Showtime", "Showtime")
                        .WithMany()
                        .HasForeignKey("ShowtimeId");

                    b.Navigation("Showtime");
                });

            modelBuilder.Entity("Cinema.Data.Movie", b =>
                {
                    b.Navigation("Showtimes");
                });
#pragma warning restore 612, 618
        }
    }
}
