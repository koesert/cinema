using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
    public enum CinemaManagementAddMovieChoice
    {
        [Display(Name = "Voeg film handmatig toe")]
        AddMovieManually,

        [Display(Name = "Voeg film(s) toe door JSON-bestand te laden")]
        AddMovieJSON,

        [Display(Name = "Terug")]
        Exit
    }
}
