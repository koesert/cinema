using System;
using System.Collections.Generic;
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
        public DateTimeOffset? CancelledAt { get; set; }
        public ICollection<CinemaSeat> Seats { get; set; } = new List<CinemaSeat>();
        public decimal PurchaseTotal { get; set; }

        public static string GenerateRandomCode(CinemaContext db)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code = "";
            
            for (int i = 0; i < 10; i++)
            {
                code += chars[random.Next(chars.Length)];
            }
            
            if (db.Ticket.ToList().Any(x => x.TicketNumber == code))
            {
                return GenerateRandomCode(db);
            }
            return code;
        }

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

        public static void CancelTicket(Ticket ticket, CinemaContext db)
        {
            ticket.CancelledAt = DateTime.UtcNow.AddHours(2);

            var seats = db.CinemaSeats.Where(seat => seat.TicketId == ticket.Id).ToList();

            foreach (var seat in seats)
            {
                seat.IsReserved = false;
                seat.IsSelected = false;
            }

            db.SaveChanges();
        }
    }



}