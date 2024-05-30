using Cinema.Data;
using Cinema.Services;
using Spectre.Console;

public class PresentAdminLogin
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            var rule = new Rule("[green]Administrator login:[/]")
            {
                Justification = Justify.Left,
                Style = Style.Parse("darkgreen")
            };
            AnsiConsole.Write(rule);

            string username = AskUsername();
            if (username.ToLower() == "terug")
            {
                return;
            }
            string password = AskPassword();
            if (password.ToLower() == "terug")
            {
                return;
            }

            loginSuccessful = Administrator.FindAdministrator(db, username, password);

            if (!loginSuccessful)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord. Probeer het opnieuw.[/]");
            }
            else
            {
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Aesthetic)
                    .SpinnerStyle(Style.Parse("green"))
                    .Start($"[green]Inloggen succesvol! Welkom [bold grey93]{username}[/]![/]", ctx =>
                    {
                        loginSuccessful = true;
                        Task.Delay(2500).Wait();
                    });
                PresentAdminOptions.Start(db.Administrators.FirstOrDefault(x => x.Username == username && x.Password == password), db);
                break;
            }
        }
    }

    private static string AskUsername()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Voer 'terug' in terug te gaan.[/]\nVoer uw [bold green]gebruikersnaam[/] in:")
                .PromptStyle("darkgreen")
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
                    return ValidationResult.Success();
                })
        );
    }

    private static string AskPassword()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]wachtwoord[/] in:")
                .PromptStyle("darkgreen")
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
