using System;
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
            UserExperienceService customerService = new UserExperienceService();

            Console.Clear();
            InitialStateChoice currentChoice = InitialStateChoice.Login;

            while (currentChoice != InitialStateChoice.Exit)
            {
                AnsiConsole.Write(new FigletText("Your Eyes").Centered().Color(Color.Blue));
                AnsiConsole.Write(new FigletText("---------------").Centered().Color(Color.Blue));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(10)
                        .AddChoices(ChoiceDescriptions.Select(kv => kv.Value))
                );

                currentChoice = ChoiceDescriptions.FirstOrDefault(kv => kv.Value == choice).Key;

                switch (currentChoice)
                {
                    case InitialStateChoice.ListMovies:
                        customerService.ListMoviesWithShowtimes(db);
                        break;
                    case InitialStateChoice.Login:
                        var loginChoice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Selecteer gebruikerstype:")
                                .PageSize(10)
                                .AddChoices(new[] { "Admin", "Gebruiker" , "Account aanmaken"})
                        );
                        switch (loginChoice)
                        {
                            case "Admin":
                                PresentAdminLogin.Start(db);                                
                                break;
                            case "Gebruiker":
                                PresentCustomerLogin.Start(db);
                                break;
                            case "Account aanmaken":
                                PresentCustomerRegistration.Start(db);
                                break;
                        }
                        break;
                }
            }
        }
    }
}
