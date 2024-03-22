using Newtonsoft.Json;
using System.Data;
using System.Data.SQLite;
public class Movie
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int Duration { get; set; }

    public int Year { get; set; }

    public List<string> Directors { get; set; }

    public List<string> Genres { get; set; }

    public List<string> Cast { get; set; }

    public static List<Movie> Movies = new List<Movie>();

    private static readonly string _moviesFolderPath = "../../../movies/";


    private static void CreateMoviesTable()
    {
        string createMoviesTableQuery =
            @"
            CREATE TABLE IF NOT EXISTS Movies (
                MovieId INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT,
                Duration INTEGER,
                Year INTEGER,
                Genres TEXT,
                Cast TEXT
            );";
        Database.OpenConnection();
        Database.ExecuteQuery(createMoviesTableQuery);
        Database.CloseConnection();
    }
    private static void AddMoviesToDB()
    {
        string[] jsonFiles = Directory.GetFiles(_moviesFolderPath, "*.json");

        foreach (var jsonFile in jsonFiles)
        {
            string jsonData = File.ReadAllText(jsonFile);
            List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(jsonData);
            
            foreach (Movie movie in movies)
            {
                Database.OpenConnection();
                try
                {
                    if (Database.Connection != null)
                    {
                        DataTable existingMovie = Database.ExecuteQuery(
                          $"SELECT * FROM Movies WHERE Title = '{movie.Title}'"
                        );

                        Database.CloseConnection();

                        if (existingMovie.Rows.Count == 0)
                        {
                            Database.OpenConnection();
                            string insertQuery =
                              @"
                                INSERT INTO Movies (Title, Description, Duration, Year, Genres, Cast)
                                VALUES (@Title, @Description, @Duration, @Year, @Genres, @Cast);";

                            using (SQLiteCommand command = new SQLiteCommand(insertQuery, Database.Connection))
                            {
                                command.Parameters.AddWithValue("@Title", movie.Title);
                                command.Parameters.AddWithValue("@Description", movie.Description);
                                command.Parameters.AddWithValue("@Duration", movie.Duration);
                                command.Parameters.AddWithValue("@Year", movie.Year);
                                command.Parameters.AddWithValue("@Genres", string.Join(",", movie.Genres));
                                command.Parameters.AddWithValue("@Cast", string.Join(",", movie.Cast));

                                command.ExecuteNonQuery();

                                
                            }
                            Database.CloseConnection();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Database connection is not established.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding movie {movie.Title}: {ex.Message}");
                }
            }
        }
    }


    private static void AddMoviesToList()
    {
        try
        {
            string[] jsonFiles = Directory.GetFiles(_moviesFolderPath, "*.json");
            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    string jsonData = File.ReadAllText(jsonFile);
                    List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(jsonData);
                    foreach (Movie movie in movies)
                    {
                        Movies.Add(movie);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing JSON file: {jsonFile}. Exception: {ex.Message}");
                }
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"Movies folder not found: {_moviesFolderPath}. Exception: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading movie files from {_moviesFolderPath}. Exception: {ex.Message}");
        }
    }

    public static void InitializeMovies()
    {
        CreateMoviesTable();
        AddMoviesToDB();
        AddMoviesToList();
    }
}