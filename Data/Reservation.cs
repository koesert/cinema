public class Reservation
{
    public int ReservationId { get; set; }
    public Showtime Showtime { get; set; }
    public List<Seat> ReservedSeats { get; set; }

    public Customer Customer { get; set; }
    public DateTime ReservationTime { get; set; }
}
