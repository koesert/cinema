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

        string KoopDatum = ticket.PurchasedAt.ToString("dd-MM-yyyy HH:mm:ss");
        string startDatum = showtime.StartTime.ToString("dd-MM-yyyy HH:mm:ss");

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("Film");
        table.AddColumn("duur");
        table.AddColumn("zaal");
        table.AddColumn("start tijd");
        table.AddColumn("gekocht op");
        table.AddColumn("Totale prijs");

        table.AddRow(movie.Title, $"{movie.Duration} min", showtime.RoomId.ToString(), startDatum, KoopDatum, $"{ticket.PurchaseTotal} euro");
        AnsiConsole.Render(table);

        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecteer een optie:")
                .PageSize(10)
                .AddChoices(new[] { "Cancel ticket", "Terug" })
        );
        switch (optionChoice)
        {
            case "Cancel ticket":
                user.Yoyo(ticket, db);
                Console.ReadLine();
                break;
            case "Terug":
                AnsiConsole.Clear();
                break;
        }
    }
}