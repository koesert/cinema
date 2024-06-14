using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Cinema.Logic;
using System.Globalization;
using System;
using Cinema;
public class UserExperienceService
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
				.Title($"Welkom {customer.Username}! Wat wilt u doen?")
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

	public void ListMoviesWithShowtimes(Customer loggedInCustomer, CinemaContext db)
	{
		if (loggedInCustomer != null) PresentCustomerReservationProgress.UpdateTrueProgress(loggedInCustomer, db);
		PresentAdminOptions.UpdateVouchers(db);
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
			  .Any(s => s.StartTime >= DateTime.UtcNow && s.StartTime >= startOfWeek && s.StartTime < endOfWeek && s.StartTime >= DateTime.UtcNow + TimeSpan.FromHours(2) && s.CinemaSeats.Any(y => y.IsReserved == false && y.SeatNumber != 99 && y.SeatNumber != 0)))
			  .OrderBy(x => x.Title)
			  .ToList();

			var options = new List<string> { "Terug", "Filter door films" };

			options.AddRange(moviesWithUpcomingShowtimes.Select(m => m.Title));
			AnsiConsole.MarkupLine("");
			AnsiConsole.MarkupLine(filterDescription); // Display active filters
			if (currentWeek < 3) options.Add("Volgende week");
			if (currentWeek > 0) options.Add("Vorige week");

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
					Program.Main();
					break;
				}
			}
			else if (selectedOption == "Filter door films")
			{
				var result = ApplyFilters(db);
				if (!(result.Item1 == null))
				{
					moviesQuery = result.Item1.Include(m => m.Showtimes);
					filterDescription = result.Item2;
				}
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
				string stringselectedShowtime = AnsiConsole.Prompt(
				  new SelectionPrompt<string>()
					.Title("Selecteer een voorstellingstijd")
					.AddChoices(new List<string> { "Terug" }.Concat(db.Showtimes.Where(s => s.StartTime >= DateTime.UtcNow && s.StartTime >= startOfWeek && s.StartTime < endOfWeek && s.StartTime >= DateTime.UtcNow + TimeSpan.FromHours(2) && s.Movie == selectedMovie && s.CinemaSeats.Any(y => y.IsReserved == false && y.SeatNumber != 99 && y.SeatNumber != 0)).Select(x => $"{x}").ToList())
				));
				if (stringselectedShowtime == "Terug")
				{
					ListMoviesWithShowtimes(loggedInCustomer, db);
					return;
				}
				Showtime selectedShowtime = db.Showtimes.AsEnumerable().FirstOrDefault(x => x.Movie == selectedMovie && x.ToString() == stringselectedShowtime);
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
				PresentAdminOptions.ConfigureSeatPrices(db);
				ShowCinemaHall(loggedInCustomer, db, selectedShowtime);
			}
		}
	}

	public void ShowCinemaHall(Customer loggedInCustomer, CinemaContext db, Showtime showtime)
	{
		Console.CursorVisible = false;
		int currentRow = 0;
		int currentSeatNumber = 0;
		int layoutLinesUsed = CalculateLinesUsedForLayout(db, showtime);
		int selectedSeatLine = layoutLinesUsed + 1;
		string ticketNumber = Ticket.GenerateRandomCode(db);
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

			// Combine and space out the outputs with AnsiConsole
			CinemaSeat basepriceseat = db.CinemaSeats.First(x => x.Showtime == showtime && x.Color == "orange" && x.Type == 0);
			if (basepriceseat.Price != 25) Console.WriteLine();
			AnsiConsole.WriteLine();
			AnsiConsole.Markup($"Geselecteerde stoelprijs: {selectedSeatPrice} euro [grey]{new string(' ', 50)}(Druk op <Enter> om stoelen te selecteren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel
			AnsiConsole.Markup($"Geselecteerde Stoel: {(char)('A' + currentRow)}{(currentSeatNumber + 1).ToString().PadLeft(2, '0')} [grey]{new string(' ', 50)}      (Druk op <Space> om stoelen te reserveren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel
			AnsiConsole.Markup($"[grey]{new string(' ', 50)}                         (Druk op <Escape> om terug te keren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel

			CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);
			if (basepriceseat.Price != 25) Console.WriteLine();

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
				foreach (var seat in selectedSeats)
				{
					seat.IsSelected = false;
				}
				ListMoviesWithShowtimes(loggedInCustomer, db);
				Console.Clear();
				break;
			}
		}
		Console.CursorVisible = true;
	}

	private void HandleReservation(Customer loggedInCustomer, CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber)
	{
		Voucher v = null;
		if (selectedSeats.Count == 0)
		{
			AnsiConsole.MarkupLine("[red]Geen stoelen geselecteerd.[/]");
			ShowCinemaHall(loggedInCustomer, db, showtime, selectedSeats);
			return;
		}

		Console.Clear();
		Console.WriteLine("Geselecteerde Stoelen:");
		var table = new Table();
		table.Border = TableBorder.Rounded;
		table.AddColumn("Rij");
		table.AddColumn("Stoelnummer");
		table.AddColumn("Prijs");


		decimal totalSeatPrice = 0;
		foreach (var seat in selectedSeats)
		{
			table.AddRow(seat.Row.ToString(), seat.SeatNumber.ToString(), $"{seat.Price} Euro");
			totalSeatPrice += seat.Price;
		}
		AnsiConsole.Write(table);

		Console.WriteLine($"Totaal Prijs: {totalSeatPrice} Euro");
		if (!(loggedInCustomer is null))
		{
			v = UseVoucher(db, loggedInCustomer);
			if (!(v is null))
			{
				Console.Clear();
				AnsiConsole.Write(table);

				Console.WriteLine($"Oude Totaal Prijs: {totalSeatPrice} Euro");
				Console.WriteLine($"Voucher gebruikt: '{v.Code}' voor {v.Discount}{v.DiscountType} korting");
				Console.WriteLine($"Nieuwe Totaal Prijs: {v.ApplyDiscount((double)totalSeatPrice)} Euro");
			}
		}

		var reservationKeyInfo = AnsiConsole.Prompt(
		new SelectionPrompt<string>()
			.PageSize(3)
			.AddChoices("Ja", "Nee")
			.Title("Wilt u deze stoelen reserveren?")
			.HighlightStyle(new Style(Color.Blue)));


		if (reservationKeyInfo == "Ja")
		{

			foreach (var seat in selectedSeats)
			{
				seat.IsReserved = true;
			}

			if (v is null)
			{
				ReserveSeats(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
			}
			else
			{
				ReserveSeats(loggedInCustomer, db, showtime, selectedSeats, ticketNumber, v);
				v.ExpirationDate = DateTimeOffset.UtcNow.AddHours(1);
				db.SaveChanges();
			}
			if (loggedInCustomer != null) PresentCustomerReservationProgress.UpdateTrueProgress(loggedInCustomer, db);
			PresentAdminOptions.UpdateVouchers(db);
			Console.Clear();
			return;
		}
		else
		{
			Console.Clear();
			foreach (var seat in selectedSeats)
			{
				seat.IsReserved = false;
			}
			ShowCinemaHall(loggedInCustomer, db, showtime, selectedSeats);
			return;
		}
	}

	private void DisplayReservationConfirmation(CinemaContext db, Customer customer, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber)
	{
		AnsiConsole.Clear();
		var NL = new CultureInfo("nl-NL");
		string date = showtime.StartTime.ToString("dddd, d MMMM HH:mm", NL);
		var table = new Table().Border(TableBorder.Rounded);
		table.AddColumn(new TableColumn("Rij").Centered());
		table.AddColumn(new TableColumn("Stoelnummer").Centered());
		table.AddColumn(new TableColumn("Prijs").Centered());

		decimal totalSeatPrice = 0;
		foreach (var seat in selectedSeats)
		{
			table.AddRow(
				new Markup($"[green]{seat.Row}[/]"),
				new Markup($"[green]{seat.SeatNumber}[/]"),
				new Markup($"[green]{seat.Price} Euro[/]")
			);
			totalSeatPrice += seat.Price;
		}

		var panel = new Panel(new Rows(
			new Markup($"[bold yellow]Ticketnummer:[/] [white]{ticketNumber}[/]"),
			new Markup(""),
			new Markup("[bold yellow]Geselecteerde Stoelen:[/]"),
			table,
			new Markup(""),
			new Markup($"[bold yellow]Totaalprijs:[/] [white]{totalSeatPrice} Euro[/]"),
			new Markup(""),
			new Markup($"[bold yellow]Film:[/] [white]{showtime.Movie.Title}[/]"),
			new Markup($"[bold yellow]Tijdstip:[/] [white]{date}[/]"),
			new Markup($"[bold yellow]Zaal:[/] [white]{showtime.RoomId}[/]"),
			new Markup("[bold yellow]Locatie:[/] [white]Witte de Withstraat 20, 3067AX Rotterdam[/]"),
			new Markup("\n[bold aqua]Bevestiging van uw reservering bevind zich in uw email[/]"),
			new Markup("[bold aqua]Bedankt voor uw reservering bij Your Eyes![/]")
		))
		{
			Header = new PanelHeader("[bold blue] Reserveringsbevestiging [/]"),
			Border = BoxBorder.Rounded,
			Padding = new Padding(1, 1, 1, 1)
		};

		AnsiConsole.Write(panel);

		var choices = new[]
		{
			"Films bekijken",
			"Inloggen",
			"Terug"
		};

		var selectedChoice = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("[bold blue]Wat wilt u nu doen?[/]")
				.PageSize(4)
				.AddChoices(choices)
		);

		switch (selectedChoice)
		{
			case "Films bekijken":
				ListMoviesWithShowtimes(customer, db);
				break;
			case "Inloggen":
				PresentCustomerLogin.Start(db);
				break;
			case "Terug":
				return;
		}
	}


	private void ReserveSeats(Customer loggedInCustomer, CinemaContext db, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber, Voucher voucherused = null)
	{
		ConfirmationEmail sender = new ConfirmationEmail();
		bool reservationSuccesful = false;
		while (!reservationSuccesful)
		{
			if (loggedInCustomer != null)
			{
				reservationSuccesful = true;
				CreateTicket(db, loggedInCustomer, showtime, selectedSeats, ticketNumber, loggedInCustomer.Email, voucherused);
				sender.SendMessage(loggedInCustomer.Email, loggedInCustomer.Username, showtime.Movie.Title, showtime.StartTime.ToString("dd-MM-yyyy"), showtime.StartTime.ToString("HH:mm"), string.Join(", ", selectedSeats.Select(x => $"{x.Row}{x.SeatNumber}")), showtime.RoomId, ticketNumber);
				DisplayReservationConfirmation(db, loggedInCustomer, showtime, selectedSeats, ticketNumber);

			}
			else
			{
				var choice = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Wilt u [blue]reserveren[/] met een [green]bestaand[/] account?")
						.PageSize(5)
						.AddChoices(new[] { "Ja", "Nee", "Terug" })
				);
				switch (choice)
				{
					case "Ja":
						bool loginSuccessful = false;
						while (!loginSuccessful)
						{
							string email = AnsiConsole.Prompt(
							new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nVoer uw [bold blue]email[/] in:")
								.PromptStyle("blue")
								.Validate(email =>
								{
									if (string.IsNullOrWhiteSpace(email))
									{
										return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
									}
									if (email.ToLower() == "terug")
									{
										return ValidationResult.Success();
									}
									return ValidationResult.Success();
								})
							);
							if (email.ToLower() == "terug")
							{
								break;
							}
							string password = AnsiConsole.Prompt(
								new TextPrompt<string>("Voer uw [bold blue]wachtwoord[/] in:")
									.PromptStyle("blue")
									.Secret()
									.Validate(password =>
									{
										if (string.IsNullOrWhiteSpace(password))
										{
											return ValidationResult.Error("[red]Wachtwoord mag niet leeg zijn[/]");
										}
										if (email.ToLower() == "terug")
										{
											return ValidationResult.Success();
										}
										return ValidationResult.Success();
									})
							);
							if (password.ToLower() == "terug")
							{
								break;
							}
							int result = Customer.FindCustomer(db, email.ToLower(), password);

							if (result == 1)
							{
								// Credentials matched, proceed with login
								Customer customer = db.Customers.FirstOrDefault(x => x.Email == email && x.Password == password);
								AnsiConsole.Status()
									.Spinner(Spinner.Known.Aesthetic)
									.SpinnerStyle(Style.Parse("white"))
									.Start($"[green]Inloggen succesvol![/] [white]Ga verder met de [blue]reservering[/] voor: [bold grey93]{showtime.Movie.Title}[/][/]", ctx =>
									{
										loginSuccessful = true;
										Task.Delay(3000).Wait();
									});
								reservationSuccesful = true;
								CreateTicket(db, customer, showtime, selectedSeats, ticketNumber, customer.Email, voucherused);
								sender.SendMessage(customer.Email, customer.Username, showtime.Movie.Title, showtime.StartTime.ToString("dd-MM-yyyy"), showtime.StartTime.ToString("HH:mm"), string.Join(", ", selectedSeats.Select(x => $"{x.Row}{x.SeatNumber}")), showtime.RoomId, ticketNumber);
								DisplayReservationConfirmation(db, loggedInCustomer, showtime, selectedSeats, ticketNumber);
								return;
							}
							else if (result == 2)
							{
								// Email found but password didn't match
								AnsiConsole.MarkupLine("[red]Wachtwoord komt niet overeen met deze email. Probeer het opnieuw.[/]");
							}
							else
							{
								// No account found with the provided email
								AnsiConsole.MarkupLine("[red]Geen account gevonden met deze inloggegevens. Probeer het opnieuw.[/]");
							}
						}
						break;
					case "Nee":
						bool validEmail = false;
						string guestEmail = "";
						while (!validEmail)
						{
							guestEmail = AnsiConsole.Prompt(
							new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nOm verder te kunnen als [blue]gast[/],voer uw [bold blue]email[/] in:")
								.PromptStyle("blue")
								.Validate(email =>
								{
									if (string.IsNullOrWhiteSpace(email))
									{
										return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
									}
									if (email.ToLower() == "terug")
									{
										return ValidationResult.Success();
									}
									if (!RegisterValidity.CheckEmail(email))
									{
										return ValidationResult.Error("[red]Email voldoet niet aan de eisen[/]");
									}
									if (db.Customers.Any(x => x.Email == email))
									{
										return ValidationResult.Error("[red]Email is verbonden aan een account[/]");
									}
									validEmail = true;
									return ValidationResult.Success();
								})
								);
							if (guestEmail.ToLower() == "terug")
							{
								break;
							}
							else
							{
								reservationSuccesful = true;
								CreateTicket(db, showtime, selectedSeats, ticketNumber, guestEmail);
								sender.SendMessage(guestEmail, "Guest", showtime.Movie.Title, showtime.StartTime.ToString("dd-MM-yyyy"), showtime.StartTime.ToString("HH:mm"), string.Join(", ", selectedSeats.Select(x => $"{x.Row}{x.SeatNumber}")), showtime.RoomId, ticketNumber);
								DisplayReservationConfirmation(db, loggedInCustomer, showtime, selectedSeats, ticketNumber);
							}
						}
						break;
					case "Terug":
						HandleReservation(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
						break;
				}
			}
			db.SaveChanges();
		}
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

	private void CreateTicket(CinemaContext db, Customer loggedInCustomer, Showtime showtime, List<CinemaSeat> selectedSeats, string ticketNumber, string userEmail, Voucher voucherused = null)
	{
		var ticket = new Ticket
		{
			Customer = loggedInCustomer,
			Showtime = showtime,
			PurchasedAt = DateTime.UtcNow.AddHours(2),
			TicketNumber = ticketNumber,
			CustomerEmail = userEmail,
			Seats = selectedSeats.ToList(),
		};
		if (voucherused is null)
		{
			ticket.PurchaseTotal = selectedSeats.Sum(s => s.Price);
		}
		else
		{
			ticket.PurchaseTotal = (decimal)voucherused.ApplyDiscount((double)selectedSeats.Sum(s => s.Price));
		}
		db.Tickets.Add(ticket);
		return;
	}



	private Voucher UseVoucher(CinemaContext db, Customer loggedInCustomer)
	{
		var voucherprompt = AnsiConsole.Prompt(
		new SelectionPrompt<string>()
			.PageSize(3)
			.AddChoices("Ja", "Nee")
			.Title("Wilt u een vouchercode invoeren?")
			.HighlightStyle(new Style(Color.Blue)));
		if (voucherprompt.Contains("Ja"))
		{
			PresentCustomerReservationProgress.UpdateTrueProgress(loggedInCustomer, db);
			PresentAdminOptions.UpdateVouchers(db);
			List<Voucher> availableVouchers = db.Vouchers.Where(x => x.Active == true && x.CustomerEmail == loggedInCustomer.Email).ToList();
			string voucherCode = AnsiConsole.Prompt(
			  new TextPrompt<string>("Voer je voucher code in (of typ 'stop' om te stoppen):")
				  .PromptStyle("yellow")
				  .Validate(input =>
				  {
					  if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
					  {
						  return ValidationResult.Success();
					  }

					  if (!availableVouchers.Any(x => x.Code == input && x.Active == true))
					  {
						  return ValidationResult.Error($"Geen geldige voucher voor code: {input}. Probeer opnieuw:");
					  }

					  return ValidationResult.Success();
				  }));
			if (voucherCode.Contains("stop"))
			{
				return null;
			}
			return availableVouchers.First(x => x.Code == voucherCode);
		}
		return null;
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
		string activeFilters = "Actieve Filters: ";

		var filterOption = AnsiConsole.Prompt(
		  new SelectionPrompt<CinemaFilterChoice>()
			.Title("Selecteer een optie om films te filteren")
			.PageSize(4)
			.AddChoices(new[]
			{
		CinemaFilterChoice.Genres,
		CinemaFilterChoice.Directeuren,
		CinemaFilterChoice.Acteurs,
		CinemaFilterChoice.Leeftijdsclassificatie,
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

			case CinemaFilterChoice.Leeftijdsclassificatie:
				var ageRatingChoice = AnsiConsole.Prompt(
				  new SelectionPrompt<string>()
					.Title("Selecteer een leeftijdsclassificatie om films te filteren")
					.AddChoices(new[] { "16+", "Onder 16" })
				);
				if (ageRatingChoice == "16+")
				{
					moviesQuery = allMovies.Where(movie => movie.MinAgeRating >= 16).AsQueryable();
					activeFilters += "Leeftijdsclassificatie: 16+";
				}
				else if (ageRatingChoice == "Onder 16")
				{
					moviesQuery = allMovies.Where(movie => movie.MinAgeRating < 16).AsQueryable();
					activeFilters += "Leeftijdsclassificatie: Onder 16";
				}
				break;

			default:
				return (null, null);
		}

		return (moviesQuery, activeFilters);
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

		AnsiConsole.Write(table);
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
				// Toon ticketinformatie
				Console.WriteLine($"Ticketnummer: {ticket.TicketNumber}");
				Console.WriteLine($"Voorstelling: {ticket.Showtime.StartTime}");
				Console.WriteLine($"Gekocht op: {ticket.PurchasedAt}");
				Console.WriteLine($"Totale prijs: {ticket.PurchaseTotal}");
				Console.WriteLine();
			}
		}
		else
		{
			Console.WriteLine("U heeft geen tickets.");
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

			// Combineer en ruim de uitvoer op met AnsiConsole
			CinemaSeat basepriceseat = db.CinemaSeats.First(x => x.Showtime == showtime && x.Color == "orange" && x.Type == 0);
			if (basepriceseat.Price != 25) Console.WriteLine();
			AnsiConsole.WriteLine();
			AnsiConsole.Markup($"Geselecteerde stoelprijs: {selectedSeatPrice} euro [grey]{new string(' ', 50)}(Druk op <Enter> om stoelen te selecteren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel
			AnsiConsole.Markup($"Geselecteerde Stoel: {(char)('A' + currentRow)}{(currentSeatNumber + 1).ToString().PadLeft(2, '0')} [grey]{new string(' ', 50)}      (Druk op <Space> om stoelen te reserveren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel
			AnsiConsole.Markup($"[grey]{new string(' ', 50)}                         (Druk op <Escape> om terug te keren)[/]");
			AnsiConsole.WriteLine(); // Zorgt voor een nieuwe regel

			CinemaReservationSystem.DrawPlan(db, showtime, (char)('A' + currentRow), currentSeatNumber + 1);
			if (basepriceseat.Price != 25) Console.WriteLine();

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
				HandleReservation(loggedInCustomer, db, showtime, selectedSeats, ticketNumber);
				break;
			}
			else if (keyInfo.Key == ConsoleKey.Escape)
			{
				foreach (var seat in selectedSeats)
				{
					seat.IsSelected = false;
				}
				Console.Clear();
				ListMoviesWithShowtimes(loggedInCustomer, db);
				break;
			}
		}
		Console.CursorVisible = true;
	}

}
