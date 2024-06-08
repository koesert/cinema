using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
  public enum UserExperienceChoice
  {
    [Display(Name = "Blader door films & vertoningen")]
    ListMovies,

    [Display(Name = "Bekijk uw tickets")]
    ViewTickets,

    [Display(Name = "Uitloggen")]
    Logout
  }
}
