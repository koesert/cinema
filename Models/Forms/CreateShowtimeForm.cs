using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Data;

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
