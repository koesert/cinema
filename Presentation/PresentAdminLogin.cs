using Cinema.Data;
using Spectre.Console;

public class PresentAdminLogin
{
    public static void Start(CinemaContext db)
    {
        AnsiConsole.Clear();
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            AnsiConsole.MarkupLine("[bold blue]Administrator Login[/]");
            AnsiConsole.WriteLine();

            string username = AskUsername();
            string password = AskPassword();

            loginSuccessful = Administrator.FindAdministrator(db, username, password);

            if (!loginSuccessful)
            {
                AnsiConsole.MarkupLine("[red]Ongeldige gebruikersnaam of wachtwoord. Probeer het opnieuw.[/]");
                AnsiConsole.WriteLine();
            }
            else
            {
                AnsiConsole.MarkupLine("[green]Inloggen succesvol! Welkom, Administrator.[/]");
                // PresentAdminOptions.Start();
            }
        }
    }

    private static string AskUsername()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]gebruikersnaam[/] in:")
                .PromptStyle("blue")
                .Validate(username =>
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        return ValidationResult.Error("[red]Gebruikersnaam mag niet leeg zijn[/]");
                    }
                    return ValidationResult.Success();
                })
        );
    }

    private static string AskPassword()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Voer uw [bold green]wachtwoord[/] in:")
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
