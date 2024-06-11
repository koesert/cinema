using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Choices
{
    public enum CinemaManagementChoice
    {
        [Display(Name = "Lijst met momenteel beschikbare films")]
        ListMovies,

        [Display(Name = "Voeg een film toe")]
        AddMovie,

        [Display(Name = "Beheer Vouchers")]
        VoucherPanel,
        [Display(Name = "Bekijk opbrengsten per periode")]
        ViewStats,

        [Display(Name = "Bioscoop Instellingen")]
        Settings,

        [Display(Name = "Bekijk nieuwsbrief abbonees")]
        ViewSubscribers,

        [Display(Name = "Terug")]
        Exit
    }
}
