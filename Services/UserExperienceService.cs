using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

public class UserExperienceService : ManagementExperienceService
{

  private static readonly Dictionary<UserExperienceChoice, string> UserExperienceChoiceDescriptions = new Dictionary<UserExperienceChoice, string>
  {
      { UserExperienceChoice.ListMovies, "Blader door films & vertoningen" },
      { UserExperienceChoice.ViewTickets, "Bekijk uw tickets" },
      { UserExperienceChoice.Logout, "Uitloggen" }
  };
  public void ManageUser(Customer customer, CinemaContext db, IConfiguration configuration)
  {
    // Initialize the current user choice
    UserExperienceChoice currentUserChoice = UserExperienceChoice.ListMovies;

    // Loop until the user chooses to logout
    while (currentUserChoice != UserExperienceChoice.Logout)
    {
      Console.Clear();

      // Prompt the user for their choice
      var choice = AnsiConsole.Prompt(
          new SelectionPrompt<string>()
              .Title($"Welcome {customer.Username}! What would you like to do?")
              .PageSize(10)
              .AddChoices(UserExperienceChoiceDescriptions.Select(kv => kv.Value))
      );

      // Retrieve the enum value based on the selected description
      currentUserChoice = UserExperienceChoiceDescriptions.FirstOrDefault(kv => kv.Value == choice).Key;

      // Execute the corresponding action based on the user's choice
      switch (currentUserChoice)
      {
        case UserExperienceChoice.ListMovies:
          // Implement logic to list movies for the user
          ListMoviesWithShowtimes(db);
          break;
        case UserExperienceChoice.ViewTickets:
          // Implement logic to view user's tickets
          ViewTickets(db);
          break;
        default:
          break;
      }
    }
  }

  public void ListMoviesWithShowtimes(CinemaContext db)
  {
    Console.Clear();

    var moviesQuery = db.Movies.Include(m => m.Showtimes);
    DateTime today = DateTime.UtcNow.Date;
    int currentWeek = 0;

    while (true)
    {
      DateTime startOfWeek = today.AddDays(7 * currentWeek);
      DateTime endOfWeek = startOfWeek.AddDays(8).Date;

      var moviesWithUpcomingShowtimes = moviesQuery
          .Where(m => m.Showtimes != null && m.Showtimes
          .Any(s => s.StartTime >= DateTime.UtcNow && s.StartTime >= startOfWeek && s.StartTime < endOfWeek && s.StartTime >= DateTime.UtcNow + TimeSpan.FromHours(2)))
          .ToList();

      var options = new List<string> { "Filter door films" };

      options.AddRange(moviesWithUpcomingShowtimes.Select(m => m.Title));
      AnsiConsole.MarkupLine("");
      if (currentWeek < 3) options.Add("Volgende week");
      if (currentWeek > 0) options.Add("Vorige week");
      options.Add("Terug");

      var selectedOption = AnsiConsole.Prompt(
          new SelectionPrompt<string>()
              .Title($"Week van {startOfWeek:dd-MM} tot {endOfWeek.AddDays(-1):dd-MM}")
              .AddChoices(options)
              .PageSize(10)
      );

      if (selectedOption == "Volgende week")
      {
        currentWeek++;
        Console.Clear();
        continue;
      }
      else if (selectedOption == "Vorige week")
      {
        currentWeek--;
        Console.Clear();
        continue;
      }
      else if (selectedOption == "Terug")
      {
        break;
      }
      else if (selectedOption == "Filter door films")
      {
        var filteredMovies = ApplyFilters(db).Include(m => m.Showtimes);
        moviesQuery = filteredMovies;
        Console.Clear();
        continue;
      }

      var selectedMovie = moviesWithUpcomingShowtimes.First(m => m.Title == selectedOption);

      DisplayMovieDetails(selectedMovie);
      AnsiConsole.MarkupLine("");

      var showtimesThisWeek = selectedMovie.Showtimes
          .Where(s => s.StartTime >= DateTime.UtcNow && s.StartTime >= startOfWeek && s.StartTime < endOfWeek && s.StartTime >= DateTime.UtcNow + TimeSpan.FromHours(2))
          .OrderBy(s => s.StartTime)
          .ToList();

      if (showtimesThisWeek.Any())
      {
        var selectedShowtime = AnsiConsole.Prompt(
            new SelectionPrompt<Showtime>()
                .Title("Selecteer een voorstellingstijd")
                .AddChoices(showtimesThisWeek)
                .UseConverter(showtime => showtime.StartTime.ToString("ddd, MMMM d hh:mm tt"))
                .PageSize(10)
        );

        if (selectedMovie.MinAgeRating >= 16)
        {
          AnsiConsole.MarkupLine("[red]Let op: Deze film heeft een minimum leeftijd van 16 jaar of ouder.[/]");
          AnsiConsole.MarkupLine("Wil je doorgaan? (Ja/Nee)");

          var confirmation = AnsiConsole.Prompt(
              new SelectionPrompt<string>()
                  .AddChoices("Ja", "Nee")
          );

          if (confirmation.ToLower() != "ja")
          {
            Console.Clear();
            continue;
          }
        }
        Console.Clear();
        ShowCinemaHall(db, selectedShowtime);
      }
    }
  }

