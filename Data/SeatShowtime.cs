using Cinema.Data;

public class SeatShowTime
{
  public int Id { get; set; }
  public virtual ICollection<SeatShowTime> Showtimes { get; set; }
  public virtual ICollection<CinemaSeat> Seats { get; set; }

}