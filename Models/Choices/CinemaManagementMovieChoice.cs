using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
