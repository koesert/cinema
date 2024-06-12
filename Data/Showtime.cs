using System.Globalization;


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

        public override string ToString()
        {
            CultureInfo nl = new CultureInfo("nl-NL");
            return $"Zaal: {RoomId}, Begintijd: {StartTime.ToString("ddd, d MMMM HH:mm", nl)}";
        }

    }
}