using Cinema.Data;
using Spectre.Console;

public class PresentDetailedReservation
{
    public static void Start(Ticket reservation)
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Uw reserveringen:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
    }
}