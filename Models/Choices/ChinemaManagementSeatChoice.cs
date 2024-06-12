using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Choices
{
  public enum CinemaManagementSeatChoice
  {
    [Display(Name = "Regular")]
    Regular,

    [Display(Name = "Love Seat")]
    LoveSeat,

    [Display(Name = "Relax Seat")]
    RelaxSeat,

  }
}
