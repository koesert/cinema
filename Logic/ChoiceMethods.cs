using System;
using System.Collections.Generic;
using System.Data;
using Sharprompt;

public class ChoiceMethods
{
    public List<Movie> GetMovies()
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        string query = "SELECT * FROM movies";
        var movieTables = databaseHelper.ExecuteQuery(query);
        List<Movie> allMovies = new List<Movie>();

        foreach (DataRow item in movieTables.Rows)
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
        return allMovies;
    }

    public void ListMovies()
    {
        var movies = GetMovies();
        List<string> movieTitles = new List<string>();
        foreach (var movie in movies)
        {
            movieTitles.Add(movie.Title);
        }

        var selectedGenre = Prompt.Select(
            "Select a genre to filter movies",
            new List<string> { "All" }
                .Concat(movies.SelectMany(movie => movie.Genres).Distinct())
                .ToList()
        );
        var filteredMovies =
            selectedGenre == "All"
                ? movies
                : movies.Where(movie => movie.Genres.Contains(selectedGenre)).ToList();

        List<string> filteredMovieTitles = filteredMovies.Select(movie => movie.Title).ToList();

        var selectedMovieTitle = Prompt.Select("Select a movie", filteredMovieTitles);
        var selectedMovie = filteredMovies.Find(movie => movie.Title == selectedMovieTitle);

        Console.WriteLine($"Title: {selectedMovie.Title}");
        Console.WriteLine($"Description: {selectedMovie.Description}");
        Console.WriteLine($"Duration: {selectedMovie.Duration} minutes");
        Console.WriteLine($"Year: {selectedMovie.Year}");
        Console.WriteLine($"Genres: {string.Join(", ", selectedMovie.Genres)}");
        Console.WriteLine($"Cast: {string.Join(", ", selectedMovie.Cast)}");
    }
}
