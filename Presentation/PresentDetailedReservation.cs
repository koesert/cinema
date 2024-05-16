using System.Data.Common;
using Cinema.Data;
using Spectre.Console;

public class PresentDetailedReservation
{
    public static void Start(Ticket reservation, Movie movie, Showtime showtime, CinemaContext db, Customer customer)
    {
        var seatList = db.CinemaSeats
                    .Where(t => t.Ticket.Id == reservation.Id)
                    .ToList();
        string seats =" ";
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
        AnsiConsole.Render(table);

        var navigateChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("")
                    .AddChoices(new[] { "Terug" }));

        if (navigateChoice == "Terug")
        {
            PresentReservations.Start(customer, db);
        }
    }
}