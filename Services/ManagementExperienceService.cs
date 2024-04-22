using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Models.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sharprompt;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Services
{
    public class ManagementExperienceService
    {
        // Mapping of enum values to their display names

        private static readonly Dictionary<CinemaManagementMovieChoice, string> Descriptions = new Dictionary<CinemaManagementMovieChoice, string>
        {
            { CinemaManagementMovieChoice.ListShowtimes, "Lijst met aankomende vertoningen voor deze film" },
            { CinemaManagementMovieChoice.DeleteMovie, "Verwijder deze film" },
            { CinemaManagementMovieChoice.AddShowtime, "Voeg een vertoningstijd toe voor deze film" },
            { CinemaManagementMovieChoice.Exit, "Uitloggen" }
        };

        private static readonly Dictionary<CinemaManagementAddMovieChoice, string> AddMovieChoiceDescriptions = new Dictionary<CinemaManagementAddMovieChoice, string>
    {
        { CinemaManagementAddMovieChoice.AddMovieManually, "Voeg film handmatig toe" },
        { CinemaManagementAddMovieChoice.AddMovieJSON, "Voeg film(s) toe door JSON-bestand te laden" },
        { CinemaManagementAddMovieChoice.Exit, "Terug" }
    };
        private static readonly Dictionary<CinemaManagementChoice, string> ManagementChoiceDescriptions = new Dictionary<CinemaManagementChoice, string>
    {
        { CinemaManagementChoice.ListMovies, "Lijst met momenteel beschikbare films" },
        { CinemaManagementChoice.AddMovie, "Voeg een film toe" },
        { CinemaManagementChoice.Exit, "Terug" }
    };

        public void ManageCinema(Administrator admin, CinemaContext db, IConfiguration configuration)
        {
            CinemaManagementChoice currentManagerChoice = CinemaManagementChoice.ListMovies;
            while (currentManagerChoice != CinemaManagementChoice.Exit)
            {
                Console.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Welkom, {admin.Username}, wat wil je doen?")
                        .PageSize(10)
                        .AddChoices(ManagementChoiceDescriptions.Select(kv => kv.Value))
                );

                // Retrieve the enum value based on the selected description
                currentManagerChoice = ManagementChoiceDescriptions.FirstOrDefault(kv => kv.Value == choice).Key;

                switch (currentManagerChoice)
                {
                    case CinemaManagementChoice.ListMovies:
                        ListMovies(db);
                        break;
                    case CinemaManagementChoice.AddMovie:
                        AddMovieChoice(db);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ListMovies(CinemaContext db)
        {
            Console.Clear();

            var movies = db.Movies.ToList();
            var options = movies.Select(movie => movie.Title).ToList();
            options.Insert(0, "Terug");

            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een film")
                    .PageSize(10)
                    .AddChoices(options)
            );

            if (selectedOption == "Terug")
            {
                return;
            }

            var selectedMovie = movies.First(movie => movie.Title == selectedOption);
            HandleSelectedMovie(db, selectedMovie);
        }
        public int HandleSelectedMovie(CinemaContext db, Movie selectedMovie)
        {
            var movieOptions = Descriptions.Values.ToList();

            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wat wil je doen met de film?")
                    .PageSize(10)
                    .AddChoices(movieOptions)
            );

            var selectedChoice = Descriptions.FirstOrDefault(kv => kv.Value == selectedOption).Key;

            switch (selectedChoice)
            {
                case CinemaManagementMovieChoice.ListShowtimes:
                    ListShowtimes(db, selectedMovie);
                    break;
                case CinemaManagementMovieChoice.DeleteMovie:
                    DeleteMovie(db, selectedMovie);
                    break;
                case CinemaManagementMovieChoice.AddShowtime:
                    AddShowtime(db, selectedMovie);
                    break;
                case CinemaManagementMovieChoice.Exit:
                    return 1;
                default:
                    break;
            }
            return 0;
        }
        private void ListShowtimes(CinemaContext db, Movie selectedMovie)
        {
            Console.Clear();
            Console.WriteLine($"Vertoningen voor {selectedMovie.Title}:");

            var showtimes = db.Showtimes.Where(s => s.Movie.Id == selectedMovie.Id).ToList();

            if (showtimes.Any())
            {
                foreach (var showtime in showtimes)
                {
                    Console.WriteLine($"- Zaal ID: {showtime.RoomId}, Starttijd: {showtime.StartTime}");

                }
            }
            else
            {
                Console.WriteLine("Geen vertoningen beschikbaar voor deze film.");
            }

            Console.WriteLine("Druk op een toets om terug te gaan...");
            Console.ReadKey();
        }


        private void AddShowtime(CinemaContext db, Movie selectedMovie)
        {
            Console.Clear();

            string roomId;
            string startTime;
            bool ready;

            // Prompt user to select RoomId
            roomId = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een zaal:")
                    .AddChoices(new[] { "1", "2", "3" })
            );

            // Prompt user for StartTime
            startTime = AnsiConsole.Prompt(
                new TextPrompt<string>("Starttijd (DD-MM-JJJJ HH:mm):")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (!DateTimeOffset.TryParseExact(
                            input,
                            "dd-MM-yyyy HH:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal,
                            out _))
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
                        }
                        return ValidationResult.Success();
                    })
            );

            // Prompt user if they really want to add the movie
            ready = AnsiConsole.Confirm("Weet je zeker dat je deze film wilt toevoegen?");

            if (!ready)
                ListMovies(db);

            while (true)
            {
                if (!DateTimeOffset.TryParseExact(
                    startTime,
                    "dd-MM-yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out DateTimeOffset StartTime
                ))
                {
                    AnsiConsole.Markup($"[red]\"{startTime}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.[/]");
                    startTime = AnsiConsole.Prompt(
                        new TextPrompt<string>("Probeer Opnieuw:")
                            .PromptStyle("yellow")
                            .Validate(input =>
                            {
                                if (!DateTimeOffset.TryParseExact(
                                    input,
                                    "dd-MM-yyyy HH:mm",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.AssumeUniversal,
                                    out _))
                                {
                                    return ValidationResult.Error($"\"{input}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
                                }
                                return ValidationResult.Success();
                            })
                    );
                    continue;
                }

                var showTime = new Showtime()
                {
                    Movie = selectedMovie,
                    RoomId = roomId,
                    StartTime = StartTime,
                };

                db.Showtimes.Add(showTime);
                db.SaveChanges();

                CinemaReservationSystem cinemaSystem = CinemaReservationSystem.GetCinemaReservationSystem(showTime, db);
                AnsiConsole.Markup("[green]Film succesvol toegevoegd aan de database![/]");
                AnsiConsole.WriteLine("Druk op een toets om terug te gaan....");
                Console.ReadKey();
                break;
            }
        }



        private int AddMovieChoice(CinemaContext db)
        {
            var addMovieOptions = AddMovieChoiceDescriptions.Keys.ToList();

            var currentChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Hoe wil je de film toevoegen?")
                    .PageSize(10)
                    .AddChoices(AddMovieChoiceDescriptions.Select(kv => kv.Value))
            );

            var choiceEnum = AddMovieChoiceDescriptions.FirstOrDefault(kv => kv.Value == currentChoice).Key;

            switch (choiceEnum)
            {
                case CinemaManagementAddMovieChoice.AddMovieManually:
                    AddMovies(db);
                    break;
                case CinemaManagementAddMovieChoice.AddMovieJSON:
                    AddMoviesFromJsonToDatabase(db);
                    break;
                case CinemaManagementAddMovieChoice.Exit:
                    return 1;
                default:
                    break;
            }

            return 0;
        }
        private void AddMovies(CinemaContext db)
        {
            Console.Clear();
            var newMovie = Prompt.Bind<CreateMovieForm>();
            if (newMovie.Ready.HasValue && !newMovie.Ready.Value)
                return;

            var movie = new Movie()
            {
                Cast = newMovie.Cast,
                Title = newMovie.Title,
                Genres = newMovie.Genres,
                Duration = newMovie.Duration,
                Directors = newMovie.Directors,
                Description = newMovie.Description,
                MinAgeRating = newMovie.MinAgeRating
            };

            if (!DateTimeOffset.TryParseExact(
                newMovie.ReleaseDate,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset releaseDate
                ))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\"{newMovie.ReleaseDate}\" is geen geldige datum. Moet in DD-MM-JJJJ formaat zijn.");

            }
            else
            {
                movie.ReleaseDate = releaseDate;
                db.Movies.Add(movie);
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Film succesvol toegevoegd aan de database!");
            }

            Console.ResetColor();

            Console.WriteLine("Druk op een toets om terug te gaan....");

            Console.ReadKey();
        }

        private void DeleteMovie(CinemaContext db, Movie selectedMovie)
        {
            Console.Clear();

            var confirmDelete = AnsiConsole.Confirm($"Weet je zeker dat je \"{selectedMovie.Title}\" wilt verwijderen?");

            if (confirmDelete)
            {
                db.Movies.Remove(selectedMovie);
                db.SaveChanges();

                AnsiConsole.MarkupLine($"[green]\"{selectedMovie.Title}\" is succesvol verwijderd.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Verwijdering geannuleerd.[/]");
            }

            AnsiConsole.MarkupLine("Druk op een toets om terug te gaan....");
            Console.ReadKey(true);
        }

        public void AddMoviesFromJsonToDatabase(CinemaContext db)
        {
            MovieDataLoader movieDataLoader = new MovieDataLoader();
            List<Movie> movies = movieDataLoader.LoadMoviesFromJson("../../../Data/Json/movies.json");

            movieDataLoader.AddMoviesToDatabase(movies, db);
        }
    }
}