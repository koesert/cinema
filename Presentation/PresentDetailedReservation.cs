using Cinema.Data;
using Spectre.Console;

public class PresentDetailedReservation
{
    public static void Start(Ticket reservation, Movie movie, Showtime showtime, CinemaContext db, Customer customer)
    {
        var seatList = db.CinemaSeats
                    .Where(t => t.Ticket.Id == reservation.Id)
                    .ToList();
        string seats = " ";
        foreach (var item in seatList)
        {
            seats += $"Soort:{item.Layout} {item.Row}{item.SeatNumber}\n ";
        }
        string KoopDatum = reservation.PurchasedAt.ToString("dd-MM-yyyy HH:mm:ss");
        string startDatum = showtime.StartTime.ToString("dd-MM-yyyy HH:mm");
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Reservering:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
        var now = DateTimeOffset.UtcNow.AddHours(2);
        var halfHourBeforeShowtime = reservation.Showtime.StartTime.AddMinutes(-30);
        if (reservation.CancelledAt.HasValue)
        {
            AnsiConsole.MarkupLine("[red]Ticket is geannuleerd.[/]");

        }
        else if (now > halfHourBeforeShowtime)
        {
            AnsiConsole.MarkupLine("[red]De vertoning is al begonnen of geweest.[/]");
        }

        var table = new Table
        {
            Border = TableBorder.Rounded
        };
        table.AddColumn("Film");
        table.AddColumn("Duur");
        table.AddColumn("Zaal");
        table.AddColumn("Stoel(en)");
        table.AddColumn("Start tijd");
        table.AddColumn("Gekocht op");
        table.AddColumn("Totale prijs");

        table.AddRow(movie.Title, $"{movie.Duration} min", showtime.RoomId.ToString(), seats, startDatum, KoopDatum, $"{reservation.PurchaseTotal} euro");
        AnsiConsole.Write(table);

        var choices = new List<string>();

        if (!reservation.CancelledAt.HasValue && now < halfHourBeforeShowtime)
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
                        .Start("[red]Ticket annuleren...[/]", ctx =>
                        {
                            Ticket.CancelTicket(reservation, db);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[red]Ticket geannuleerd.[/]");
                    CancellationEmails sender = new CancellationEmails();
                    sender.SendMessageCancel(reservation.CustomerEmail, customer.Username, reservation.Showtime.Movie.Title, reservation.Showtime.StartTime.ToString("dd-MM-yyyy"), reservation.Showtime.StartTime.ToString("HH:mm"), string.Join(", ", db.CinemaSeats.Where(x => x.TicketId == reservation.Id).ToList().Select(x => $"{x.Row}{x.SeatNumber}")), reservation.Showtime.RoomId, reservation.TicketNumber);
                    Thread.Sleep(2500);
                    AnsiConsole.Clear();
                    PresentReservations.Start(customer, db);
                    break;
                }
                else
                {
                    PresentReservations.Start(customer, db);
                }
                break;

            case "Terug":
                PresentReservations.Start(customer, db);
                break;
        }
    }
}