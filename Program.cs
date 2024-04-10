﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace Cinema
{
    public class Program
    {
        private static readonly Dictionary<InitialStateChoice, string> ChoiceDescriptions = new Dictionary<InitialStateChoice, string>
        {
            { InitialStateChoice.ListMovies, "Blader door films & vertoningen" },
            { InitialStateChoice.Login, "Inloggen" },
            { InitialStateChoice.Exit, "Afsluiten" }
        };
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("Main");

            CinemaContext db = new CinemaContext(connectionString);

            ManagementExperienceService service = new ManagementExperienceService();

            Console.Clear();
            InitialStateChoice currentChoice = InitialStateChoice.Login;

            while (currentChoice != InitialStateChoice.Exit)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wat wil je doen?")
                        .PageSize(10)
                        .AddChoices(ChoiceDescriptions.Select(kv => kv.Value))
                );

                currentChoice = ChoiceDescriptions.FirstOrDefault(kv => kv.Value == choice).Key;

                switch (currentChoice)
                {
                    case InitialStateChoice.ListMovies:
                        service.ListMoviesWithShowtimes(db);
                        break;
                    case InitialStateChoice.Login:
                        string username = AnsiConsole.Prompt(new TextPrompt<string>("Gebruikersnaam"));
                        string password = AnsiConsole.Prompt(new TextPrompt<string>("Wachtwoord").Secret());

                        Administrator admin = db.Administrators
                            .FirstOrDefault(admin =>
                                admin.Username == username && admin.Password == password
                            );

                        if (admin == null)
                        {
                            AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord[/]");
                            break;
                        }
                        service.ManageCinema(admin, db, configuration);
                        break;
                }
            }
        }
    }
}
