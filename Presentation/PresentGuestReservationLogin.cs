using Cinema.Data;
using Spectre.Console;

public class PresentGuestReservationLogin
{
    [Obsolete]
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            var rule = new Rule("[bold blue]Gast ticket reservatie[/]");
            rule.Justification = Justify.Left;
            rule.Style = Style.Parse("blue");
            AnsiConsole.Write(rule);

            string email = AskEmail();
            string TicketNumber = AskTicketNumber();

            bool EmailGuest = Customer.CheckEmailCustomer(db, email);
            Ticket ticketGuest = Ticket.FindTicket(db, TicketNumber, email);

            if (EmailGuest)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]De gebruikte e-mail heeft een account. Log in om alle reserveringen te bekijken[/]");

                break;
            }
            if (ticketGuest == null)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige e-mail of ticket nummer. Probeer het opnieuw.[/]");

            }
            else
            {
                PresentGuestReservationOptions.Start(ticketGuest, db);
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

    private static string AskTicketNumber()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold blue]ticket nummer[/] in:")
                .PromptStyle("blue")
                .Secret()
                .Validate(TicketNumber =>
                {
                    if (string.IsNullOrWhiteSpace(TicketNumber))
                    {
                        return ValidationResult.Error("[red]ticket nummer mag niet leeg zijn[/]");
                    }
                    return ValidationResult.Success();
                })
        );
    }
}
