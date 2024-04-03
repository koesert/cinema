using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
    public enum CinemaManagementChoice
    {
        [Display(Name = "Lijst met momenteel beschikbare films")]
        ListMovies,

        [Display(Name = "Voeg een film toe")]
        AddMovie,

        [Display(Name = "Terug")]
        Exit
    }
}
