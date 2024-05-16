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
        var seatList = db.CinemaSeats
                    .Where(t => t.TicketId == ticket.Id)
                    .ToList();
        string seats =" ";
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

        if (2 > 1)
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
                Console.ReadLine();
                break;
            case "Terug":
                AnsiConsole.Clear();
                break;
        }
    }
}