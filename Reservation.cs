public class Reservation
{
    public int ReservationId { get; set; }
    public Showtime Showtime { get; set; }
    public List<Seat> ReservedSeats { get; set; }
}
