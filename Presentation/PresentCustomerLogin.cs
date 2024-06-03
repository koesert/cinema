using Cinema.Data;
using Spectre.Console;

public class PresentCustomerLogin
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();
        bool loginSuccessful = false;
        var rule = new Rule("[bold blue]Login:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
        while (!loginSuccessful)
        {
            string email = AskEmail();
            if (email.ToLower() == "terug")
            {
                return;
            }
            string password = AskPassword();
            if (password.ToLower() == "terug")
            {
                return;
            }

            int result = Customer.FindCustomer(db, email, password);

            if (result == 1)
            {
                // Credentials matched, proceed with login
                Customer customer = db.Customers.FirstOrDefault(x => x.Email == email && x.Password == password);
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Aesthetic)
                    .SpinnerStyle(Style.Parse("blue"))
                    .Start($"[blue]Inloggen succesvol! Welkom [bold grey93]{customer.Username}[/]![/]", ctx =>
                    {
                        loginSuccessful = true;
                        Task.Delay(2500).Wait();
                    });
                PresentCustomerOptions.Start(customer, db);
                break;
            }
            else if (result == 2)
            {
                // Email found but password didn't match
                AnsiConsole.MarkupLine("[red]Wachtwoord komt niet overeen met deze email. Probeer het opnieuw.[/]");
            }
            else
            {
                // No account found with the provided email
                AnsiConsole.MarkupLine("[red]Geen account gevonden met deze inloggegevens. Probeer het opnieuw.[/]");
            }
        }
    }

    private static string AskEmail()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nVoer uw [bold blue]email[/] in:")
                .PromptStyle("blue")
                .Validate(email =>
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
                    }
                    if (email.ToLower() == "terug")
                    {
                        return ValidationResult.Success();
                    }
                    return ValidationResult.Success();
                })
        );
    }

    private static string AskPassword()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold blue]wachtwoord[/] in:")
                .PromptStyle("blue")
                .Secret()
                .Validate(password =>
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag niet leeg zijn[/]");
                    }
                    if (password.ToLower() == "terug")
                    {
                        return ValidationResult.Success();
                    }
                    return ValidationResult.Success();
                })
        );
    }
}
