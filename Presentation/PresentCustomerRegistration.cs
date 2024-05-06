using System.Text.RegularExpressions;
using Cinema.Data;
using Spectre.Console;

public class PresentCustomerRegistration
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();

        List<Customer> existingCustomers = db.Customers.ToList();

        bool registerSuccesful = false;

        while (!registerSuccesful)
        {
            var rule = new Rule("[bold blue]Klanten registratie[/]");
            rule.Justification = Justify.Left;
            rule.Style = Style.Parse("blue");
            AnsiConsole.Write(rule);

            string username = AskUsername(existingCustomers);
            string email = AskEmail(existingCustomers);
            string password = AskPassword();

            Customer newCustomer = Customer.CreateCustomer(db, username, password, email);

            if (newCustomer != null)
            {
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Aesthetic)
                    .SpinnerStyle(Style.Parse("bold blue"))
                    .Start($"Account voor [bold blue]{username}[/] is succesvol aangemaakt!", ctx =>
                    {
                        registerSuccesful = true;
                        Task.Delay(3000).Wait();
                        
                    });
                PresentCustomerOptions.Start(newCustomer, db);
            }
        }
    }

    private static string AskUsername(List<Customer> existingCustomers)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold blue]gebruikersnaam[/] in:")
                .PromptStyle("blue")
                .Validate(username =>
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam mag niet leeg zijn[/]");
                    }
                    if (!Regex.IsMatch(username, "^[a-zA-Z0-9_]+$"))
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam mag alleen (hoofd)letters, cijfers en lage streepjes bevatten[/]");
                    }
                    if (username.Length < 4)
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam moet minimaal 4 tekens lang zijn[/]");
                    }
                    foreach (Customer existingCustomer in existingCustomers)
                    {
                        if (existingCustomer.Username.ToLower() == username.ToLower())
                        {
                            return ValidationResult.Error("[red]Deze gebruikersnaam is al in gebruik[/]");
                        }
                    }
                    return ValidationResult.Success();
                })
        );
    }

    private static string AskEmail(List<Customer> existingCustomers)
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
                    foreach (Customer existingCustomer in existingCustomers)
                    {
                        if (existingCustomer.Email.ToLower() == email.ToLower())
                        {
                            return ValidationResult.Error("[red]Deze email is al in gebruik[/]");
                        }
                    }
                    if (!RegisterValidity.CheckEmail(email))
                    {
                        return ValidationResult.Error("[red]Email is ongeldig[/]");
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
                    if (password.Length < 6)
                    {
                        return ValidationResult.Error("[red]Wachtwoord moet minimaal 6 tekens lang zijn[/]");
                    }
                    if (!password.Any(char.IsDigit))
                    {
                        return ValidationResult.Error("[red]Wachtwoord moet minimaal één cijfer bevatten[/]");
                    }
                    if (!password.All(char.IsLetterOrDigit))
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag alleen letters (hoofdletters en kleine letters) en cijfers bevatten[/]");
                    }
                    return ValidationResult.Success();
                })
        );
    }
}