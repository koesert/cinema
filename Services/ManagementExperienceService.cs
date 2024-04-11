using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Models.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sharprompt;
using System.Globalization;

namespace Cinema.Services
{
    public class ManagementExperienceService
    {
        public void ManageCinema(Administrator admin, CinemaContext db, IConfiguration configuration)
        {
            CinemaManagementChoice currentManagerChoice = CinemaManagementChoice.ListMovies;
            while (currentManagerChoice != CinemaManagementChoice.Exit)
            {
                Console.Clear();

                currentManagerChoice = Prompt.Select<CinemaManagementChoice>($"Welkom, {admin.Username}, wat wil je doen?");

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

        private void ListMovies(CinemaContext db)
        {
            Console.Clear();

            var movies = db.Movies.ToList();
            var selectedMovie = Prompt.Select("Selecteer een film", movies, textSelector: movie => movie.Title);

            HandleSelectedMovie(db, selectedMovie);
        }
        private int HandleSelectedMovie(CinemaContext db, Movie selectedMovie)
        {
            var movieOptions = new[]
            {
        CinemaManagementMovieChoice.ListShowtimes,
        CinemaManagementMovieChoice.DeleteMovie,
        CinemaManagementMovieChoice.AddShowtime,
        CinemaManagementMovieChoice.Exit
    };

            CinemaManagementMovieChoice currentChoice = Prompt.Select<CinemaManagementMovieChoice>("Wat wil je doen met de film?", movieOptions);
            switch (currentChoice)
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

            var showTime = new Showtime()
            {
                Movie = selectedMovie,
                RoomId = createShowtime.RoomId,
            };

            if (!DateTimeOffset.TryParseExact(
                createShowtime.StartTime,
                "dd-MM-yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset StartTime
                ))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\"{createShowtime.StartTime}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
            }

            showTime.StartTime = StartTime;
            db.Showtimes.Add(showTime);
            db.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Film succesvol toegevoegd aan de database!");
            Console.ResetColor();

            Console.WriteLine("Druk op een toets om terug te gaan....");
            Console.ReadKey();

        }

        private int AddMovieChoice(CinemaContext db)
        {
            var AddMovieOptions = new[]
            {
        CinemaManagementAddMovieChoice.AddMovieManually,
        CinemaManagementAddMovieChoice.AddMovieJSON,
        CinemaManagementAddMovieChoice.Exit
            };


            CinemaManagementAddMovieChoice currentChoice = Prompt.Select<CinemaManagementAddMovieChoice>("Hoe wil je de film toevoegen?", AddMovieOptions);

            switch (currentChoice)
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

            var confirmDelete = Prompt.Confirm($"Weet je zeker dat je \"{selectedMovie.Title}\" wilt verwijderen?");


            if (confirmDelete)
            {

                db.Movies.Remove(selectedMovie);
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\"{selectedMovie.Title}\" is succesvol verwijderd.");
                Console.ResetColor();

            }
            Console.WriteLine("Druk op een toets om terug te gaan....");
            Console.ReadKey();
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

            var moviesQuery = db.Movies.Include(m => m.Showtimes); // Include Showtimes here
            DateTime today = DateTime.UtcNow.Date;
            int currentWeek = 0;

            while (true)
            {
                DateTime startOfWeek = today.AddDays(7 * currentWeek);
                DateTime endOfWeek = startOfWeek.AddDays(8);

                var moviesWithShowtimes = moviesQuery
                    .Where(m => m.Showtimes != null && m.Showtimes.Any(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek))
                    .ToList();

                List<string> options = new List<string> { "Filter door films" }; // Clear options list here

                options.AddRange(moviesWithShowtimes.Select(m => m.Title));
                System.Console.WriteLine();
                if (currentWeek < 3) options.Add("Volgende week");
                if (currentWeek > 0) options.Add("Vorige week");
                options.Add("Terug");

                var selectedOption = Prompt.Select($"Week van {startOfWeek:dd-MM} tot {endOfWeek.AddDays(-1):dd-MM}", options);

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
                    var filteredMovies = ApplyFilters(db).Include(m => m.Showtimes); // Apply filters
                    moviesQuery = filteredMovies; // Update moviesQuery
                    Console.Clear();
                    continue;
                }

                var selectedMovie = moviesWithShowtimes.First(m => m.Title == selectedOption);
                Console.WriteLine($"Je hebt \"{selectedMovie.Title}\" geselecteerd.");


                DisplayMovieDetails(selectedMovie);

                Console.WriteLine();

                var showtimesThisWeek = selectedMovie.Showtimes
                    .Where(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek)
                    .ToList().OrderBy(s=> s.StartTime);

                var selectedShowtime = Prompt.Select("Selecteer een voorstellingstijd", showtimesThisWeek,

                textSelector: showtime => showtime.StartTime.ToString("ddd, MMMM d hh:mm tt"));

                // Console.WriteLine($"You selected the showtime: {selectedShowtime.StartTime}");

                var cinemahall = selectedShowtime.RoomId;
                Console.Clear();
                ShowCinemaHall(db, cinemahall);
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
                string continueReserving = Console.ReadLine();
                if (continueReserving.ToUpper() != "Y")
                {
                    break;
                }
            }


            Console.ReadLine();
        }




        private IQueryable<Movie> ApplyFilters(CinemaContext db)
        {
            var allMovies = db.Movies.ToList();
            var moviesQuery = allMovies.AsQueryable();

            var filterOption = Prompt.Select<CinemaFilterChoice>("Selecteer een optie om films te filteren",
                new[] { CinemaFilterChoice.Genres, CinemaFilterChoice.Directors, CinemaFilterChoice.Cast });

            switch (filterOption)
            {
                case CinemaFilterChoice.Genres:
                    var allGenres = allMovies.SelectMany(movie => movie.Genres ?? Enumerable.Empty<string>())
                                             .Where(genre => genre != null)
                                             .Distinct()
                                             .ToList();
                    var selectedGenres = Prompt.MultiSelect("Selecteer genres om films te filteren", allGenres);
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
                    var selectedActors = Prompt.MultiSelect("Selecteer acteurs om films te filteren", allActors);
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
                    var selectedDirectors = Prompt.MultiSelect("Selecteer regisseurs om films te filteren", allDirectors);
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