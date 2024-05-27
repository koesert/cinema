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
            { CinemaManagementMovieChoice.Exit, "Terug" }
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
        { CinemaManagementChoice.Settings, "Bioscoop Instellingen"},
        { CinemaManagementChoice.Exit, "Log Uit" }
    };
        private static readonly Dictionary<CinemaManagementVoucherChoice, string> VoucherChoiceDescriptions = new Dictionary<CinemaManagementVoucherChoice, string>
        {
            { CinemaManagementVoucherChoice.MakeVoucher, "Maak nieuwe voucher aan" },
            { CinemaManagementVoucherChoice.DeleteVoucher, "Verwijder een bestaande voucher" },
            { CinemaManagementVoucherChoice.ChangeVoucher, "Wijzig een bestaande voucher" },
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
                    case CinemaManagementChoice.Settings:
                        SettingsPanel(db);
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
            var options = movies.Select(movie => movie.Title).OrderBy(x => x).ToList();
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
            DateTimeOffset selectedDate;
            string startTime;

            // Prompt user to select RoomId
            roomId = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een zaal:")
                    .AddChoices(new[] { "1", "2", "3" })
            );

            // Prompt user to select date
            selectedDate = PromptDateSelection();

            var formattedDate = selectedDate.ToString("dd-MM-yyyy");

            var availableHours = GetAvailableHours(db, roomId, formattedDate, selectedMovie.Duration);

            if (availableHours.Any())
            {
                // Prompt user to select start time
                startTime = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Selecteer een starttijd:")
                        .AddChoices(availableHours)
                );

                if (!AnsiConsole.Confirm("Weet je zeker dat je deze film wilt toevoegen?"))
                {
                    ListMovies(db);
                    return;
                }

                DateTimeOffset showtimeStartTime;
                if (!TryParseAndValidateDateTime($"{formattedDate} {startTime}", out showtimeStartTime))
                {
                    AnsiConsole.Markup($"[red]\"{formattedDate} {startTime}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.[/]");
                    AddShowtime(db, selectedMovie);
                    return;
                }

                var showTime = new Showtime()
                {
                    Movie = selectedMovie,
                    RoomId = roomId,
                    StartTime = showtimeStartTime,
                };

                try
                {
                    db.Showtimes.Add(showTime);
                    db.SaveChanges();
                    CinemaReservationSystem cinemaSystem = CinemaReservationSystem.GetCinemaReservationSystem(showTime, db);
                    AnsiConsole.Markup("[green]Film succesvol toegevoegd aan de database![/]");
                    AnsiConsole.WriteLine("Druk op een toets om terug te gaan....");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    AnsiConsole.Markup($"[red]Er is een fout opgetreden bij het toevoegen van de film: {ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.WriteLine("Er zijn geen beschikbaar tijden\n");
                AnsiConsole.WriteLine("Druk op een toets om terug te gaan....");

                Console.ReadKey();
            }


        }

        private static DateTimeOffset PromptDateSelection()
        {
            var currentDate = DateTimeOffset.UtcNow.Date;
            var dates = Enumerable.Range(0, 28)
                                  .Select(offset => currentDate.AddDays(offset))
                                  .ToList();

            var selectedDateString = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een datum:")
                    .AddChoices(dates.Select(date => date.ToString("dd-MM-yyyy")))
            );

            return DateTimeOffset.ParseExact(selectedDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        private static bool TryParseAndValidateDateTime(string input, out DateTimeOffset result)
        {
            return DateTimeOffset.TryParseExact(
                input,
                "dd-MM-yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out result
            );
        }

        private static List<string> GetAvailableHours(CinemaContext db, string roomId, string date, int movieDuration)
        {
            var parsedDate = DateTimeOffset.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var startTime = new DateTimeOffset(parsedDate.Year, parsedDate.Month, parsedDate.Day, 12, 0, 0, TimeSpan.Zero);
            var endTime = new DateTimeOffset(parsedDate.Year, parsedDate.Month, parsedDate.Day, 22, 0, 0, TimeSpan.Zero);

            var utcStartTime = startTime.ToUniversalTime();
            var utcEndTime = endTime.ToUniversalTime();

            var bookedShowtimes = db.Showtimes
                                    .Where(s => s.RoomId == roomId && s.StartTime >= utcStartTime && s.StartTime <= utcEndTime)
                                    .Select(s => new { s.StartTime, EndTime = s.StartTime.AddMinutes(movieDuration) })
                                    .ToList();

            var availableHours = new List<string>();
            var currentTime = utcStartTime;
            while (currentTime <= utcEndTime)
            {
                // Check if the movie fits starting from the current hour
                var isMovieFit = Enumerable.Range(0, movieDuration).All(offset =>
                {
                    var currentOffset = currentTime.AddMinutes(offset);
                    return !bookedShowtimes.Any(s => currentOffset >= s.StartTime && currentOffset < s.EndTime);
                });

                if (isMovieFit)
                {
                    availableHours.Add(currentTime.ToString("HH:mm"));
                }

                // Move to the next hour
                currentTime = currentTime.AddHours(1);
            }

            return availableHours;
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
            UpdateVouchers(db);
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
                case CinemaManagementVoucherChoice.ChangeVoucher:
                    ChangeExistingVoucher(db);
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
            string dateoption = "";
            string expDate = DateTimeOffset.Now.AddMonths(6).ToString("dd-MM-yyyy HH:mm");
            DateTimeOffset date;
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
                string stringcode = AnsiConsole.Prompt(
                new TextPrompt<string>("Code voor de voucher (5-15 letters en/of nummers): ")
                    .PromptStyle("yellow")
                );
                code = LogicLayerVoucher.CodeCheck(db, stringcode);
            }

            dateoption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer VervalDatum invoer:")
                    .AddChoices(new[] { "Standaard vervaldatum (6 maanden vanaf nu)", "Handmatig vervaldatum aanmaken" })
            );
            if (dateoption.Contains("aanmaken"))
            {
                expDate = AnsiConsole.Prompt(
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
                    }));
            }

            date = DateTimeOffset.ParseExact(expDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            Customer customer = AnsiConsole.Prompt(
                new SelectionPrompt<Customer>()
                    .Title("Selecteer de klant aan wie u een voucher wilt binden:")
                    .AddChoices(db.Customers.ToList())
            );

            Console.Clear();
            AnsiConsole.Markup($"[blue]Code: {code}, VervalDatum: {expDate}, Klant: {customer.Email}[/]\n");

            string stringdiscount = AnsiConsole.Prompt(
            new TextPrompt<string>($"Voeg {discountType} korting: ")
                .PromptStyle("yellow")
            );
            discount = discountType.Contains("%") ? LogicLayerVoucher.CheckPercentDiscount(stringdiscount) : LogicLayerVoucher.CheckDiscount(stringdiscount);
            Voucher voucher = discountType.Contains("%") ? new PercentVoucher(code, discount, date, customer.Email) : new Voucher(code, discount, date, customer.Email);

            AnsiConsole.Markup($"[blue]Nieuwe Voucher: {voucher}[/]\n");
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
            List<string> vouchers = db.Vouchers.Where(x => x.IsReward == "false").Select(x => $"{x}").ToList();
            vouchers.Add("Terug");
            string stringvouchertodelete = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een voucher om te verwijderen:")
                    .AddChoices(vouchers)
            );
            if (stringvouchertodelete == "Terug")
            {
                Voucherpanel(db);
                return;
            }
            Console.Clear();
            Voucher vouchertodelete = db.Vouchers.AsEnumerable().FirstOrDefault(x => x.ToString() == stringvouchertodelete);
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

        public static void UpdateVouchers(CinemaContext db)
        {
            List<Voucher> expiredvouchers = db.Vouchers.Where(x => DateTimeOffset.UtcNow > x.ExpirationDate.UtcDateTime.AddHours(-2)).ToList();
            List<Voucher> indatevouchers = db.Vouchers.Where(x => DateTimeOffset.UtcNow <= x.ExpirationDate.UtcDateTime.AddHours(-2)).ToList();
            foreach (Voucher voucher in expiredvouchers) voucher.Active = false;
            foreach (Voucher voucher in indatevouchers) voucher.Active = true;
            db.SaveChanges();
        }

        public static void ChangeExistingVoucher(CinemaContext db)
        {
            string option;
            string code;
            List<string> vouchers = db.Vouchers.Where(x => x.IsReward == "false").Select(x => $"{x}").ToList();
            vouchers.Add("Terug");
            string stringvoucher = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een voucher om te wijzigen:")
                    .AddChoices(vouchers)
            );
            if (stringvoucher == "Terug")
            {
                Voucherpanel(db);
                return;
            }

            Console.Clear();
            Voucher originalVoucher = db.Vouchers.AsEnumerable().FirstOrDefault(x => x.ToString() == stringvoucher);
            Voucher voucher = originalVoucher.DiscountType.Contains("%") ? new PercentVoucher(originalVoucher.Code, originalVoucher.Discount, originalVoucher.ExpirationDate, originalVoucher.CustomerEmail) : new Voucher(originalVoucher.Code, originalVoucher.Discount, originalVoucher.ExpirationDate, originalVoucher.CustomerEmail);
            voucher.Id = originalVoucher.Id;

            AnsiConsole.Markup($"[blue]Gekozen Voucher: {voucher.ToString()}[/]\n");

            option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer wat u wilt wijzigen aan deze voucher:")
                    .AddChoices(new[] { "Code", "Korting(prijs en type)", "Vervaldatum", "Klant", "Terug" })
            );
            if (option == "Terug")
            {
                Console.Clear();
                ChangeExistingVoucher(db);
                return;
            }
            Console.Clear();
            if (option.Contains("Code"))
            {
                AnsiConsole.Markup($"[blue]Oude Code: {voucher.Code}[/]\n");
                string random = AnsiConsole.Prompt(
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
                    string stringcode = AnsiConsole.Prompt(
                    new TextPrompt<string>("Code voor de voucher (5-15 letters en/of nummers): ")
                        .PromptStyle("yellow")
                    );
                    code = LogicLayerVoucher.CodeCheck(db, stringcode);

                }
                voucher.Code = code;
            }
            else if (option.Contains("Korting"))
            {
                AnsiConsole.Markup($"[blue]Oude Korting: {voucher.Discount}{voucher.DiscountType}[/]\n");
                string discountType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer een kortingstype:")
                    .AddChoices(new[] { "% (voor een korting van 5% Bijvoorbeeld)", "- (voor een korting van 5,- Bijvoorbeeld)" })
                );
                string stringdiscount = AnsiConsole.Prompt(
                new TextPrompt<string>($"Voeg {discountType} korting: ")
                    .PromptStyle("yellow")
                );
                double discount = discountType.Contains("%") ? LogicLayerVoucher.CheckPercentDiscount(stringdiscount) : LogicLayerVoucher.CheckDiscount(stringdiscount);
                int id = voucher.Id;
                voucher = discountType.Contains("%") ? new PercentVoucher(voucher.Code, discount, voucher.ExpirationDate, voucher.CustomerEmail) : new Voucher(voucher.Code, discount, voucher.ExpirationDate, voucher.CustomerEmail);
                voucher.Id = id;
            }
            else if (option.Contains("datum"))
            {
                AnsiConsole.Markup($"[blue]Oude Vervaldatum: {voucher.ExpirationDate.ToString("dd-MM-yyyy HH:mm")}[/]\n");
                string expDate = DateTimeOffset.Now.AddMonths(6).ToString("dd-MM-yyyy HH:mm");
                string dateoption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecteer VervalDatum invoer:")
                    .AddChoices(new[] { "Standaard vervaldatum (6 maanden vanaf nu)", "Handmatig vervaldatum aanmaken" })
                );
                if (dateoption.Contains("aanmaken"))
                {
                    expDate = AnsiConsole.Prompt(
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
                        }));
                }
                voucher.ExpirationDate = DateTimeOffset.ParseExact(expDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }
            else if (option.Contains("Klant"))
            {
                Customer oldcustomer = db.Customers.FirstOrDefault(c => c.Email == voucher.CustomerEmail);
                AnsiConsole.Markup($"[blue]Oude Klant: {oldcustomer}[/]\n");
                Customer customer = AnsiConsole.Prompt(
                new SelectionPrompt<Customer>()
                    .Title("Selecteer de klant aan wie u een voucher wilt binden:")
                    .AddChoices(db.Customers.ToList())
                );
                voucher.CustomerEmail = customer.Email;
            }

            AnsiConsole.Markup($"[blue]Gewijzigde Voucher: {voucher.ToString()}[/]\n");
            bool ready = AnsiConsole.Confirm("Weet je zeker dat je deze voucher wilt opslaan?");
            if (!ready)
            {
                voucher = originalVoucher;
                db.SaveChanges();
                return;
            }
            db.Vouchers.Remove(originalVoucher);
            db.Vouchers.Add(voucher);
            db.SaveChanges();
            UpdateVouchers(db);
            AnsiConsole.Markup("[green]Voucher succesvol opgeslagen![/]");
            AnsiConsole.WriteLine("\nDruk op een toets om terug te gaan....");
            Console.ReadKey();
            Voucherpanel(db);
        }
        public static void ConvertPercentVouchers(CinemaContext db)
        {
            var percentVouchers = db.Vouchers.Where(v => v.DiscountType == "%").ToList();
            foreach (var voucher in percentVouchers)
            {
                var percentVoucher = new PercentVoucher(voucher.Code, voucher.Discount, voucher.ExpirationDate, voucher.CustomerEmail);
                percentVoucher.Id = voucher.Id;
                db.Vouchers.Remove(voucher);
                db.Vouchers.Add(percentVoucher);
            }
            db.SaveChanges();
        }
        public static void SettingsPanel(CinemaContext db)
        {
            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Wat wilt u aanpassen?")
                        .PageSize(10)
                        .AddChoices(new List<string>{ "Wijzig basisprijs voor periode", "Terug"})
                );
            if (choice.Contains("basisprijs"))
            {
                ChangeSeatPrice(db);
                return;
            }
            return;
        }

        public static void ChangeSeatPrice(CinemaContext db)
        {
            DateTimeOffset startdate;
            DateTimeOffset enddate;
            Administrator admin = db.Administrators.First();
            db.Entry(admin).Reload();
            DateTimeOffset now = DateTimeOffset.UtcNow.AddHours(2);
            Console.Clear();
            double basisprijs = 0;
            if (admin.PriceEndTime >= now && admin.PriceStartTime <= now)
            {
                Console.WriteLine($"Huidige basisprijs van elke vertoning die in periode {admin.PriceStartTime.ToString("dd-MM-yyyy HH:mm")} - {admin.PriceEndTime.ToString("dd-MM-yyyy HH:mm")} begint = {admin.TempPrice}");
                basisprijs = admin.TempPrice;

            }
            else
            {
                Console.WriteLine($"Huidige basisprijs voor alle vertoningen (standaard) = 25");
                if (admin.PriceEndTime >= now && admin.PriceStartTime >= now)
                {
                    Console.WriteLine($"Huidige Ingeplande periode en basisprijs {admin.PriceStartTime.ToString("dd-MM-yyyy HH:mm")} - {admin.PriceEndTime.ToString("dd-MM-yyyy HH:mm")} = {admin.TempPrice}");
                }
                else
                {
                    Console.WriteLine("Geen periode en basisprijs ingepland op dit moment.");
                }
                basisprijs = 25.0;
            }
            Console.WriteLine("Prijsverhouding stoelen:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Blauw: {basisprijs - 5},- (basisprijs - 5)");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Geel: {basisprijs},- (basisprijs)");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Rood: {basisprijs + 5},- (basisprijs + 5)");
            Console.ResetColor();
            string random = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wilt u de basisprijs en tijdsperiode aanpassen?")
                    .AddChoices(new[] { "Ja", "Nee" })
                );
                if (random.Contains("Nee"))
                {
                    Console.Clear();
                    SettingsPanel(db);
                    return;
                }
            string firstDate = AnsiConsole.Prompt(
                new TextPrompt<string>("Starttijd (DD-MM-JJJJ HH:mm):")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (!DateTimeOffset.TryParseExact(
                            input,
                            "dd-MM-yyyy HH:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal,
                            out DateTimeOffset output))
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
                        }
                        if (DateTimeOffset.UtcNow.AddHours(2).AddMinutes(-1) > output)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Mag niet in het verleden zijn");
                        }
                        return ValidationResult.Success();
                    }));
            startdate = DateTimeOffset.ParseExact(firstDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            string lastDate = AnsiConsole.Prompt(
                new TextPrompt<string>("Eindtijd (DD-MM-JJJJ HH:mm):")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (!DateTimeOffset.TryParseExact(
                            input,
                            "dd-MM-yyyy HH:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal,
                            out DateTimeOffset output))
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Moet in DD-MM-JJJJ HH:mm formaat zijn.");
                        }

                        if (startdate >= output)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Einddatum kan niet eerder zijn dan het startdatum ({startdate.ToString("dd-MM-yyyy HH:mm")}).");
                        }
                        if (DateTimeOffset.UtcNow.AddHours(2) > output)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige datum. Mag niet in het verleden zijn");
                        }

                        return ValidationResult.Success();
                    }));
            enddate = DateTimeOffset.ParseExact(lastDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            Console.Clear();
            AnsiConsole.Markup($"[blue]Gekozen tijdperiode: {startdate.ToString("dd-MM-yyyy HH:mm")} - {enddate.ToString("dd-MM-yyyy HH:mm")}[/]\n");
            double newvalue = AnsiConsole.Prompt(
                new TextPrompt<double>("Nieuwe basisprijs voor de tijdperiode (10-50) (moet een heel getal zijn):")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (input % 1 != 0)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige waarde. Moet een heel getal zijn.");
                        }
                        if (input < 0)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige waarde. Mag geen negatief getal zijn.");
                        }
                        if (input < 10 || input > 50)
                        {
                            return ValidationResult.Error($"\"{input}\" is geen geldige waarde. Overschrijdt grenswaarden (10-50).");
                        }
                        return ValidationResult.Success();
                    }));
            Console.Clear();
            AnsiConsole.Markup($"[blue]Basisprijs: {newvalue} voor alle vertoningen in Gekozen tijdperiode: {startdate.ToString("dd-MM-yyyy HH:mm")} - {enddate.ToString("dd-MM-yyyy HH:mm")}[/]\n");
            Console.WriteLine("Nieuwe Prijsverhouding stoelen:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Blauw: {newvalue - 5},- (basisprijs - 5)");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Geel: {newvalue},- (basisprijs)");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Rood: {newvalue + 5},- (basisprijs + 5)");
            Console.ResetColor();
            bool ready = AnsiConsole.Confirm("Weet je zeker dat je dit nieuwe waarde en tijdsperiode wilt opslaan?");
            if (!ready)
            {
                SettingsPanel(db);
                return;
            }
            admin.PriceStartTime = startdate;
            admin.PriceEndTime = enddate;
            admin.TempPrice = newvalue;
            db.SaveChanges();
            ConfigureSeatPrices(db);
            AnsiConsole.Markup("[green]Basisprijs succesvol opgeslagen![/]");
            AnsiConsole.WriteLine("\nDruk op een toets om terug te gaan....");
            Console.ReadKey();
            Console.Clear();
            SettingsPanel(db);
        }
        public static void ConfigureSeatPrices(CinemaContext db)
        {
            Administrator admin = db.Administrators.First();
            db.Entry(admin).Reload();
            DateTimeOffset now = DateTimeOffset.UtcNow.AddHours(2);
            double baseprice = 25;
            if (admin.PriceEndTime >= now && admin.PriceStartTime <= now)
            {
                baseprice = admin.TempPrice;
                foreach (CinemaSeat s in db.CinemaSeats.Where(s => admin.PriceEndTime >= s.Showtime.StartTime && admin.PriceStartTime <= s.Showtime.StartTime))
                {
                    s.Price = (decimal)baseprice;
                    s.Price += s.Color == "red" ? 5 : s.Color == "blue" ? -5 : 0;
                    if (s.Type == 1) s.Price += 5;
                    if (s.Type == 2) s.Price = s.Price*2;
                }
            }
            else
            {
                foreach (CinemaSeat s in db.CinemaSeats)
                {
                    s.Price = (decimal)baseprice;
                    s.Price += s.Color == "red" ? 5 : s.Color == "blue" ? -5 : 0;
                    if (s.Type == 1) s.Price += 5;
                    if (s.Type == 2) s.Price = s.Price*2;
                }
            }
            db.SaveChanges();
        }
    }
}