using System.Text.RegularExpressions;

public static class RegisterValidity
{
    public static bool CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$";

        return Regex.IsMatch(email, emailRegex);
    }
}