  public void ShowCinemaHall(CinemaContext db, Showtime showtime)
  {

    Console.CursorVisible = false;
    // Console.Clear();
    int currentRow = 0;
    int currentSeatNumber = 0;
    int layoutLinesUsed = CalculateLinesUsedForLayout(db, showtime);
    int selectedSeatLine = layoutLinesUsed + 1; // Adjust this based on your layout

    // Find the first available seat to start highlighting
    var firstAvailableSeat = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id && s.SeatNumber != 0) // Exclude empty spaces
        .OrderBy(s => s.Row)
        .ThenBy(s => s.SeatNumber)
        .FirstOrDefault();

    if (firstAvailableSeat != null)
    {
      currentRow = firstAvailableSeat.Row - 'A'; // Convert char to index (e.g., 'A' -> 0)
      currentSeatNumber = firstAvailableSeat.SeatNumber - 1; // Convert seat number to index (e.g., 1 -> 0)
    }

    // Draw the cinema hall layout initially
    CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

    while (true)
    {

      Console.SetCursorPosition(0, selectedSeatLine);
      // Fetch the price of the currently selected seat from the database
      var selectedSeatPrice = db.CinemaSeats
          .FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber == currentSeatNumber + 1)?.Price ?? 0;

      // Display the price of the currently selected seat
      Console.WriteLine($"Selected Seat Price: ${selectedSeatPrice}");
      Console.WriteLine($"Selected Seat: {(char)('A' + currentRow)}{(currentSeatNumber + 1).ToString().PadLeft(2, '0')}");


      // Draw the cinema hall layout with the new highlight
      CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

