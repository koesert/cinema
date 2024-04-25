using System.Text.RegularExpressions;

public class RegisterValidity
{
    public static bool CheckEmail(string email)
    {
        const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$";

        return Regex.IsMatch(email, emailRegex);
    }
}