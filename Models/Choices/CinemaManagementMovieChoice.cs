using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Choices
{
  public enum CinemaManagementMovieChoice
  {
    [Display(Name = "Lijst met aankomende vertoningen voor deze film")]
    ListShowtimes,

    [Display(Name = "Verwijder deze film")]
    DeleteMovie,

    [Display(Name = "Voeg een vertoningstijd toe voor deze film")]
    AddShowtime,

    [Display(Name = "Terug")]
    Exit
  }
}
