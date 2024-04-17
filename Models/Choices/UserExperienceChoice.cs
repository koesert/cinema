using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
  public enum UserExperienceChoice
  {
    [Description("Blader door films & vertoningen")]
    ListMovies,

    [Description("Bekijk uw tickets")]
    ViewTickets,
    [Description("Uitloggen")]
    Logout
  }
}
