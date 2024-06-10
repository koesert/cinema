using Cinema;
using Cinema.Data;
using Cinema.Services;
using Spectre.Console;

public static class PresentCinemaInformation
{
    public static void Start(CinemaContext db)
    {
        string time = "10:00 - 22:00";
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Bioscoop informatie:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine("\n\nHuidige Locatie(s):");
        var table1 = new Table().Border(TableBorder.Rounded);
            table1.AddColumn(new TableColumn("[blue]Witte de Withstraat 20, 3067AX Rotterdam.[/]").Centered());
        AnsiConsole.Write(table1);
        AnsiConsole.MarkupLine("\n\nOpeningstijden:");
        var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("[green]Maandag[/]").Centered());
            table.AddColumn(new TableColumn("[red]Dinsdag[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Woensdag[/]").Centered());
            table.AddColumn(new TableColumn("[orange1]Donderdag[/]").Centered());
            table.AddColumn(new TableColumn("[blue]Vrijdag[/]").Centered());
            table.AddColumn(new TableColumn("[white]Zaterdag[/]").Centered());
            table.AddColumn(new TableColumn("[white]Zondag[/]").Centered());
        table.AddRow(time, time, time, time, time, time, time);
        AnsiConsole.Write(table);
        DateTimeOffset now = DateTimeOffset.UtcNow.AddHours(2);
        if (now.TimeOfDay >= new TimeSpan(10, 0, 0) && now.TimeOfDay <= new TimeSpan(22, 0, 0))
        {
            AnsiConsole.MarkupLine("[green]\nBioscoop is op dit moment open![/]");
        }
        else
        {
           AnsiConsole.MarkupLine("[red]\nBioscoop is helaas op dit moment gesloten....[/]"); 
        }
        AnsiConsole.MarkupLine("\n\nContact:");
        var table3 = new Table().Border(TableBorder.Rounded);
            table3.AddColumn(new TableColumn("[purple]spyrabv@gmail.com[/]").Centered());
        AnsiConsole.Write(table3);
        var loginChoice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("")
                                .PageSize(10)
                                .AddChoices(new[] {"Terug"})
                        );
        return;
    }
}