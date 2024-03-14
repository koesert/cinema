public class Superuser
{
    public string Email;
    public string Password;
    public static List<Superuser> ?Superusers;

    public Superuser(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public static void CreateSuperuser(string email, string password) => Superusers.Add(new Superuser(email, password));
}