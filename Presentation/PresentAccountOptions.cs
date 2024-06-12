using System.Text.RegularExpressions;
using Cinema.Data;
using Cinema.Logic;
using Spectre.Console;

public static class PresentAccountOptions
{
    public static void Start(Customer loggedInCustomer, CinemaContext db)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Account opties:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);

        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("")
                .PageSize(10)
                .AddChoices(new[] { "Email aanpassen", "Gebruikersnaam aanpassen", "Wachtwoord aanpassen", "[hotpink2]Nieuwsbrief voorkeur[/]", "[red]Account verwijderen[/]", "[blueviolet]Terug[/]" })
        );

        List<Customer> existingCustomers = db.Customers.ToList();

        switch (optionChoice)
        {
            case "Email aanpassen":
                AnsiConsole.Clear();
                var emailRule = new Rule("[blue]Email aanpassen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("blue")
                };
                AnsiConsole.Write(emailRule);

                string newEmail = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nWat word uw nieuwe [blue]email[/]?")
                            .PromptStyle("blue")
                            .Validate(email =>
                            {
                                if (string.IsNullOrEmpty(email))
                                {
                                    return ValidationResult.Error("[red]Email mag niet leeg zijn[/]");
                                }
                                if (email.ToLower() == "terug")
                                {
                                    return ValidationResult.Success();
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
                if (newEmail.ToLower() == "terug")
                {
                    Start(loggedInCustomer, db);
                    return;
                }
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
            case "Gebruikersnaam aanpassen":
                AnsiConsole.Clear();
                var usernameRule = new Rule("[blue]Gebruikersnaam aanpassen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("blue")
                };
                AnsiConsole.Write(usernameRule);

                string newUsername = AnsiConsole.Prompt(
                    new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nWat word uw nieuwe [blue]gebruikersnaam[/]?")
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
                if (newUsername.ToLower() == "terug")
                {
                    Start(loggedInCustomer, db);
                    return;
                }
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
            case "Wachtwoord aanpassen":
                AnsiConsole.Clear();
                var passwordRule = new Rule("[blue]Wachtwoord aanpassen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("blue")
                };
                AnsiConsole.Write(passwordRule);

                string newPassword = AnsiConsole.Prompt(
                    new TextPrompt<string>("[grey]Voer 'terug' in om terug te gaan.[/]\nWat word uw nieuwe [bold blue]wachtwoord[/]?")
                        .PromptStyle("blue")
                        .Secret()
                        .Validate(password =>
                        {
                            if (password == loggedInCustomer.Password)
                            {
                                return ValidationResult.Error("[red]Wachtwoord mag niet hetzelfde zijn als uw huidige wachtwoord[/]");
                            }
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
                if (newPassword.ToLower() == "terug")
                {
                    Start(loggedInCustomer, db);
                    return;
                }
                if (AnsiConsole.Confirm($"Weet u zeker dat u uw [blue]wachtwoord[/] wilt aanpassen?"))
                {
                    AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("blue"))
                        .Start("[blue]Wachtwoord[/] updaten...", ctx =>
                        {
                            loggedInCustomer.Password = newPassword;
                            Customer.UpdateCustomer(db, loggedInCustomer);
                            Thread.Sleep(2500);
                        });
                    AnsiConsole.MarkupLine("[blue]Wachtwoord[/] geüpdatet!");
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
            case "[hotpink2]Nieuwsbrief voorkeur[/]":
                AnsiConsole.Clear();
                var newsletterRule = new Rule("[hotpink2]Nieuwsbrief voorkeur[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("hotpink2")
                };
                AnsiConsole.Write(newsletterRule);

                if (loggedInCustomer.Subscribed == true)
                {
                    if (AnsiConsole.Confirm("Wilt u zich afmelden van onze [hotpink2]nieuwsbrief[/]?"))
                    {
                        AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("hotpink2"))
                        .Start("[hotpink2]Voorkeur opslaan...[/]", ctx =>
                        {
                            Customer.UpdatePreference(loggedInCustomer, db);
                            Thread.Sleep(2500);
                        });
                        AnsiConsole.MarkupLine("[hotpink2]Voorkeur opgeslagen![/]");
                        NewsletterSubscription senders = new NewsletterSubscription();
                        senders.SendMessageCancelNewsletter(loggedInCustomer.Email, loggedInCustomer.Username);
                        Thread.Sleep(2500);
                    }
                }
                else
                {
                    if (AnsiConsole.Confirm("U bent niet aangemeld voor onze [hotpink2]nieuwsbrief[/]. Wilt u zich aanmelden?"))
                    {
                        AnsiConsole.Status()
                        .Spinner(Spinner.Known.Aesthetic)
                        .SpinnerStyle(Style.Parse("hotpink2"))
                        .Start("[hotpink2]Voorkeur opslaan...[/]", ctx =>
                        {
                            Customer.UpdatePreference(loggedInCustomer, db);
                            Thread.Sleep(2500);
                        });
                        AnsiConsole.MarkupLine("[hotpink2]Voorkeur opgeslagen![/]");
                        NewsletterSubscription senders = new NewsletterSubscription();
                        senders.SendMessageSubscribeNewsletter(loggedInCustomer.Email, loggedInCustomer.Username);
                        Thread.Sleep(2500);
                    }
                }
                Start(loggedInCustomer, db);
                break;
            case "[red]Account verwijderen[/]":
                AnsiConsole.Clear();
                var deleteAccountRule = new Rule("[red]Account verwijderen[/]")
                {
                    Justification = Justify.Left,
                    Style = Style.Parse("red")
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
                    return;
                }
                else
                {
                    Start(loggedInCustomer, db);
                }
                break;
            case "[blueviolet]Terug[/]":
                AnsiConsole.Clear();
                PresentCustomerOptions.Start(loggedInCustomer, db);
                break;
        }
    }
}