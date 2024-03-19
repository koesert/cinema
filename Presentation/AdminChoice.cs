using System.ComponentModel.DataAnnotations;

public enum AdminChoice
{
  [Display(Name = "Manage Vouchers")]
  ManageVouchers,

  [Display(Name = "View Mailing List")]
  MailingList,

  [Display(Name = "View Statistics")]
  Statistics,

  [Display(Name = "Exit")]
  Exit
}