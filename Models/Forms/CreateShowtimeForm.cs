using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Forms
{
  public class CreateShowtimeForm
  {

    [Required]
    [Display(Name = "Zaal Id")]
    public string RoomId { get; set; }

    [Required]
    [Display(Name = "Starttijd")]
    public string StartTime { get; set; }

    // [Display(Name = "Prijs")]
    // public List<Seat> Prices { get; set; }

    [Display(Name = "Weet je zeker dat je deze film wilt toevoegen?")]
    public bool? Ready { get; set; }
  }
}
