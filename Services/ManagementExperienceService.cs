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
            var selectedMovie = AnsiConsole.Prompt(
                new SelectionPrompt<Movie>()
                    .Title("Selecteer een film")
                    .PageSize(10)
                    .AddChoices(movies)
                    .UseConverter(movie => movie.Title)
            );

            HandleSelectedMovie(db, selectedMovie);
        }
        public int HandleSelectedMovie(CinemaContext db, Movie selectedMovie)
        {
            var movieOptions = Enum.GetValues(typeof(CinemaManagementMovieChoice)).Cast<CinemaManagementMovieChoice>();

            var index = AnsiConsole.Prompt(
                new SelectionPrompt<CinemaManagementMovieChoice>()
                    .Title("Wat wil je doen met de film?")
                    .PageSize(10)
                    .AddChoices(movieOptions)
            );

            switch (index)
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

            var createShowtime = Prompt.Bind<CreateShowtimeForm>();

            if (createShowtime.Ready.HasValue && !createShowtime.Ready.Value)
                return;

            while (true)
            {
                if (!DateTimeOffset.TryParseExact(
                createShowtime.StartTime,
                "dd-MM-yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out DateTimeOffset StartTime
                ))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\"{createShowtime.StartTime}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
                    Console.ResetColor();
                    Console.WriteLine("Probeer Opnieuw:");
                    createShowtime = Prompt.Bind<CreateShowtimeForm>();
                    continue;
                }

                var showTime = new Showtime()
                {
                    Movie = selectedMovie,
                    RoomId = createShowtime.RoomId,
                    StartTime = StartTime,
                };

                db.Showtimes.Add(showTime);
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Film succesvol toegevoegd aan de database!");
                Console.ResetColor();

                Console.WriteLine("Druk op een toets om terug te gaan....");
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

        public void ListMoviesWithShowtimes(CinemaContext db)
        {
            Console.Clear();

            var moviesQuery = db.Movies.Include(m => m.Showtimes);
            DateTime today = DateTime.UtcNow.Date;
            int currentWeek = 0;

            while (true)
            {
                DateTime startOfWeek = today.AddDays(7 * currentWeek);
                DateTime endOfWeek = startOfWeek.AddDays(8);

                var moviesWithShowtimes = moviesQuery
                    .Where(m => m.Showtimes != null && m.Showtimes.Any(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek))
                    .ToList();

                var options = new List<string> { "Filter door films" };

                options.AddRange(moviesWithShowtimes.Select(m => m.Title));
                AnsiConsole.MarkupLine("");
                if (currentWeek < 3) options.Add("Volgende week");
                if (currentWeek > 0) options.Add("Vorige week");
                options.Add("Terug");

                var selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Week van {startOfWeek:dd-MM} tot {endOfWeek.AddDays(-1):dd-MM}")
                        .AddChoices(options)
                        .PageSize(10)
                );

                if (selectedOption == "Volgende week")
                {
                    currentWeek++;
                    Console.Clear();
                    continue;
                }
                else if (selectedOption == "Vorige week")
                {
                    currentWeek--;
                    Console.Clear();
                    continue;
                }
                else if (selectedOption == "Terug")
                {
                    break;
                }
                else if (selectedOption == "Filter door films")
                {
                    var filteredMovies = ApplyFilters(db).Include(m => m.Showtimes);
                    moviesQuery = filteredMovies;
                    Console.Clear();
                    continue;
                }

                var selectedMovie = moviesWithShowtimes.First(m => m.Title == selectedOption);
                AnsiConsole.MarkupLine($"Je hebt \"{selectedMovie.Title}\" geselecteerd.");

                DisplayMovieDetails(selectedMovie);
                AnsiConsole.MarkupLine("");

                var showtimesThisWeek = selectedMovie.Showtimes
                    .Where(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek && s.StartTime >= DateTime.UtcNow + TimeSpan.FromHours(2))
                    .OrderBy(s => s.StartTime)
                    .ToList();

                var selectedShowtime = AnsiConsole.Prompt(
                    new SelectionPrompt<Showtime>()
                        .Title("Selecteer een voorstellingstijd")
                        .AddChoices(showtimesThisWeek)
                        .UseConverter(showtime => showtime.StartTime.ToString("ddd, MMMM d hh:mm tt"))
                        .PageSize(10)
                );

                if (selectedMovie.MinAgeRating >= 16)
                {
                    AnsiConsole.MarkupLine("[red]Let op: Deze film heeft een minimum leeftijd van 16 jaar of ouder.[/]");
                    AnsiConsole.MarkupLine("Wil je doorgaan? (Ja/Nee)");

                    var confirmation = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices("Ja", "Nee")
                    );

                    if (confirmation.ToLower() == "ja")
                    {
                        var cinemahall = selectedShowtime.RoomId;
                        Console.Clear();
                        ShowCinemaHall(db, cinemahall);
                    }
                    else
                    {
                        Console.Clear();
                        ListMoviesWithShowtimes(db);
                    }
                }
                break;
            }
        }

        public void ShowCinemaHall(CinemaContext db, string cinemahall)
        {
            CinemaReservationSystem cinemaSystem = CinemaReservationSystem.GetCinemaReservationSystem(cinemahall);
            cinemaSystem.DrawPlan();

            Console.WriteLine("\n\n");

            while (true)
            {
                Console.WriteLine("Voer het aantal stoelen in dat je wilt reserveren:");
                int numSeatsToReserve;
                while (!int.TryParse(Console.ReadLine(), out numSeatsToReserve) || numSeatsToReserve < 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ongeldige invoer. Voer een geldig aantal stoelen in om te reserveren.");
                    Console.ResetColor();
                }

                List<CinemaSeat> selectedSeats = new List<CinemaSeat>();

                for (int i = 0; i < numSeatsToReserve; i++)
                {
                    char selectedRow;
                    char endRow = 'T';
                    if (cinemahall == "1") endRow = 'N';
                    if (cinemahall == "2") endRow = 'S';

                    Console.WriteLine($"Voer de rij in (A-{endRow}) voor stoel {i + 1}:");
                    while (!char.TryParse(Console.ReadLine().ToUpper(), out selectedRow) || selectedRow < 'A' || selectedRow > endRow)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ongeldige rij. Voer een geldige rij in (A-{endRow}).");
                        Console.ResetColor();
                    }

                    int selectedSeatNumber;
                    while (true)
                    {
                        Console.WriteLine($"Voer het stoelnummer in voor stoel {i + 1} (1-{cinemaSystem.Seats[0].Count}):");
                        if (int.TryParse(Console.ReadLine(), out selectedSeatNumber) && selectedSeatNumber >= 1 && selectedSeatNumber <= cinemaSystem.Seats[0].Count)
                        {
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Ongeldig stoelnummer. Voer een nummer in tussen 1 en {cinemaSystem.Seats[0].Count}.");
                            Console.ResetColor();
                        }
                    }

                    CinemaSeat selectedSeat = cinemaSystem.FindSeat(selectedRow, selectedSeatNumber);
                    if (selectedSeat == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Stoel {selectedRow}{selectedSeatNumber} bestaat niet");
                        Console.ResetColor();
                        i--;
                        continue;
                    }
                    else if (selectedSeat.IsReserved)
                    {
                        Console.Clear();
                        cinemaSystem.DrawPlan();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Stoel {selectedRow}{selectedSeatNumber} is al gereserveerd.");
                        Console.ResetColor();
                        i--;
                        continue;
                    }


                    selectedSeats.Add(cinemaSystem.FindSeat(selectedRow, selectedSeatNumber));
                }

                Console.Clear();
                bool allSeatsAvailable = true;
                foreach (CinemaSeat selectedSeat in selectedSeats)
                {
                    if (selectedSeat == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Op tenminste één van de geselecteerde plaatsen zit geen stoel.");
                        Console.ResetColor();
                        allSeatsAvailable = false;
                        break;
                    }
                    else if (selectedSeat.IsReserved)
                    {
                        Console.Clear();
                        cinemaSystem.DrawPlan();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Stoel {selectedSeat.Row}{selectedSeat.SeatNumber} is al gereserveerd.");
                        Console.ResetColor();
                        allSeatsAvailable = false;
                        break;
                    }
                }
                // Console.Clear();
                if (allSeatsAvailable)
                {
                    bool allSeatsReserved = true;
                    foreach (CinemaSeat selectedSeat in selectedSeats)
                    {
                        if (!cinemaSystem.ReserveSeat(selectedSeat.Row, selectedSeat.SeatNumber))
                        {
                            allSeatsReserved = false;
                            break;
                        }
                    }
                    if (allSeatsReserved)
                    {
                        foreach (CinemaSeat selectedSeat in selectedSeats)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Stoel {selectedSeat.Row}{selectedSeat.SeatNumber} succesvol gereserveerd!");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Reservering mislukt. Sommige van de geselecteerde stoelen zijn al gereserveerd.");
                        Console.ResetColor();
                    }
                }

                cinemaSystem.DrawPlan();

                Console.WriteLine("Wil je meer stoelen reserveren? (Y/N)");
                string continueReserving = Console.ReadLine().ToLower();
                if (continueReserving != "y")
                {
                    break;
                }
            }
        }

        private IQueryable<Movie> ApplyFilters(CinemaContext db)
        {
            var allMovies = db.Movies.ToList();
            var moviesQuery = allMovies.AsQueryable();

            var filterOption = AnsiConsole.Prompt(
                new SelectionPrompt<CinemaFilterChoice>()
                    .Title("Selecteer een optie om films te filteren")
                    .PageSize(3)
                    .AddChoices(new[]
                    {
                CinemaFilterChoice.Genres,
                CinemaFilterChoice.Directors,
                CinemaFilterChoice.Cast
                    })
            );

            switch (filterOption)
            {
                case CinemaFilterChoice.Genres:
                    var allGenres = allMovies.SelectMany(movie => movie.Genres ?? Enumerable.Empty<string>())
                                            .Where(genre => genre != null)
                                            .Distinct()
                                            .ToList();
                    var selectedGenres = AnsiConsole.Prompt(
                        new MultiSelectionPrompt<string>()
                            .Title("Selecteer genres om films te filteren")
                            .PageSize(10)
                            .AddChoices(allGenres)
                    );
                    // Filter films op basis van geselecteerde genres
                    moviesQuery = allMovies.Where(movie => movie.Genres != null &&
                        movie.Genres.Intersect(selectedGenres ?? Enumerable.Empty<string>()).Any())
                        .AsQueryable();
                    break;

                case CinemaFilterChoice.Cast:
                    var allActors = allMovies.SelectMany(movie => movie.Cast ?? Enumerable.Empty<string>())
                                            .Where(actor => actor != null)
                                            .Distinct()
                                            .ToList();
                    var selectedActors = AnsiConsole.Prompt(
                        new MultiSelectionPrompt<string>()
                            .Title("Selecteer acteurs om films te filteren")
                            .PageSize(10)
                            .AddChoices(allActors)
                    );
                    // Filter films op basis van geselecteerde acteurs
                    moviesQuery = allMovies.Where(movie => movie.Cast != null &&
                        movie.Cast.Intersect(selectedActors ?? Enumerable.Empty<string>()).Any())
                        .AsQueryable();
                    break;

                case CinemaFilterChoice.Directors:
                    var allDirectors = allMovies.SelectMany(movie => movie.Directors ?? Enumerable.Empty<string>())
                                                .Where(director => director != null)
                                                .Distinct()
                                                .ToList();
                    var selectedDirectors = AnsiConsole.Prompt(
                        new MultiSelectionPrompt<string>()
                            .Title("Selecteer regisseurs om films te filteren")
                            .PageSize(10)
                            .AddChoices(allDirectors)
                    );
                    // Filter films op basis van geselecteerde regisseurs
                    moviesQuery = allMovies.Where(movie => movie.Directors != null &&
                        movie.Directors.Intersect(selectedDirectors ?? Enumerable.Empty<string>()).Any())
                        .AsQueryable();
                    break;

                default:
                    break;
            }

            return moviesQuery;
        }
        private void DisplayMovieDetails(Movie movie)
        {
            Console.WriteLine($"Details voor \"{movie.Title}\":");
            Console.WriteLine($"- Genre: {string.Join(", ", movie.Genres)}");
            Console.WriteLine($"- Duur: {movie.Duration} minuten");
            Console.WriteLine($"- Regisseurs: {string.Join(", ", movie.Directors)}");
            Console.WriteLine($"- Beschrijving: {movie.Description}");
            Console.WriteLine($"- Minimum leeftijdsbeoordeling: {movie.MinAgeRating}");
            Console.WriteLine();
        }
    }
}