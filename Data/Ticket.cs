﻿using System.ComponentModel.DataAnnotations.Schema;

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
    }
}