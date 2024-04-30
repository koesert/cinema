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

            string email = AskEmail();
            string password = AskPassword();

            Customer customer = Customer.FindCustomer(db, email, password);

            if (customer == null)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord. Probeer het opnieuw.[/]");
                AnsiConsole.WriteLine();
            }
            else
            {
                loginSuccessful = true;
                AnsiConsole.MarkupLine($"[green]Inloggen succesvol! Welkom {customer.Username}![/]");
                // PresentCustomerOptions.Start(customer);
                continue;
            }
        }
    }

    private static string AskEmail()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]email[/] in:")
                .PromptStyle("blue")
                .Validate(email =>
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
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
