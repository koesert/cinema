using System.Text.RegularExpressions;

/// <summary>
/// This class provides static methods for checking login credential validity.
/// </summary>
public class LoginValidity
{
    /// <summary>
    /// Checks if the provided email address is in a valid format.
    /// This method performs basic validation for the presence of "@" symbol
    /// and a dot (.) within the domain part. For more comprehensive validation,
    /// consider using more complex regular expressions.
    /// </summary>
    /// <param name="email">The email address to be validated.</param>
    /// <returns>True if the email is in a valid format, false otherwise.</returns>
    public static bool CheckEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Checks if the provided password meets the minimum length requirement.
    /// This method offers a basic check and does not enforce password complexity.
    /// For more secure password validation, consider using dedicated libraries
    /// or implementing custom logic for complexity checks.
    /// </summary>
    /// <param name="password">The password to be validated.</param>
    /// <returns>True if the password meets the minimum length, false otherwise.</returns>
    public static bool CheckPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        const int minimumLength = 8;
        return password.Length >= minimumLength;
    }
}