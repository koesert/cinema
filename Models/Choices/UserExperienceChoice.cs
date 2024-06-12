using System.ComponentModel.DataAnnotations;

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
