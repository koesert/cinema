using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class Login
{
    public static bool CheckSuperuserCredentials(string email, string password)
    {
        // Hardcoded email and password for testing
        string hardcodedEmail = "test";
        string hardcodedPassword = "test";

        // Check if the provided email and password match the hardcoded values
        return email == hardcodedEmail && password == hardcodedPassword;
    }

}