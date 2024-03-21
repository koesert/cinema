using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;

public static class DatabaseHelper
{
    private static SQLiteConnection _connection;
    private static string _databasePath;
    private static string _moviesFolderPath = "../../../movies/";

    // Constructor to set up database path and connection
    static DatabaseHelper()
    {
        // Set the path for the SQLite database file
        _databasePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "../../../db/cinema.db"
        );

        // Check if the folder containing the database file exists, if not, create it
        string dbFolderPath = Path.GetDirectoryName(_databasePath);
        if (dbFolderPath != null && !Directory.Exists(dbFolderPath))
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

        // Create a new SQLite connection with the specified database path
        _connection = new SQLiteConnection($"Data Source={_databasePath}");
    }

    // Method to open the connection to the database
    private static void OpenConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    // Method to close the connection to the database
    private static void CloseConnection()
    {
        if (_connection.State != ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    // Method to execute a SQL query and return the result as a DataTable
    public static DataTable ExecuteQuery(string query)
    {
        try
        {
            // Open the connection to the database
            OpenConnection();

            // Create a SQLiteCommand object with the query and connection
            using (SQLiteCommand command = new SQLiteCommand(query, _connection))
            {
                // Create a SQLiteDataAdapter to fill the DataTable with query results
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    // Fill the DataTable with query results
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
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

    // Method to create necessary tables in the database
    private static void CreateTables()
    {
        // SQL query to create Movies table if it doesn't exist
        string createTableQuery =
            @"
            CREATE TABLE IF NOT EXISTS Movies (
                MovieId INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT,
                Duration INTEGER,
                Year INTEGER,
                Genres TEXT,
                Cast TEXT,
                Directors TEXT,
                IsRated16Plus INTEGER
            );";
        // Execute the create table query
        ExecuteQuery(createTableQuery);
    }

    // Method to read JSON file and add movie data to the database
    private static void AddMoviesFromJson(string jsonFilePath)
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding movies from JSON: {ex.Message}");
        }
    }

    // Helper method to add a movie to the database
    private static void AddMovieToDatabase(Movie movie)
    {
        try
        {
            // Check if the movie already exists in the database based on title
            DataTable existingMovie = ExecuteQuery(
                $"SELECT * FROM Movies WHERE Title = '{movie.Title}'"
            );

            // If movie doesn't exist in the database, add it
            if (existingMovie.Rows.Count == 0)
            {
                // SQL query to insert a movie into the Movies table
                string insertQuery =
                    @"
                INSERT INTO Movies (Title, Description, Duration, Year, Genres, Cast, Directors, IsRated16Plus)
                VALUES (@Title, @Description, @Duration, @Year, @Genres, @Cast, @Directors, @IsRated16Plus);";

                // Create a SQLiteCommand object with the insert query and connection
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, _connection))
                {
                    // Add parameters to the command for the movie data
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@Description", movie.Description);
                    command.Parameters.AddWithValue("@Duration", movie.Duration);
                    command.Parameters.AddWithValue("@Year", movie.Year);
                    command.Parameters.AddWithValue("@Genres", string.Join(",", movie.Genres));
                    command.Parameters.AddWithValue("@Cast", string.Join(",", movie.Cast));
                    command.Parameters.AddWithValue("@Directors", string.Join(",", movie.Directors));
                    command.Parameters.AddWithValue("@IsRated16Plus", movie.IsRated16Plus ? 1 : 0);

                    // Open the database connection
                    OpenConnection();

                    // Execute the command to insert the movie into the database
                    command.ExecuteNonQuery();

                    // Close the database connection
                    CloseConnection();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding movie {movie.Title}: {ex.Message}");
        }
    }

    // Method to add movies from JSON files in the movie folder
    public static void AddMoviesFromFolder()
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

    // Method to initialize the database by creating tables and adding movies
    public static void InitializeDatabase()
    {
        try
        {
            // Open the connection to the database
            OpenConnection();

            // Create necessary tables in the database
            CreateTables();

            // Add movies from the specified folder
            AddMoviesFromFolder();
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
