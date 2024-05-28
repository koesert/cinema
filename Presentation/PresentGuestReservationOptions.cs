using Cinema.Data;
using Spectre.Console;
using Microsoft.EntityFrameworkCore;


public static class PresentGuestReservationOptions
{
    [Obsolete]
    public static void Start(Ticket ticket, CinemaContext db)
    {
        UserExperienceService user = new UserExperienceService();
        AnsiConsole.Clear();

        var rule = new Rule("[bold blue]gast gebruiker[/]");
        rule.Justification = Justify.Left;
        rule.Style = Style.Parse("blue");
        AnsiConsole.Write(rule);

        Console.WriteLine("Ticket:\n");

        Movie movie = db.Tickets
                   .Where(t => t.TicketNumber == ticket.TicketNumber)
                   .Select(t => t.Showtime.Movie)
                   .FirstOrDefault();

        Showtime showtime = db.Tickets
                           .Where(t => t.TicketNumber == ticket.TicketNumber)
                           .Select(t => t.Showtime)
                           .FirstOrDefault();

        var now = DateTimeOffset.UtcNow.AddHours(2);
        var halfHourBeforeShowtime = ticket.Showtime.StartTime.AddMinutes(-30);
        if (ticket.CancelledAt.HasValue)
        {
            AnsiConsole.MarkupLine("[red]Ticket is geannuleerd.[/]");

        }
        else if (now > halfHourBeforeShowtime)
        {
            AnsiConsole.MarkupLine("[red]De vertoning is al begonnen of geweest.[/]");
        }

        var seatList = db.CinemaSeats
                    .Where(t => t.TicketId == ticket.Id)
                    .ToList();
        string seats = " ";
        foreach (var item in seatList)
        {
            seats += $"type:{item.Layout} {item.Row}{item.SeatNumber}\n ";
        }

        string KoopDatum = ticket.PurchasedAt.ToString("dd-MM-yyyy HH:mm:ss");
        string startDatum = showtime.StartTime.ToString("dd-MM-yyyy HH:mm");

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("Film");
        table.AddColumn("duur");
        table.AddColumn("zaal");
        table.AddColumn("stoel");
        table.AddColumn("start tijd");
        table.AddColumn("gekocht op");
        table.AddColumn("Totale prijs");

        table.AddRow(movie.Title, $"{movie.Duration} min", showtime.RoomId.ToString(), seats, startDatum, KoopDatum, $"{ticket.PurchaseTotal} euro");
        AnsiConsole.Render(table);

        var choices = new List<string>();

        if (!ticket.CancelledAt.HasValue && now < halfHourBeforeShowtime)
        {
            choices.Add("Cancel ticket");
        }

        choices.Add("Terug");

        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecteer een optie:")
                .PageSize(10)
                .AddChoices(choices.ToArray())
        );
        switch (optionChoice)
        {
            case "Cancel ticket":
                AnsiConsole.Clear();
                var deleteTicketRule = new Rule("[red]Ticket annuleren[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("red dim")
                };
                AnsiConsole.Write(deleteTicketRule);
                if (AnsiConsole.Confirm($"Weet u zeker dat u uw ticket wilt [red]annuleren[/]?"))
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("red"))
                        .Start("[red]Ticket verwijderen...[/]", ctx =>
                        {
                            Ticket.CancelTicket(ticket, db);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[red]Ticket geannuleerd. Tot ziens![/]");
                    CancellationEmails sender = new CancellationEmails();
                    sender.SendMessageCancel(ticket.CustomerEmail, "Guest", ticket.Showtime.Movie.Title, ticket.Showtime.StartTime.ToString("dd-MM-yyyy"), ticket.Showtime.StartTime.ToString("HH:mm"), string.Join(", ", db.CinemaSeats.Where(x => x.TicketId == ticket.Id).ToList().Select(x => $"{x.Row}{x.SeatNumber}")), ticket.Showtime.RoomId, ticket.TicketNumber);
                    Thread.Sleep(2500);
                    AnsiConsole.Clear();
                    break;
                }
                else
                {
                    Start(ticket, db);
                }
                break;

            case "Terug":
                AnsiConsole.Clear();
                break;
        }
    }
}