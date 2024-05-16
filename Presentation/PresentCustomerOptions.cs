using Cinema.Data;
using Spectre.Console;

public class PresentCustomerOptions
{
    public static void Start(Customer loggedInCustomer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Klanten opties:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine($"[bold blue]Welkom [bold grey93]{loggedInCustomer.Username}[/]![/]");
        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("")
                .PageSize(10)
                .AddChoices(new[] { "Bladeren door films", "Reserveringen bekijken", "Account beheren" , "Terug"})
        );
        switch (optionChoice)
        {
            case "Bladeren door films":
                AnsiConsole.Clear();
                break;
            case "Reserveringen bekijken":
                PresentReservations.Start(loggedInCustomer, db);
                break;
            case "Account beheren":
                AnsiConsole.Clear();
                PresentAccountOptions.Start(loggedInCustomer, db);
                break;
            case "Terug":
                AnsiConsole.Clear();
                break;
        }
    }
}