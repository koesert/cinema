using Cinema.Data;
using Spectre.Console;

public class PresentReservations
{
    public static void Start(Customer customer, CinemaContext db)
    {
        List<Ticket> AllCustomersReservations = db.Ticket.ToList();
        List<Ticket> ThisCustomerReservations = new List<Ticket>();

        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Uw reservaties:[/]");
            rule.Justification = Justify.Left;
            rule.Style = Style.Parse("blue");
            AnsiConsole.Write(rule);

        foreach(Ticket Reservation in AllCustomersReservations)
        {
            if (Reservation.Customer.Id == customer.Id)
            {
                ThisCustomerReservations.Add(Reservation) ;
            }
        }
        if (ThisCustomerReservations.Count == 0)
        {
            AnsiConsole.MarkupLine($"Nog geen [blue]reservaties[/] voor [bold grey93]{customer.Username}[/].");
        }
        else
        {
            foreach (Ticket Reservation in ThisCustomerReservations)
            { // Dit nog veranderen zodat er movie details getoond worden
                AnsiConsole.MarkupLine($"Ticket with id: [bold grey93]{Reservation.Id}[/] reserved under email: [bold grey93]{Reservation.CustomerEmail}[/]");
            }
        }
    }
}