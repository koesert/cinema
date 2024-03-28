using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sharprompt;

namespace Cinema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            string connectionString = configuration.GetConnectionString("Main");

            CinemaContext db = new CinemaContext(connectionString);

            ManagementExperienceService service = new ManagementExperienceService();

            // GenerateShowtimes(db);

            Console.Clear();
            InitialStateChoice currentChoice = InitialStateChoice.Login;

            while (currentChoice != InitialStateChoice.Exit)
            {
                currentChoice = Prompt.Select<InitialStateChoice>("Wat wil je doen");
                switch (currentChoice)
                {
                    case InitialStateChoice.ListMovies:
                        service.ListMoviesWithShowtimes(db);
                        break;
                    case InitialStateChoice.Login:
                        string username = Prompt.Input<string>("Gebruikersnaam");
                        string password = Prompt.Password("Wachtwoord");

                        Administrator admin = db.Administrators
                            .FirstOrDefault(admin =>
                                admin.Username == username && admin.Password == password
                            );

                        if (admin == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ongeldige gebruikersnaam of wachtwoord");
                            Console.ResetColor();
                            break;
                        }
                        service.ManageCinema(admin, db, configuration);
                        break;
                }

            }

        }
        private static void GenerateShowtimes(CinemaContext db)
        {
            Random random = new Random();

            // Generate a random movie ID within the range of 2 to 17
            int movieId = random.Next(2, 18);

            // Fetch the movie with the random ID
            var movie = db.Movies.Include(m => m.Showtimes).FirstOrDefault(m => m.Id == movieId);
            if (movie != null)
            {
                // Generate showtimes
                var showtimes = ShowTimeGenerator.GenerateShowtimes(DateTimeOffset.UtcNow, 6, movie);

                // Add showtimes to database
                db.Showtimes.AddRange(showtimes);
                db.SaveChanges();
            }
        }
    }
}
