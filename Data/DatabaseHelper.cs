using System.Data;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;

public class DatabaseHelper
{
    private SQLiteConnection _connection;
    private string _databasePath;
    private string _moviesFolderPath = "../../../movies/";

    public DatabaseHelper()
    {
        // _databasePath = databasePath;
        _databasePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "../../../db/cinema.db"
        );

        string dbFolderPath = Path.GetDirectoryName(_databasePath);
        // Check if the database file exists
        if (dbFolderPath != null)
        {
            if (!Directory.Exists(dbFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(dbFolderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("Error: Database folder path is null.");
        }

        // Create a new SQLite connection with the specified database path
        _connection = new SQLiteConnection($"Data Source={_databasePath}");
    }

    // Method to open the connection to the database
    public void OpenConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    // Method to close the connection to the database
    public void CloseConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    // Method to execute a SQL query and return the result as a DataTable
    public DataTable ExecuteQuery(string query)
    {
        try
        {
            // Open the connection to the database
            OpenConnection();

            SQLiteCommand command = new SQLiteCommand(query, _connection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
            return null;
        }
        finally
        {
            // Close the connection to the database
            CloseConnection();
        }
    }

    // Execute a SQL query to create tables
    public void CreateTables()
    {
        string createTableQuery =
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
        ExecuteQuery(createTableQuery);
    }

    // Method to read JSON file and add data to the database
    public void AddMoviesFromJson(string jsonFilePath)
    {
        try
        {
            // Read the contents of the JSON file
            string jsonData = File.ReadAllText(jsonFilePath);

            // Deserialize the JSON into a list of Movie objects
            List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(jsonData);

            // Add each movie to the database
            foreach (var movie in movies)
            {
                AddMovieToDatabase(movie);
            }

            // Console.WriteLine("Movies added to the database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding movies from JSON: {ex.Message}");
        }
    }

    private void AddMovieToDatabase(Movie movie)
    {
        try
        {
            // Check if the movie already exists in the database based on title
            DataTable existingMovie = ExecuteQuery(
                $"SELECT * FROM Movies WHERE Title = '{movie.Title}'"
            );

            if (existingMovie.Rows.Count == 0)
            {
                // Build the SQL INSERT statement
                string insertQuery =
                    @"
                INSERT INTO Movies (Title, Description, Duration, Year, Genres, Cast)
                VALUES (@Title, @Description, @Duration, @Year, @Genres, @Cast);";

                // Create a SQLiteCommand object
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, _connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@Description", movie.Description);
                    command.Parameters.AddWithValue("@Duration", movie.Duration);
                    command.Parameters.AddWithValue("@Year", movie.Year);
                    command.Parameters.AddWithValue("@Genres", string.Join(",", movie.Genres));
                    command.Parameters.AddWithValue("@Cast", string.Join(",", movie.Cast));

                    // Open the database connection
                    OpenConnection();

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Close the database connection
                    CloseConnection();
                }
            }
            // else
            // {
            //     Console.WriteLine($"Movie '{movie.Title}' already exists in the database. Skipping insertion.");
            // }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding movie {movie.Title}: {ex.Message}");
        }
    }

    // Method to add movies from JSON files in the movie folder
    public void AddMoviesFromFolder()
    {
        try
        {
            // Search for JSON files in the specified folder
            string[] jsonFiles = Directory.GetFiles(_moviesFolderPath, "*.json");

            // Add movies from each found JSON file
            foreach (var jsonFile in jsonFiles)
            {
                AddMoviesFromJson(jsonFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding movies from folder: {ex.Message}");
        }
    }

    public void InitializeDatabase()
    {
        try
        {
            // Open the connection to the database
            OpenConnection();

            // Create tables if they don't exist
            CreateTables();

            // Add movies from the specified folder
            AddMoviesFromFolder();

            // Console.WriteLine("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
        finally
        {
            // Close the connection to the database
            CloseConnection();
        }
    }
}
