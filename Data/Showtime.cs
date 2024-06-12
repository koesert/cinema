namespace Cinema.Data
{
    public class Showtime
    {
        public int Id { get; set; }
        public string RoomId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Movie Movie { get; set; }
        public double Prices { get; set; }
        public virtual ICollection<CinemaSeat> CinemaSeats { get; set; }

        public override string ToString() => $"Zaal: {RoomId}, Starttijd: {StartTime.ToString("ddd, MMMM d hh:mm tt")}";
    }
}