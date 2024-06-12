using Cinema.Data;
using Newtonsoft.Json;
using System.Globalization;

namespace Cinema.Services
{
  public class MovieDataLoader
  {
    public List<Movie> LoadMoviesFromJson(string filePath)
    {
      List<Movie> movies = new List<Movie>();
      if (File.Exists(filePath))
      {
        string json = File.ReadAllText(filePath);
        var jsonMovies = JsonConvert.DeserializeObject<List<JsonMovie>>(json);

        movies = jsonMovies.Select(jsonMovie =>
        {
          DateTimeOffset releaseDate;
          if (!DateTimeOffset.TryParseExact(
                jsonMovie.ReleaseDate,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out releaseDate
                ))
          {
            releaseDate = DateTimeOffset.MinValue;
          }

          return new Movie
          {
            Title = jsonMovie.Title,
            Duration = jsonMovie.Duration,
            Description = jsonMovie.Description,
            ReleaseDate = releaseDate,
            MinAgeRating = jsonMovie.MinAgeRating,
            Genres = jsonMovie.Genres,
            Cast = jsonMovie.Cast,
            Directors = jsonMovie.Directors
          };
        }).ToList();
      }
      return movies;
    }

    public void AddMoviesToDatabase(List<Movie> movies, CinemaContext db)
    {
      foreach (var movie in movies)
      {
        if (!db.Movies.Any(x => x.Title == movie.Title))
        {
          db.Movies.Add(movie);
        }
      }
      db.SaveChanges();
    }
  }
}