using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
