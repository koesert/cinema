using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Cinema.Data
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public Showtime Showtime { get; set; }
        public Customer Customer { get; set; }
        public string CustomerEmail { get; set; }
        public DateTimeOffset PurchasedAt { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        public ICollection<CinemaSeat> Seats { get; set; } = new List<CinemaSeat>();
        public decimal PurchaseTotal { get; set; }


        private static List<Ticket> RetrieveTickets(CinemaContext db)
        {
            var AllTickets = db.Ticket.ToList();
            return AllTickets;
        }


        public static Ticket FindTicket(CinemaContext db, string ticketNumber, string email)
        {
            var AllTickets = RetrieveTickets(db);

            foreach (Ticket ticket in AllTickets)
            {
                if (ticket.TicketNumber == ticketNumber && ticket.CustomerEmail == email)
                {
                    return ticket;
                }
            }
            return null;
        }

        public static void DeleteTicket(Ticket ticket, CinemaContext db)
        {

            db.Ticket.Update(ticket);
            db.SaveChanges();
        }
    }


}