public class Superuser // This class represents a superuser account with limited instances
{
    public string Email { get; set; } // Email address of the superuser
    public string Password { get; set; } // Password for the superuser account

    public Superuser(string email, string password) // Constructor to create a new superuser
    {
        Email = email;
        Password = password;
    }
}