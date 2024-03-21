using System;
using System.Collections.Generic;
using System.Data;
using Sharprompt;

public static class ChoiceMethods
{
    public static List<Movie> GetMovies()
    {
        string query = "SELECT * FROM movies";
        var movieTables = DatabaseHelper.ExecuteQuery(query);
        List<Movie> allMovies = new List<Movie>();

        foreach (DataRow item in movieTables.Rows)
        {
            Movie movie = new Movie();
            movie.Title = item["Title"].ToString();
            movie.Description = item["Description"].ToString();
            movie.Duration = Convert.ToInt32(item["Duration"]);
            movie.Year = Convert.ToInt32(item["Year"]);

            string[] genresArray = item["Genres"].ToString().Split(',');
            movie.Genres = new List<string>(genresArray);

            string[] castArray = item["Cast"].ToString().Split(',');
            movie.Cast = new List<string>(castArray);

            string[] directorsArray = item["Directors"].ToString().Split(',');
            movie.Directors = new List<string>(directorsArray);

            int isRated16Plus = Convert.ToInt32(item["IsRated16Plus"]);
            movie.IsRated16Plus = isRated16Plus == 1 ? true : false;

            allMovies.Add(movie);
        }
        return allMovies;
    }

    public static void ListMovies()
    {
        var movies = GetMovies();
        var filteredMovies = movies;

        var applyFilters = Prompt.Confirm("Would you like to apply filters before browsing movies?");
        if (applyFilters)
        {
            var filterOption = Prompt.Select<FilterOption>("Select an option to filter movies");

            if (filterOption == FilterOption.Genres)
            {
                var availableGenres = new List<string> { "All" }.Concat(movies.SelectMany(movie => movie.Genres).Distinct()).ToList();
                var selectedGenres = Prompt.MultiSelect("Select genres to filter movies", availableGenres);

                if (!selectedGenres.Contains("All"))
                {
                    filteredMovies = movies.Where(movie => movie.Genres.Any(selectedGenres.Contains)).ToList();
                }
            }
            else if (filterOption == FilterOption.Cast)
            {
                var availableActors = new List<string> { "All" }.Concat(movies.SelectMany(movie => movie.Cast).Distinct()).ToList();
                var selectedActors = Prompt.MultiSelect("Select actors to filter movies", availableActors);

                if (!selectedActors.Contains("All"))
                {
                    filteredMovies = movies.Where(movie => movie.Cast.Any(selectedActors.Contains)).ToList();
                }
            }
            else if (filterOption == FilterOption.Directors) // Corrected placement of this condition
            {
                var availableDirectors = new List<string> { "All" }.Concat(movies.SelectMany(movie => movie.Directors).Distinct()).ToList();
                var selectedDirectors = Prompt.MultiSelect("Select directors to filter movies", availableDirectors);

                if (!selectedDirectors.Contains("All"))
                {
                    filteredMovies = movies.Where(movie => movie.Directors.Any(selectedDirectors.Contains)).ToList();
                }
            }

            List<string> filteredMovieTitles = filteredMovies.Select(movie => movie.Title).ToList();

            if (filteredMovieTitles.Count == 0)
            {
                Console.WriteLine("No movies found based on the selected filters.");
                return;
            }

            var selectedMovieTitle = Prompt.Select("Select a movie", filteredMovieTitles);
            var selectedMovie = filteredMovies.Find(movie => movie.Title == selectedMovieTitle);

            Console.WriteLine($"Title: {selectedMovie.Title}");
            Console.WriteLine($"Description: {selectedMovie.Description}");
            Console.WriteLine($"Duration: {selectedMovie.Duration} minutes");
            Console.WriteLine($"Year: {selectedMovie.Year}");
            Console.WriteLine($"Genres: {string.Join(", ", selectedMovie.Genres)}");
            Console.WriteLine($"Cast: {string.Join(", ", selectedMovie.Cast)}");
            Console.WriteLine($"Director(s): {string.Join(", ", selectedMovie.Directors)}");
            Console.WriteLine($"Age: {(selectedMovie.IsRated16Plus ? "16+" : "G-rated")}");
        }
    }

}