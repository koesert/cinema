using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Choices
{
    public enum CinemaManagementVoucherChoice
    {
        [Display(Name = "Maak nieuwe voucher aan")]
        MakeVoucher,

        [Display(Name = "Verwijder een bestaande voucher")]
        DeleteVoucher,

        [Display(Name = "Wijzig een bestaande voucher")]
        ChangeVoucher,

        [Display(Name = "Terug")]
        Exit
    }
}