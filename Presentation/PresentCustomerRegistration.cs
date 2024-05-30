using System.Text.RegularExpressions;
using Cinema.Data;
using Spectre.Console;

public class PresentCustomerRegistration
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();

        List<Customer> existingCustomers = db.Customers.ToList();

        List<Ticket> existingTicekts = db.Ticket.ToList();

        bool registerSuccesful = false;

        while (!registerSuccesful)
        {
            var rule = new Rule("[bold blue]Registreren:[/]")
            {
                Justification = Justify.Left,
                Style = Style.Parse("blue")
            };
            AnsiConsole.Write(rule);

            string username = AskUsername(existingCustomers);
            if (username.ToLower() == "terug")
            {
                return;
            }
            string email = AskEmail(existingCustomers, existingTicekts);
            if (email.ToLower() == "terug")
            {
                return;
            }
            string password = AskPassword();
            if (password.ToLower() == "terug")
            {
                return;
            }

            Customer newCustomer = Customer.CreateCustomer(db, username, password, email);

            if (newCustomer != null)
            {
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Aesthetic)
                    .SpinnerStyle(Style.Parse("bold blue"))
                    .Start($"Account voor [bold blue]{username}[/] is succesvol aangemaakt!", ctx =>
                    {
                        registerSuccesful = true;
                        Task.Delay(2500).Wait();

                    });
                PresentCustomerOptions.Start(newCustomer, db);
            }
        }
    }

    private static string AskUsername(List<Customer> existingCustomers)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Voer 'terug' in terug te gaan.[/]\nVoer uw [bold blue]gebruikersnaam[/] in:")
                .PromptStyle("blue")
                .Validate(username =>
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam mag niet leeg zijn[/]");
                    }
                    if (username.ToLower() == "terug")
                    {
                        return ValidationResult.Success();
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

    private static string AskEmail(List<Customer> existingCustomers, List<Ticket> existingTickets)
    {
        string email = "";
        bool isValidEmail = false;

        while (!isValidEmail)
        {
            email = AnsiConsole.Prompt(
                new TextPrompt<string>("Voer uw [bold blue]email[/] in:")
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
                        if (!RegisterValidity.CheckEmail(email))
                        {
                            return ValidationResult.Error("[red]Email voldoet niet aan de eisen[/]");
                        }
                        return ValidationResult.Success();
                    })
            );

            if (existingCustomers.Any(c => c.Email.ToLower() == email.ToLower()))
                AnsiConsole.MarkupLine("[red]Deze email is al in gebruik[/]");
            else
            {
                Ticket existingTicket = existingTickets.FirstOrDefault(t => t.CustomerEmail.ToLower() == email.ToLower());

                if (existingTicket != null)
                {
                    AnsiConsole.MarkupLine("[red]Deze email word al gebruikt voor een reservering als gast[/]");
                    bool validTicketNumber = false;

                    while (!validTicketNumber)
                    {
                        string ticketNumber = AnsiConsole.Ask<string>("Om verder te gaan, voer het [bold blue]ticketnummer[/] in van de laatst gemaakte reservering met deze email:");

                        if (existingTicket.TicketNumber == ticketNumber)
                        {
                            AnsiConsole.MarkupLine("[green]Ticketreservering geverifieerd, je kunt doorgaan met de registratie[/]");
                            isValidEmail = true;
                            validTicketNumber = true;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Ongeldige ticketnummer voor deze email[/]");
                        }
                    }
                }
                else
                {
                    isValidEmail = true;
                }
            }
        }

        return email;
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