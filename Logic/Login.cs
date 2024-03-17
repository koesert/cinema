using System.Text.RegularExpressions;

public class Login
{
    // Static method to attempt login
    public static bool TryLogin(string email, string password)
    {
        // Get the list of superusers
        List<Superuser>? superusers = Superuser.Superusers;

        // Check if the list is null (no superusers created)
        if (superusers == null)
        {
            return false; // Login fails if no superusers exist
        }

        // Loop through each superuser
        foreach (Superuser superuser in superusers)
        {
            // Check email and password match
            if (email == superuser.Email && password == superuser.Password)
            {
                return true; // Login successful
            }
        }

        return false; // Login failed (no matching email and password found)
    }

    // Static method to validate email format
    public static bool IsValidEmail(string email)
    {
        // Regex pattern for email format
        string pattern =
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$";
        Regex regex = new Regex(pattern); // Create a Regex object with the pattern
        return regex.IsMatch(email); // Check if the email matches the pattern
    }

    // Static method to provide feedback on password strength
    private static string GetPasswordFeedback(string password)
    {
        if (password.Length < 8) // Check if password length is less than 8 characters
        {
            return "Password must be at least 8 characters long.";
        }

        bool hasUpper = password.Any(char.IsUpper); // Check if password contains an uppercase letter
        bool hasLower = password.Any(char.IsLower); // Check if password contains a lowercase letter
        bool hasDigit = password.Any(char.IsDigit); // Check if password contains a digit

        string feedback = ""; // Initialize empty string for feedback
        if (!hasUpper || !hasLower || !hasDigit)
        {
            feedback =
                "Password should include an uppercase letter, a lowercase letter, and a number.";
        }
        else
        {
            feedback = "Strong password"; // All complexity requirements met
        }

        return feedback;
    }

    // Static method to validate password complexity
    public static bool IsValidPassword(string password)
    {
        string feedback = GetPasswordFeedback(password); // Get feedback on password strength
        return feedback == "Strong password"; // Password is valid if feedback indicates strong password
    }
}
