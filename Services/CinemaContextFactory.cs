﻿using Cinema.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cinema.Services
{
    public class CinemaContextFactory : IDesignTimeDbContextFactory<CinemaContext>
    {
        public CinemaContext CreateDbContext(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("Main");
            return new CinemaContext(connectionString);
        }
    }
}
