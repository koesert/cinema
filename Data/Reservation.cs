public class Reservation
{
    public int ReservationId { get; set; }
    public Showtime Showtime { get; set; }

    public Customer Customer { get; set; }
    public string CustomerCode { get; set; }
    public DateTime ReservationTime { get; set; }
    public List<Seat> ReservedSeats { get; set; }
}
