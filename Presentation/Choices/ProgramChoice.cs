using System.ComponentModel.DataAnnotations;

public enum ProgramChoice
{
    [Display(Name = "Browse movies & showtimes")]
    ListMovies,

    [Display(Name = "Login")]
    Login,

    [Display(Name = "Exit")]
    Exit
}
