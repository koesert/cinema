namespace Cinema.Data
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public Showtime Showtime { get; set; }
        public Customer Customer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTimeOffset PurchasedAt { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        public DateTimeOffset LastChangedAt { get; set; }
        public List<CinemaSeat> Seats { get; set; }
        public decimal PurchaseTotal => Seats.Sum(s => s.Price);
    }
}