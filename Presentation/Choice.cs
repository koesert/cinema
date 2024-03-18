using System.ComponentModel.DataAnnotations;

public enum Choice
{
    [Display(Name = "Browse movies & showtimes")]
    ListMovies,

    [Display(Name = "Login")]
    Login,

    [Display(Name = "Exit")]
    Exit
}
