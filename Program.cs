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
            ManagementExperienceService service = new ManagementExperienceService();
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
                                .AddChoices(new[] { "Admin", "Gebruiker" , "Registreren"})
                        );
                        switch (loginChoice)
                        {
                            case "Admin":
                                PresentAdminLogin.Start(db);                                
                                break;
                            case "Gebruiker":
                                string customerName = AnsiConsole.Prompt(new TextPrompt<string>("Gebruikersnaam"));
                                string customerPassword = AnsiConsole.Prompt(new TextPrompt<string>("Wachtwoord").Secret());

                                Customer customer = db.Customers
                                    .FirstOrDefault(c =>
                                        c.Username == customerName && c.Password == customerPassword
                                    );

                                if (customer == null)
                                {
                                    Console.Clear();
                                    AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord[/]");
                                    break;
                                }

                                AnsiConsole.MarkupLine("[green]Succesvol ingelogd als beheerder[/]");
                                customerService.ManageUser(customer, db, configuration);

                                // Add logic for user actions
                                break;
                            // case "Registreren":
                            //     PresentRegistration.Start();
                        }
                        break;
                }
            }
        }
    }
}
