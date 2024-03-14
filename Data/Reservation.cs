public class Reservation
{
    public int ReservationId { get; set; }
    public Showtime Showtime { get; set; }
    public Customer Customer { get; set; }
    public string CustomerCode { get; set; }
    public DateTime ReservationTime { get; set; }
    public List<Seat> ReservedSeats { get; set; }

    public Reservation(Showtime showtime, Customer customer, DateTime time, List<Seat> seats)
    {
        Showtime = showtime;
        Customer = customer;
        CustomerCode = GenerateUniqueCode();
        ReservationTime = time;
        ReservedSeats = seats;
    }
    // Generates a unique 16-character code for this reservation.
    private string GenerateUniqueCode()
    {
        Random random = new Random();
        string randomChars = string.Join("", Enumerable.Range(0, 16).Select(i => (char)random.Next(48, 123)));
        return randomChars;
    }
}