public class Showtime
{
    public int ShowtimeId { get; set; }
    public DateTime StartTime { get; set; }
    public Movie Movie { get; set; }
    public CinemaHall Hall { get; set; }
}
