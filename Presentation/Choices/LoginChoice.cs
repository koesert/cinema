using System.ComponentModel.DataAnnotations;

public enum LoginChoice
{
  [Display(Name = "Login as a guest")]
  GuestLogin,

  [Display(Name = "Login as an admin")]
  AdminLogin,

  [Display(Name = "Exit")]
  Exit
}