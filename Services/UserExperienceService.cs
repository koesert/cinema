using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

public class UserExperienceService
{

  private static readonly Dictionary<UserExperienceChoice, string> UserExperienceChoiceDescriptions = new Dictionary<UserExperienceChoice, string>
  {
      { UserExperienceChoice.ListMovies, "Blader door films & vertoningen" },
      { UserExperienceChoice.ViewTickets, "Bekijk uw tickets" },
      { UserExperienceChoice.Logout, "Uitloggen" }
  };

  [Obsolete]
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
          ListMoviesWithShowtimes(customer, db);
          break;
        case UserExperienceChoice.ViewTickets:
          // Implement logic to view user's tickets
          ViewTickets(db, customer);
          break;
        default:
          break;
      }
    }
  }

  [Obsolete]
  public void ListMoviesWithShowtimes(Customer loggedInCustomer, CinemaContext db)
  {
    Console.Clear();

    var moviesQuery = db.Movies.Include(m => m.Showtimes);
    DateTime today = DateTime.UtcNow.Date;
    int currentWeek = 0;
    string filterDescription = "";

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
      AnsiConsole.MarkupLine(filterDescription); // Display active filters
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
        if (loggedInCustomer != null)
        {
          PresentCustomerOptions.Start(loggedInCustomer, db);
          break;
        }
        else
        {
          Console.Clear();
          break;
        }
      }
      else if (selectedOption == "Filter door films")
      {
        var result = ApplyFilters(db);
        moviesQuery = result.Item1.Include(m => m.Showtimes);
        filterDescription = result.Item2;
        Console.Clear();
        continue;
      }

      var selectedMovie = moviesWithUpcomingShowtimes.First(m => m.Title == selectedOption);

      Console.Clear();
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
        ShowCinemaHall(loggedInCustomer, db, selectedShowtime);
      }
    }
  }

  [Obsolete]
  public void ShowCinemaHall(Customer loggedInCustomer, CinemaContext db, Showtime showtime)
  {
    Console.CursorVisible = false;
    int currentRow = 0;
    int currentSeatNumber = 0;
    int layoutLinesUsed = CalculateLinesUsedForLayout(db, showtime);
    int selectedSeatLine = layoutLinesUsed + 1;
    string ticketNumber = LogicLayerVoucher.GenerateRandomCode(db);
    List<CinemaSeat> selectedSeats = new List<CinemaSeat>();

    var firstAvailableSeat = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id && s.SeatNumber != 0)
        .OrderBy(s => s.Row)
        .ThenBy(s => s.SeatNumber)
        .FirstOrDefault();

    if (firstAvailableSeat != null)
    {
      currentRow = firstAvailableSeat.Row - 'A';
      currentSeatNumber = firstAvailableSeat.SeatNumber - 1;
    }

    CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

    while (true)
    {
      Console.SetCursorPosition(0, selectedSeatLine);
      var selectedSeatPrice = db.CinemaSeats
          .FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber == currentSeatNumber + 1)?.Price ?? 0;

      Console.WriteLine($"Selected Seat Price: ${selectedSeatPrice}");
      Console.WriteLine($"Selected Seat: {(char)('A' + currentRow)}{(currentSeatNumber + 1).ToString().PadLeft(2, '0')}");

      CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

      ConsoleKeyInfo keyInfo = Console.ReadKey(true);
      if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.W)
      {
        if (currentRow > 0)
        {
          var seatAbove = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow - 1) && s.SeatNumber == currentSeatNumber + 1);
          if (seatAbove != null)
          {
            currentRow--;
          }
        }
      }
      else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.S)
      {
        var seatBelow = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow + 1) && s.SeatNumber == currentSeatNumber + 1);
        if (seatBelow != null)
        {
          currentRow++;
        }
      }
      else if (keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.A)
      {
        var minSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow))
            .Where(s => s.SeatNumber != 0)
            .Min(s => (int?)s.SeatNumber);

        currentSeatNumber = Math.Max(minSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber - 1);
      }
      else if (keyInfo.Key == ConsoleKey.RightArrow || keyInfo.Key == ConsoleKey.D)
      {
        var maxSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber != 99)
            .Max(s => (int?)s.SeatNumber);

        currentSeatNumber = Math.Min(maxSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber + 1);
      }
      else if (keyInfo.Key == ConsoleKey.Enter)
      {
        char selectedRow = (char)('A' + currentRow);
        int selectedSeatNumber = currentSeatNumber + 1;

        var selectedSeat = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == selectedRow && s.SeatNumber == selectedSeatNumber);

        if (selectedSeat != null && !selectedSeat.IsReserved)
        {
          if (selectedSeats.Contains(selectedSeat))
          {
            selectedSeats.Remove(selectedSeat);
            selectedSeat.IsSelected = false; // Unselect the seat
          }
          else
          {
            selectedSeats.Add(selectedSeat);
            selectedSeat.IsSelected = true; // Select the seat
          }
        }
      }
      else if (keyInfo.Key == ConsoleKey.Spacebar)
      {
        // Call a method to handle reservation
        HandleReservation(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
        break;
      }
      else if (keyInfo.Key == ConsoleKey.Escape)
      {
        Console.Clear();
        break;
      }
    }
    Console.CursorVisible = true;
  }

  [Obsolete]
  private void HandleReservation(Customer loggedInCustomer, CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber)
  {
    if (selectedSeats.Count == 0)
    {
      AnsiConsole.MarkupLine("[red]No seats selected.[/]");
      ShowCinemaHall(loggedInCustomer, db, showtime, selectedSeats);
      return;
    }

    Console.Clear();
    Console.WriteLine("Selected Seats:");
    var table = new Table();
    table.Border = TableBorder.Rounded;
    table.AddColumn("Row");
    table.AddColumn("Seat Number");
    table.AddColumn("Price");

    decimal totalSeatPrice = 0;
    foreach (var seat in selectedSeats)
    {
      table.AddRow(seat.Row.ToString(), seat.SeatNumber.ToString(), $"${seat.Price}");
      totalSeatPrice += seat.Price;
    }
    AnsiConsole.Render(table);

    Console.WriteLine($"Total Price: ${totalSeatPrice}");

    var reservationKeyInfo = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .PageSize(3)
        .AddChoices("Yes", "No")
        .Title("Do you want to reserve these seats?")
        .HighlightStyle(new Style(Color.Blue)));

    if (reservationKeyInfo == "Yes")
    {

      foreach (var seat in selectedSeats)
      {
        seat.IsReserved = true;
      }

      ReserveSeats(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
      AnsiConsole.MarkupLine("[green]Seats successfully reserved.[/]");
      Console.WriteLine("Press any key to return to start.");
      Console.ReadKey(true);
      Console.Clear();
    }
    else
    {
      ShowCinemaHall(loggedInCustomer, db, showtime, selectedSeats);
    }
  }

  private void ReserveSeats(Customer loggedInCustomer, CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber)
  {
    if (loggedInCustomer != null)
    {
      CreateTicket(db, loggedInCustomer, showtime, selectedSeats, ticketNumber, loggedInCustomer.Email);
    }
    else
    {
      string userEmail = null;
      bool emailIsValid = false;

      while (!emailIsValid)
      {
        Console.Write("Voer uw e-mailadres in om verder te gaan met de reservering: ");
        userEmail = Console.ReadLine();

        if (!RegisterValidity.CheckEmail(userEmail))
        {
          AnsiConsole.Markup("[red]Voer alstublieft een geldig e-mailadres in.[/]");
          Console.WriteLine("");
          continue;
        }

        if (db.Customers.Any(c => c.Email.ToLower() == userEmail.ToLower()))
        {
          AnsiConsole.Markup("[red]Dit e-mailadres is al in gebruik. Gebruik een ander e-mailadres.[/]");
          Console.WriteLine("");
        }
        else
        {
          emailIsValid = true;
        }
      }

      CreateTicket(db, showtime, selectedSeats, ticketNumber, userEmail);
    }
    db.SaveChanges();
  }


  private void CreateTicket(CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber, string userEmail)
  {
    var ticket = new Ticket
    {
      Showtime = showtime,
      PurchasedAt = DateTime.UtcNow.AddHours(2),
      TicketNumber = ticketNumber,
      CustomerEmail = userEmail,
      Seats = selectedSeats.ToList(),
      PurchaseTotal = selectedSeats.Sum(s => s.Price)
    };
    db.Tickets.Add(ticket);
  }

  private void CreateTicket(CinemaContext db, Customer loggedInCustomer, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber, string userEmail)
  {
    var ticket = new Ticket
    {
      Customer = loggedInCustomer,
      Showtime = showtime,
      PurchasedAt = DateTime.UtcNow.AddHours(2),
      TicketNumber = ticketNumber,
      CustomerEmail = userEmail,
      Seats = selectedSeats.ToList(),
      PurchaseTotal = selectedSeats.Sum(s => s.Price)
    };
    db.Tickets.Add(ticket);
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


  private (IQueryable<Movie>, string) ApplyFilters(CinemaContext db)
  {
    var allMovies = db.Movies.ToList();
    var moviesQuery = allMovies.AsQueryable();
    string activeFilters = "Active Filters: ";

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
                                .Distinct()
                                .ToList();
        var selectedGenres = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer genres om films te filteren")
                .PageSize(10)
                .AddChoices(allGenres)
        );
        moviesQuery = allMovies.Where(movie => movie.Genres != null &&
            movie.Genres.Intersect(selectedGenres).Any())
            .AsQueryable();
        activeFilters += "Genres: " + string.Join(", ", selectedGenres);
        break;

      case CinemaFilterChoice.Acteurs:
        var allActors = allMovies.SelectMany(movie => movie.Cast ?? Enumerable.Empty<string>())
                                .Distinct()
                                .ToList();
        var selectedActors = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer acteurs om films te filteren")
                .PageSize(10)
                .AddChoices(allActors)
        );
        moviesQuery = allMovies.Where(movie => movie.Cast != null &&
            movie.Cast.Intersect(selectedActors).Any())
            .AsQueryable();
        activeFilters += "Acteurs: " + string.Join(", ", selectedActors);
        break;

      case CinemaFilterChoice.Directeuren:
        var allDirectors = allMovies.SelectMany(movie => movie.Directors ?? Enumerable.Empty<string>())
                                    .Distinct()
                                    .ToList();
        var selectedDirectors = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Selecteer regisseurs om films te filteren")
                .PageSize(10)
                .AddChoices(allDirectors)
        );
        moviesQuery = allMovies.Where(movie => movie.Directors != null &&
            movie.Directors.Intersect(selectedDirectors).Any())
            .AsQueryable();
        activeFilters += "Directeuren: " + string.Join(", ", selectedDirectors);
        break;

      default:
        return (null, null);
    }

    return (moviesQuery, activeFilters);
  }


  [Obsolete]
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

  private void ViewTickets(CinemaContext db, Customer customer)
  {
    Console.Clear();
    var tickets = db.Tickets
        .Include(t => t.Showtime)
        .Where(t => t.Customer.Id == customer.Id)
        .ToList();

    if (tickets.Any())
    {
      foreach (var ticket in tickets)
      {
        // Display ticket information
        Console.WriteLine($"Ticket Number: {ticket.TicketNumber}");
        Console.WriteLine($"Showtime: {ticket.Showtime.StartTime}");
        Console.WriteLine($"Purchased At: {ticket.PurchasedAt}");
        Console.WriteLine($"Total Price: {ticket.PurchaseTotal}");
        Console.WriteLine();
      }
    }
    else
    {
      Console.WriteLine("You have no tickets.");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
  }

  public void ShowCinemaHall(Customer loggedInCustomer, CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats)
  {
    Console.CursorVisible = false;
    int currentRow = 0;
    int currentSeatNumber = 0;
    int layoutLinesUsed = CalculateLinesUsedForLayout(db, showtime);
    int selectedSeatLine = layoutLinesUsed + 1;
    string ticketNumber = LogicLayerVoucher.GenerateRandomCode(db);
    // List<CinemaSeat> selectedSeats = new List<CinemaSeat>();

    var firstAvailableSeat = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id && s.SeatNumber != 0)
        .OrderBy(s => s.Row)
        .ThenBy(s => s.SeatNumber)
        .FirstOrDefault();

    if (firstAvailableSeat != null)
    {
      currentRow = firstAvailableSeat.Row - 'A';
      currentSeatNumber = firstAvailableSeat.SeatNumber - 1;
    }

    CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

    while (true)
    {
      Console.SetCursorPosition(0, selectedSeatLine);
      var selectedSeatPrice = db.CinemaSeats
          .FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber == currentSeatNumber + 1)?.Price ?? 0;

      Console.WriteLine($"Selected Seat Price: ${selectedSeatPrice}");
      Console.WriteLine($"Selected Seat: {(char)('A' + currentRow)}{(currentSeatNumber + 1).ToString().PadLeft(2, '0')}");

      CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);

      ConsoleKeyInfo keyInfo = Console.ReadKey(true);
      if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.W)
      {
        if (currentRow > 0)
        {
          var seatAbove = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow - 1) && s.SeatNumber == currentSeatNumber + 1);
          if (seatAbove != null)
          {
            currentRow--;
          }
        }
      }
      else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.S)
      {
        var seatBelow = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow + 1) && s.SeatNumber == currentSeatNumber + 1);
        if (seatBelow != null)
        {
          currentRow++;
        }
      }
      else if (keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.A)
      {
        var minSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow))
            .Where(s => s.SeatNumber != 0)
            .Min(s => (int?)s.SeatNumber);

        currentSeatNumber = Math.Max(minSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber - 1);
      }
      else if (keyInfo.Key == ConsoleKey.RightArrow || keyInfo.Key == ConsoleKey.D)
      {
        var maxSeatNumberForRow = db.CinemaSeats
            .Where(s => s.Showtime.Id == showtime.Id && s.Row == (char)('A' + currentRow) && s.SeatNumber != 99)
            .Max(s => (int?)s.SeatNumber);

        currentSeatNumber = Math.Min(maxSeatNumberForRow.GetValueOrDefault(1) - 1, currentSeatNumber + 1);
      }
      else if (keyInfo.Key == ConsoleKey.Enter)
      {
        char selectedRow = (char)('A' + currentRow);
        int selectedSeatNumber = currentSeatNumber + 1;

        var selectedSeat = db.CinemaSeats.FirstOrDefault(s => s.Showtime.Id == showtime.Id && s.Row == selectedRow && s.SeatNumber == selectedSeatNumber);

        if (selectedSeat != null && !selectedSeat.IsReserved)
        {
          if (selectedSeats.Contains(selectedSeat))
          {
            selectedSeats.Remove(selectedSeat);
            selectedSeat.IsSelected = false; // Unselect the seat
          }
          else
          {
            selectedSeats.Add(selectedSeat);
            selectedSeat.IsSelected = true; // Select the seat
          }
        }
      }
      else if (keyInfo.Key == ConsoleKey.Spacebar)
      {
        // Call a method to handle reservation
        HandleReservation(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
        break;
      }
      else if (keyInfo.Key == ConsoleKey.Escape)
      {
        Console.Clear();
        break;
      }
    }
    Console.CursorVisible = true;
  }

}
