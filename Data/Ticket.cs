namespace Cinema.Data
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public Showtime Showtime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTimeOffset PurchasedAt { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        public DateTimeOffset LastChangedAt { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public decimal PurchaseTotal { get; set; }
    }
}