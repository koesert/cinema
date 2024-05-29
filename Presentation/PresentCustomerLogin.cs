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
            var rule = new Rule("[bold blue]Login:[/]")
            {
                Justification = Justify.Left,
                Style = Style.Parse("blue")
            };
            AnsiConsole.Write(rule);

            string email = AskEmail();
            string password = AskPassword();

            Customer customer = Customer.FindCustomer(db, email, password);

            if (customer == null)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord. Probeer het opnieuw.[/]");
            }
            else
            {
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
        }
    }

    private static string AskEmail()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold blue]email[/] in:")
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
            new TextPrompt<string>("Voer uw [bold blue]wachtwoord[/] in:")
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
