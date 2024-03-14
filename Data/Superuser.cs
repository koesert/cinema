public class Superuser // This class represents a superuser account with limited instances
{
    public string Email { get; set; } // Email address of the superuser
    public string Password { get; set; } // Password for the superuser account

    public static List<Superuser> Superusers = new List<Superuser>(); // Stores a list of all created superuser accounts

    public Superuser(string email, string password) // Constructor to create a new superuser
    {
        Email = email;
        Password = password;
    }

    public static void CreateSuperuser(string email, string password) // Static method to add a new superuser to the list
    => Superusers.Add(new Superuser(email, password));
}