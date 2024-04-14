using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data
{
    public class Showtime
    {
        public int Id { get; set; }
        public string RoomId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Movie Movie { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Seat> Prices { get; set; }
        public virtual ICollection<CinemaSeat> CinemaSeats { get; set; }
    }
}