      // Listen for arrow key inputs and process user actions
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);
      if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.W)
      {
        // Check if moving up is possible
        if (currentRow > 0)
        {
          // Check if there is a seat in the row above at the same seat number
          var seatAbove = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow - 1) && s.SeatNumber == currentSeatNumber + 1);
          if (seatAbove != null)
          {
            currentRow--;
          }
        }
      }
      else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.S)
      {
        // Check if moving down is possible
        var seatBelow = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow + 1) && s.SeatNumber == currentSeatNumber + 1);
        if (seatBelow != null)
        {
          currentRow++;
        }
      }
      else if (keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.A)
      {
        // Fetch the minimum seat number for the current row from the database
        var minSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow))
            .Where(s => s.SeatNumber != 0) // Exclude seat numbers that are 0 (empty spaces)
            .Min(s => (int?)s.SeatNumber); // Use (int?) to handle null case safely

        // Adjust the seat number to move left, ensuring it's within bounds
        currentSeatNumber = Math.Max(minSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber - 1);
      }
      else if (keyInfo.Key == ConsoleKey.RightArrow || keyInfo.Key == ConsoleKey.D)
      {
        // Fetch the maximum seat number for the current row from the database
        var maxSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber != 99)
            .Max(s => (int?)s.SeatNumber); // Use (int?) to handle null case safely

        // Adjust the seat number to move right, ensuring it's within bounds
        currentSeatNumber = Math.Min(maxSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber + 1);
      }
      else if (keyInfo.Key == ConsoleKey.Enter)
      {
        // Seat selection logic
        char selectedRow = (char)('A' + currentRow);
        int selectedSeatNumber = currentSeatNumber + 1;

        CinemaSeat selectedSeat = CinemaReservationSystem.FindSeat(selectedRow, selectedSeatNumber, showtime, db);
        if (selectedSeat == null)
        {
          AnsiConsole.MarkupLine($"[red]Seat {selectedRow}{selectedSeatNumber} does not exist[/]");
        }
        else if (selectedSeat.IsReserved)
        {
          AnsiConsole.MarkupLine($"[red]Seat {selectedRow}{selectedSeatNumber} is already reserved.[/]");
        }
        else
        {
          // Update the seat reservation status in the database
          selectedSeat.IsReserved = true;
          db.SaveChanges();

          // Redraw the cinema hall layout after successful reservation
          Console.Clear();
          // Display "Successfully reserved seat" message in green
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine("Successfully reserved seat");
          Console.ResetColor(); // Reset color back to default
          ShowCinemaHall(db, showtime);

          // Exit loop to prevent further input handling after reservation
          break;
        }
      }
      else if (keyInfo.Key == ConsoleKey.Escape)
      {
        // Exit loop if user presses escape key
        break;
      }
    }
    Console.CursorVisible = true;
  }
  private int CalculateLinesUsedForLayout(CinemaContext db, Showtime showtime)
  {
    int linesUsed = 0;

    // Calculate the number of rows in the cinema hall layout
    var showtimes = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id)
        .OrderBy(s => s.Row)
        .ThenBy(s => s.SeatNumber)
        .ToList();

    char previousRowChar = '\0';
    foreach (var seat in showtimes)
    {
      if (seat.Row != previousRowChar)
      {
        linesUsed++;
        previousRowChar = seat.Row;
      }
    }

    // Add additional lines for legend and labels
    linesUsed += 5; // Adjust this value based on the actual number of lines used

    return linesUsed;
  }


  private IQueryable<Movie> ApplyFilters(CinemaContext db)
  {
    var allMovies = db.Movies.ToList();
    var moviesQuery = allMovies.AsQueryable();

    var filterOption = AnsiConsole.Prompt(
    new SelectionPrompt<CinemaFilterChoice>()
        .Title("Selecteer een optie om films te filteren")
        .PageSize(4)
        .AddChoices(new[]
        {
                    CinemaFilterChoice.Genres,
                    CinemaFilterChoice.Directeuren,
                    CinemaFilterChoice.Acteurs,
                    CinemaFilterChoice.Terug
        })
);

    switch (filterOption)
    {
      case CinemaFilterChoice.Genres:
        var allGenres = allMovies.SelectMany(movie => movie.Genres ?? Enumerable.Empty<string>())
                                .Where(genre => genre != null)
                                .Distinct()
                                .ToList();
        var selectedGenres = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer genres om films te filteren")
                .PageSize(10)
                .AddChoices(allGenres)
        );
        // Filter films op basis van geselecteerde genres
        moviesQuery = allMovies.Where(movie => movie.Genres != null &&
            movie.Genres.Intersect(selectedGenres ?? Enumerable.Empty<string>()).Any())
            .AsQueryable();
        break;

      case CinemaFilterChoice.Acteurs:
        var allActors = allMovies.SelectMany(movie => movie.Cast ?? Enumerable.Empty<string>())
                                .Where(actor => actor != null)
                                .Distinct()
                                .ToList();
        var selectedActors = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer acteurs om films te filteren")
                .PageSize(10)
                .AddChoices(allActors)
        );
        // Filter films op basis van geselecteerde acteurs
        moviesQuery = allMovies.Where(movie => movie.Cast != null &&
            movie.Cast.Intersect(selectedActors ?? Enumerable.Empty<string>()).Any())
            .AsQueryable();
        break;

      case CinemaFilterChoice.Directeuren:
        var allDirectors = allMovies.SelectMany(movie => movie.Directors ?? Enumerable.Empty<string>())
                                    .Where(director => director != null)
                                    .Distinct()
                                    .ToList();
        var selectedDirectors = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer regisseurs om films te filteren")
                .PageSize(10)
                .AddChoices(allDirectors)
        );
        moviesQuery = allMovies.Where(movie => movie.Directors != null &&
            movie.Directors.Intersect(selectedDirectors ?? Enumerable.Empty<string>()).Any())
            .AsQueryable();
        break;

      default:
        break;
    }

    return moviesQuery;
  }
  private void DisplayMovieDetails(Movie movie)
  {
    var table = new Table();

    table.Border = TableBorder.Rounded;
    table.Title($"Details voor \"{movie.Title}\"");
    table.AddColumn(new TableColumn("Categorie").Centered());
    table.AddColumn(new TableColumn("Waarde"));

    table.AddRow("Genre", string.Join(", ", movie.Genres));
    table.AddRow("Duur", $"{movie.Duration} minuten");
    table.AddRow("Regisseurs", string.Join(", ", movie.Directors));
    table.AddRow("Beschrijving", movie.Description);
    table.AddRow("Minimum leeftijdsbeoordeling", movie.MinAgeRating.ToString());

    AnsiConsole.Render(table);
  }

  private void ViewTickets(CinemaContext db)
  {
    // Implement logic to view user's tickets
  }
}
