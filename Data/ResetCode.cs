using System;
using MailKit;
using MimeKit;
using System.Net.Mail;
using System.Net;
using Spectre.Console;

namespace Cinema.Data
{
    public class ResetCode
{
    private readonly CinemaContext _db;

    public ResetCode(CinemaContext db)
    {
        _db = db;
    }

    public void SendMessageResetCode(string email, string code)
    {
        string userName = _db.Customers.FirstOrDefault(c => c.Email == email)?.Username;
        string userResetCode = code;

        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
        {
            smtpClient.Credentials = new NetworkCredential("spyrabv@gmail.com", "ivai afee sfeb xbhe");
            smtpClient.EnableSsl = true;

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("spyrabv@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Uw code om uw wachtwoord te resetten";

                mailMessage.Body = $@"
Beste {userName},

We hebben uw verzoek voor een wachtwoordherstel ontvangen. Gebruik de onderstaande code om uw wachtwoord te resetten:

Resetcode: {userResetCode}

Volg de onderstaande stappen om een nieuw wachtwoord aan te maken:

Ga terug naar de applicatie.
Voer de bovenstaande resetcode in.
Volg de instructies op de pagina om een nieuw wachtwoord aan te maken.
Mocht u nog vragen hebben of problemen ondervinden, neem dan gerust contact met ons op via spyrabv@gmail.com.

We helpen u graag verder en kijken ernaar uit om u weer bij Your Eyes te verwelkomen.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com";

                try
                {
                    smtpClient.Send(mailMessage);
                    AnsiConsole.MarkupLine("[green]Reset code is naar uw email verstuurd![/]");
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Kon email niet verzenden: {ex.Message}");
                }
            }
        }

        UpdateResetCodeInDatabase(_db, email, userResetCode);
    }

    

    public void UpdateResetCodeInDatabase(CinemaContext db, string userEmail, string resetCode)
    {
        var customer = db.Customers.FirstOrDefault(c => c.Email == userEmail);
        if (customer != null)
        {
            customer.ResetCode = resetCode;
            db.SaveChanges();
        }
    }
    public static string AskNewPassword(CinemaContext db, string userEmail)
{   string oldPassword = db.Customers.FirstOrDefault(c => c.Email == userEmail)?.Password;
    string newPassword = AnsiConsole.Prompt(
        new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nWat word uw nieuwe [bold blue]wachtwoord[/]?")
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
                if (password == oldPassword)
                {
                    return ValidationResult.Error("[red]Nieuw wachtwoord kan niet hetzelfde zijn als het oude wachtwoord[/]");
                }
                return ValidationResult.Success();
            })
    );
    return newPassword;
    }
            public void UpdatePassword(CinemaContext db, Customer customer, string newPassword)
            {
                AnsiConsole.Status()
                .Spinner(Spinner.Known.Aesthetic)
                .SpinnerStyle(Style.Parse("blue"))
                .Start("[blue]Wachtwoord[/] updaten...", ctx =>
                    {
                        customer.Password = newPassword;
                        Customer.UpdateCustomer(db, customer);
                        Thread.Sleep(2500);
                    });

                AnsiConsole.MarkupLine("[blue]Wachtwoord[/] geüpdatet!");
                Thread.Sleep(2500);
                PresentCustomerOptions.Start(customer, db);
            }
    }
}