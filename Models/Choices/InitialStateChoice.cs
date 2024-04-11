using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Choices
{
    public enum InitialStateChoice
    {
        [Display(Name = "Blader door films & vertoningen")]
        ListMovies,

        [Display(Name = "Inloggen")]
        Login,

        [Display(Name = "Afsluiten")]
        Exit
    }
}
