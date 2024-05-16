using Cinema.Data;
using Spectre.Console;
using Microsoft.EntityFrameworkCore;

public class PresentReservations
{
    public static void Start(Customer customer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Uw reserveringen:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);

        var customerReservations = db.Ticket
            .Include(t => t.Showtime)
            .ThenInclude(s => s.Movie)
            .Where(t => t.CustomerEmail == customer.Email)
            .ToList();

        if (!customerReservations.Any())
        {
            AnsiConsole.MarkupLine($"Geen [blue]reserveringen[/] gevonden voor [bold grey93]{customer.Username}[/].");
            var navigateChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("")
                    .AddChoices(new[] { "Terug" }));

            if (navigateChoice == "Terug")
            {
                PresentCustomerOptions.Start(customer, db);
            }
        }
        else
        {
            var movieOptions = customerReservations
                .Select((r, index) => new { Index = index, Option = $"Reservering: [bold]{r.Showtime.Movie.Title} - {r.Showtime.StartTime:g}[/]" })
                .ToDictionary(x => x.Option, x => x.Index);

            var choices = movieOptions.Keys.ToList();
            choices.Add("Terug");

            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("")
                    .AddChoices(choices)
                    .PageSize(10));

            if (selectedOption == "Terug")
            {
                PresentCustomerOptions.Start(customer, db);
            }
            else
            {
                var selectedReservationIndex = movieOptions[selectedOption];
                var selectedReservation = customerReservations[selectedReservationIndex];
                var selectedMovie = selectedReservation.Showtime.Movie;
                var selectedShowtime = selectedReservation.Showtime;
                PresentDetailedReservation.Start(selectedReservation, selectedMovie, selectedShowtime, db, customer);
            }
        }
    }
}