using Cinema.Data;
using Spectre.Console;

public static class PresentAccountOptions
{
    [Obsolete]
    public static void Start(Customer loggedInCustomer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Account opties[/]");
        rule.Justification = Justify.Left;
        rule.Style = Style.Parse("blue");
        AnsiConsole.Write(rule);
        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecteer een optie:")
                .PageSize(10)
                .AddChoices(new[] { "Email aanpassen", "Gebruikersnaam aanpassen", "Wachtwoord aanpassen", "Terug" })
        );
        switch (optionChoice)
        {
            case "Email aanpassen":
                AnsiConsole.Clear();
                break;
            case "Gebruikersnaam aanpassen":
                AnsiConsole.Clear();
                break;
            case "Wachtwoord aanpassen":
                AnsiConsole.Clear();
                break;
            case "Terug":
                AnsiConsole.Clear();
                PresentCustomerOptions.Start(loggedInCustomer, db);
                break;
        }
    }
}