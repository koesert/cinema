using Cinema.Data;
using Cinema.Services;
using Spectre.Console;

public class PresentViewSubscribers
{
    public static void Start(CinemaContext db, Administrator admin)
    {
        List<Customer> subscribedCustomers = FetchSubscribers(db);
        AnsiConsole.Clear();

        var table = new Table();
        table.AddColumn(new TableColumn("[blue]Gebruikersnaam[/]").LeftAligned());
        table.AddColumn(new TableColumn("[blue]E-mailadres[/]").LeftAligned());
        table.Title = new TableTitle("[blue]Nieuwsbrief abonnees[/]", new Style(decoration: Decoration.Bold));

        int rowIndex = 0;
        foreach (Customer customer in subscribedCustomers)
        {
            table.AddRow(new Markup(customer.Username, rowIndex % 2 == 0 ? null : new Style(foreground: Color.Grey)),
                         new Markup(customer.Email, rowIndex % 2 == 0 ? null : new Style(foreground: Color.Grey)));
            rowIndex++;
        }

        AnsiConsole.Write(table);

        var navigateChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("")
                .AddChoices(new[] { "Terug" }));

        if (navigateChoice == "Terug")
        {
            return;
        }
    }

    private static List<Customer> FetchSubscribers(CinemaContext db)
    {
        return db.Customers.Where(c => c.Subscribed).ToList();
    }
}