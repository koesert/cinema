using System.ComponentModel.DataAnnotations;

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
