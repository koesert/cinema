using System;
using System.Collections.Generic;
using System.Data;
using Sharprompt;

class Program
{
    static void Main(string[] args)
    {
        // Create a new instance of the DatabaseHelper class
        DatabaseHelper databaseHelper = new DatabaseHelper();

        // Initialize database with tables and movies from specified folder
        databaseHelper.InitializeDatabase();
        string a = "SELECT * FROM movies";
        var oki = databaseHelper.ExecuteQuery(a);
        List<Movie> allMovies = new List<Movie>();

        foreach (DataRow item in oki.Rows)
        {
            Movie movie = new Movie();
            // movieTitles.Add(item["Title"].ToString());
            movie.Title = item["Title"].ToString();
            movie.Description = item["Description"].ToString();
            movie.Duration = Convert.ToInt32(item["Duration"]);
            movie.Year = Convert.ToInt32(item["Year"]);

            string[] genresArray = item["Genres"].ToString().Split(',');
            movie.Genres = new List<string>(genresArray);

            string[] castArray = item["Cast"].ToString().Split(',');
            movie.Cast = new List<string>(castArray);
            allMovies.Add(movie);
        }

        Choice currentChoice = Choice.Login;

        while (currentChoice != Choice.Login)
        {
            currentChoice = Prompt.Select<Choice>("What would you like to do?");

            switch (currentChoice)
            {
                case Choice.ListMovies:
                    break;
            }
        }
        var selectedMovieTitle = Prompt.Select(
            "Select a movie:",
            allMovies,
            textSelector: selectedMovieTitle => selectedMovieTitle.Title
        );
        System.Console.WriteLine();
        Console.WriteLine($"Title: {selectedMovieTitle.Title}");
        Console.WriteLine($"Description: {selectedMovieTitle.Description}");
        Console.WriteLine($"Duration: {selectedMovieTitle.Duration} minutes");
        Console.WriteLine($"Year: {selectedMovieTitle.Year}");
        Console.WriteLine($"Genres: {string.Join(", ", selectedMovieTitle.Genres)}");
        Console.WriteLine($"Cast: {string.Join(", ", selectedMovieTitle.Cast)}");
        Console.ReadLine();
        // var selectedMovie = GetMovieByTitle(selectedMovieTitle, oki);
    }
}
