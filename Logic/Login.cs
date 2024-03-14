using System.Text.RegularExpressions; // Namespace for regular expressions

public class Login
{
    public static bool TryLogin(string email, string password) // Static method to attempt login
    {
        List<Superuser>? superusers = Superuser.Superusers; // Get the list of superusers

        if (superusers == null) // Check if the list is null (no superusers created)
        {
            return false; // Login fails if no superusers exist
        }

        foreach (Superuser superuser in superusers) // Loop through each superuser
        {
            if (email == superuser.Email && password == superuser.Password) // Check email and password match
            {
                return true; // Login successful
            }
        }

        return false; // Login failed (no matching email and password found)
    }

    public static bool IsValidEmail(string email) // Static method to validate email format
    {
        string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$"; // Regex pattern for email format
        Regex regex = new Regex(pattern); // Create a Regex object with the pattern
        return regex.IsMatch(email); // Check if the email matches the pattern
    }

    public static string GetPasswordFeedback(string password) // Static method to provide feedback on password strength
    {
        if (password.Length < 8) // Check if password length is less than 8 characters
        {
            return "Password must be at least 8 characters long.";
        }

        bool hasUpper = password.Any(char.IsUpper); // Check if password contains an uppercase letter
        bool hasLower = password.Any(char.IsLower); // Check if password contains a lowercase letter
        bool hasDigit = password.Any(char.IsDigit); // Check if password contains a digit
        bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c)); // Check if password contains a special symbol

        string feedback = ""; // Initialize empty string for feedback
        if (!hasUpper) // Check for missing uppercase letter
        {
            feedback += "Password should include an uppercase letter. ";
        }
        else if (!hasLower) // Check for missing lowercase letter (else if for efficiency)
        {
            feedback += "Password should include a lowercase letter. ";
        }
        else if (!hasDigit) // Check for missing digit (else if for efficiency)
        {
            feedback += "Password should include a number. ";
        }
        else if (!hasSymbol) // Check for missing symbol (else if for efficiency)
        {
            feedback += "Password should include a special symbol.";
        }
        else if (hasUpper && hasLower && hasDigit && hasSymbol) // All complexity requirements met
        {
            feedback = "Strong password";
        }

        return feedback.Trim(); // Remove trailing space from feedback
    }

    public static bool IsValidPassword(string password) // Static method to validate password complexity
    {
        string feedback = GetPasswordFeedback(password); // Get feedback on password strength
        return feedback == "Strong password"; // Password is valid if feedback indicates strong password
    }
}