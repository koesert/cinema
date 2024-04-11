using System.ComponentModel.DataAnnotations;

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
