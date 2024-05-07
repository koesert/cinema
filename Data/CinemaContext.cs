using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data
{
    public class CinemaContext : DbContext
    {
        private readonly string _connectionString;
        public CinemaContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<CinemaSeat> CinemaSeats { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CinemaSeat>()
                .Ignore(c => c.IsSelected);
        }
    }
}
