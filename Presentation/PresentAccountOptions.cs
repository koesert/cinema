using System.Text.RegularExpressions;
using Cinema;
using Cinema.Data;
using Spectre.Console;

public class PresentAccountOptions
{
    public static void Start(Customer loggedInCustomer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Account opties[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue dim")
        };
        AnsiConsole.Write(rule);

        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecteer een optie:")
                .PageSize(10)
                .AddChoices(new[] { "Email aanpassen", "Gebruikersnaam aanpassen" , "Wachtwoord aanpassen", "[red]Account verwijderen[/]" ,"Terug"})
        );

        List<Customer> existingCustomers = db.Customers.ToList();

        switch (optionChoice)
        {
            case "[blue]Email aanpassen[/]":
                AnsiConsole.Clear();
                var emailRule = new Rule("[blue]Email aanpassen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("blue dim")
                };
                AnsiConsole.Write(emailRule);
                
                string newEmail = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("Wat word uw nieuwe [blue]email[/]?")
                            .PromptStyle("blue")
                            .Validate(email =>
                            {
                                if (string.IsNullOrEmpty(email))
                                {
                                    return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
                                }
                                foreach (Customer existingCustomer in existingCustomers)
                                {
                                    if (existingCustomer.Email == email)
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
                if (AnsiConsole.Confirm($"Weet u zeker dat u '[blue]{newEmail}[/]' als email wilt gebruiken?"))
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("blue"))
                        .Start("[blue]Email[/] updaten...", ctx =>
                        {
                            loggedInCustomer.Email = newEmail;
                            Customer.UpdateCustomer(db, loggedInCustomer);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[blue]Email[/] geüpdatet!");
                    Thread.Sleep(2500);
                    Start(loggedInCustomer, db);
                }
                else
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("red"))
                        .Start("[red]Annuleren...[/]", ctx =>
                        {
                            Thread.Sleep(2500);
                        });
                    Start(loggedInCustomer, db);
                }
                break;
            case "[blue]Gebruikersnaam aanpassen[/]":
                AnsiConsole.Clear();
                var usernameRule = new Rule("[blue]Gebruikersnaam aanpassen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("blue dim")
                };
                AnsiConsole.Write(usernameRule);

                string newUsername = AnsiConsole.Prompt(
                    new TextPrompt<string>("Wat word uw nieuwe [blue]gebruikersnaam[/]?")
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
                if (AnsiConsole.Confirm($"Weet u zeker dat u '[blue]{newUsername}[/]' als gebruikersnaam wilt gebruiken?"))
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("blue"))
                        .Start("[blue]Gebruikersnaam[/] updaten...", ctx =>
                        {
                            loggedInCustomer.Username = newUsername;
                            Customer.UpdateCustomer(db, loggedInCustomer);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[blue]Gebruikersnaam[/] geüpdatet!");
                    Thread.Sleep(2500);
                    Start(loggedInCustomer, db);
                }
                else
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("red"))
                        .Start("[red]Annuleren...[/]", ctx =>
                        {
                            Thread.Sleep(2500);
                        });
                    Start(loggedInCustomer, db);
                }
                break;
            case "[blue]Wachtwoord aanpassen[/]":
                AnsiConsole.Clear();
                break;
            case "[red]Account verwijderen[/]":
                AnsiConsole.Clear();
                var deleteAccountRule = new Rule("[red]Account verwijderen[/]")
                    {
                        Justification = Justify.Left,
                        Style = Style.Parse("red dim")
                    };
                AnsiConsole.Write(deleteAccountRule);
                if (AnsiConsole.Confirm($"Weet u zeker dat u uw account wilt [red]verwijderen[/]?"))
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("red"))
                        .Start("[red]Account verwijderen...[/]", ctx =>
                        {
                            Customer.DeleteCustomer(db, loggedInCustomer);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[red]Account verwijderd. Tot ziens![/]");
                    Thread.Sleep(2500);
                    AnsiConsole.Clear();
                    break;
                }
                else
                {
                    Start(loggedInCustomer, db);
                }
                break;
            case "Terug":
                AnsiConsole.Clear();
                PresentCustomerOptions.Start(loggedInCustomer, db);
                break;
        }
    }
}