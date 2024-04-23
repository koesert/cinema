using Cinema.Data;
using Spectre.Console;

public class PresentCustomerLogin
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            var rule = new Rule("[bold blue]Klanten login[/]");
            rule.Justification = Justify.Left;
            rule.Style = Style.Parse("blue dim");
            AnsiConsole.Write(rule);

            string username = AskUsername();
            string password = AskPassword();

            Customer customer = Customer.FindCustomer(db, username, password);

            if (customer == null)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord. Probeer het opnieuw.[/]");
                AnsiConsole.WriteLine();
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Inloggen succesvol! Welkom {customer.Username}![/]");
                // PresentCustomerOptions.Start(customer);
                break;
            }
        }
    }

    private static string AskUsername()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]gebruikersnaam[/] in:")
                .PromptStyle("blue")
                .Validate(username =>
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam mag niet leeg zijn[/]");
                    }
                    return ValidationResult.Success();
                })
        );
    }

    private static string AskPassword()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]wachtwoord[/] in:")
                .PromptStyle("blue")
                .Secret()
                .Validate(password =>
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag niet leeg zijn[/]");
                    }
                    return ValidationResult.Success();
                })
        );
    }
}
