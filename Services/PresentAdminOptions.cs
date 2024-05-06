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
    public static class PresentAdminOptions
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
        { CinemaManagementChoice.VoucherPanel, "Beheer Vouchers"},
        { CinemaManagementChoice.Exit, "Terug" }
    };
        private static readonly Dictionary<CinemaManagementVoucherChoice, string> VoucherChoiceDescriptions = new Dictionary<CinemaManagementVoucherChoice, string>
        {
            { CinemaManagementVoucherChoice.MakeVoucher, "Maak nieuwe voucher aan" },
            { CinemaManagementVoucherChoice.DeleteVoucher, "Verwijder een bestaande voucher" },
            { CinemaManagementVoucherChoice.Exit, "Terug" }
        };
        public static void Start(Administrator admin, CinemaContext db)
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
                    case CinemaManagementChoice.VoucherPanel:
                        Voucherpanel(db);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ListMovies(CinemaContext db)
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
        public static int HandleSelectedMovie(CinemaContext db, Movie selectedMovie)
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
        private static void ListShowtimes(CinemaContext db, Movie selectedMovie)
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


        private static void AddShowtime(CinemaContext db, Movie selectedMovie)
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



        private static int AddMovieChoice(CinemaContext db)
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
        private static void AddMovies(CinemaContext db)
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

        private static void DeleteMovie(CinemaContext db, Movie selectedMovie)
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

        public static void AddMoviesFromJsonToDatabase(CinemaContext db)
        {
            MovieDataLoader movieDataLoader = new MovieDataLoader();
            List<Movie> movies = movieDataLoader.LoadMoviesFromJson("../../../Data/Json/movies.json");

            movieDataLoader.AddMoviesToDatabase(movies, db);
        }

        public static int Voucherpanel(CinemaContext db)
        {
            Console.Clear();
            var VoucherOptions = VoucherChoiceDescriptions.Keys.ToList();

            var currentChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wat wilt u aan vouchers wijzigen?")
                    .PageSize(10)
                    .AddChoices(VoucherChoiceDescriptions.Select(kv => kv.Value))
            );

            var choiceEnum = VoucherChoiceDescriptions.FirstOrDefault(kv => kv.Value == currentChoice).Key;

            switch (choiceEnum)
            {
                case CinemaManagementVoucherChoice.MakeVoucher:
                    MakeVoucher(db);
                    break;
                case CinemaManagementVoucherChoice.DeleteVoucher:
                    DeleteVoucher(db);
                    break;
                case CinemaManagementVoucherChoice.Exit:
                    return 1;
                default:
                    break;
            }

            return 0;
        }

        public static void MakeVoucher(CinemaContext db)
        {
            Console.Clear();

            string discountType;
            string code;
            string random;
            double discount;
            bool ready;

            // Prompt user to select RoomId
            discountType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een kortingstype:")
                    .AddChoices(new[] { "% (voor een korting van 5% Bijvoorbeeld)", "- (voor een korting van 5,- Bijvoorbeeld)" })
            );
            random = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer Code Invoer:")
                    .AddChoices(new[] { "Genereer code", "Handmatig code aanmaken" })
            );
            if (random.Contains("Genereer"))
            {
                code = LogicLayerVoucher.GenerateRandomCode(db);
            }
            else
            {
                code = AnsiConsole.Prompt(
                new TextPrompt<string>("Code voor de voucher (5-15 letters en/of nummers): ")
                    .PromptStyle("yellow")
                    .Validate(input => 
                    {
                        string result = LogicLayerVoucher.CodeCheck(db, input);
                        return true;
                    })
                );
            }

            Console.Clear();
            AnsiConsole.Markup($"[blue]Code: {code}[/]\n");

            discount = AnsiConsole.Prompt(
            new TextPrompt<double>($"Voeg {discountType} korting: ")
                .PromptStyle("yellow")
            );
            discount = discountType.Contains("%") ? LogicLayerVoucher.CheckPercentDiscount(Convert.ToString(discount)) : LogicLayerVoucher.CheckDiscount(Convert.ToString(discount));
            Voucher voucher = discountType.Contains("%") ? new PercentVoucher(code, discount) : new Voucher(code, discount);

            AnsiConsole.Markup($"[blue]Nieuwe Voucher: {voucher.ToString()}[/]\n");
            ready = AnsiConsole.Confirm("Weet je zeker dat je deze voucher wilt toevoegen?");
            if (!ready)
                return;

            db.Vouchers.Add(voucher);
            db.SaveChanges();
            AnsiConsole.Markup("[green]Voucher succesvol toegevoegd![/]");
            AnsiConsole.WriteLine("\nDruk op een toets om terug te gaan....");
            Console.ReadKey();
        }

        public static void DeleteVoucher(CinemaContext db)
        {
            List<Voucher> vouchers = db.Vouchers.ToList();
            Voucher vouchertodelete = AnsiConsole.Prompt(
                new SelectionPrompt<Voucher>()
                    .Title("Selecteer een voucher om te verwijderen:")
                    .AddChoices(vouchers)
            );

            Console.Clear();

            AnsiConsole.Markup($"[blue]Gekozen Voucher: {vouchertodelete.ToString()}[/]\n");
            bool ready = AnsiConsole.Confirm("Weet je zeker dat je deze voucher wilt verwijderen?");
            if (!ready)
                return;

            db.Vouchers.Remove(vouchertodelete);
            db.SaveChanges();
            AnsiConsole.Markup("[green]Voucher succesvol verwijderd![/]");
            AnsiConsole.WriteLine("\nDruk op een toets om terug te gaan....");
            Console.ReadKey();
        }
    }
}