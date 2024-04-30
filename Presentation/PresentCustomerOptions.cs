using Cinema.Data;
using Spectre.Console;

public class PresentCustomerOptions
{
    public static void Start(Customer loggedInCustomer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Klanten opties[/]");
            rule.Justification = Justify.Left;
            rule.Style = Style.Parse("blue dim");
            AnsiConsole.Write(rule);
        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecteer een optie:")
                .PageSize(10)
                .AddChoices(new[] { "Reserveringen bekijken", "Account beheren" , "Terug"})
        );
        switch (optionChoice)
        {
            case "Reserveringen bekijken":
                AnsiConsole.Clear();
                break;
            case "Account beheren":
                AnsiConsole.Clear();
                break;
            case "Terug":
                AnsiConsole.Clear();
                break;
        }
    }
}