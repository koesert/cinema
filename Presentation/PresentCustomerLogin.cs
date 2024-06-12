
using System.Data.Common;
using Cinema.Data;
using Microsoft.EntityFrameworkCore.Query;
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
            string password = AskPassword(db, email);
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

    private static string AskPassword(CinemaContext db, string email)
    {
        while (true)
        {
            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("[grey]Voer 'vergeten' in om een nieuw wachtwoord in te stellen.[/]\n Voer uw [bold blue]wachtwoord[/] in:")
                .PromptStyle("blue")
                .Secret()
                .Validate(passwordInput =>
                {
                    if (string.IsNullOrWhiteSpace(passwordInput))
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag niet leeg zijn[/]");
                    }
                    if (passwordInput.ToLower() == "terug")
                    {
                        return ValidationResult.Success();
                    }
                    if (passwordInput.ToLower() == "vergeten")
                    {
                        return ValidationResult.Success();
                    }
                    return ValidationResult.Success();
                })
            );

            if (password.ToLower() == "vergeten")
            {
                bool resetSuccesful = false;
                string resetCode = GenerateRandomCode(db);
                ResetCode rc = new ResetCode(db);
                rc.SendMessageResetCode(email, resetCode);
                while(!resetSuccesful)
                {
                    string userInput = AskResetCode();
                    
                    if (userInput == resetCode)
                    {
                        AnsiConsole.MarkupLine("[green]Identiteit geverifieerd, ga verder met uw wachtwoord resetten[/]");
                        string newPassword = AskNewPassword(db, email);
                        if (newPassword.ToLower() == "terug")
                        {
                            return newPassword;
                        }
                        rc.UpdateResetCodeInDatabase(db,newPassword, resetCode);
                        Customer customer = db.Customers.FirstOrDefault(x => x.Email == email);
                        rc.UpdatePassword(db, customer, newPassword);
                        resetSuccesful = true;
                        AnsiConsole.MarkupLine("[green]Wachtwoord succesvol gereset![/]");
                        return newPassword;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Codes komen niet overeen. Probeer het opnieuw[/]");
                    }
                }       
            }
            else
            {
                return password;
            }
        }
    }

    private static string AskResetCode()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer de [bold blue]resetcode[/] in:")
            .PromptStyle("blue")
            .Validate(resetCode =>
            {
                if (string.IsNullOrWhiteSpace(resetCode))
                {
                    return ValidationResult.Error("[red]Resetcode mag niet leeg zijn[/]");
                }
                return ValidationResult.Success();
            })
        );
    }
    private static string AskNewPassword(CinemaContext db, string email)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Voer 'terug' in om te annuleren.[/]\nVoer uw nieuwe [bold blue]wachtwoord[/] in:")
                .PromptStyle("blue")
                .Secret()
                .Validate(password =>
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag niet leeg zijn[/]");
                    }
                    Customer customer = db.Customers.FirstOrDefault(x => x.Email == email);
                    if (password == customer.Password)
                    {
                        return ValidationResult.Error("[red]Wachtwoord mag niet hetzelfde zijn als uw huidige wachtwoord[/]");
                    }
                    if (password.ToLower() == "terug")
                    {
                        return ValidationResult.Success();
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
    private static string GenerateRandomCode(CinemaContext db)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = string.Empty;

        for (int i = 0; i < 10; i++)
        {
            code += chars[random.Next(chars.Length)];
        }

        if (db.Customers.Any(x => x.ResetCode == code))
        {
            return GenerateRandomCode(db);
        }
        return code;
    }
}