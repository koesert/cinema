public class User
{
    public static int ID { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public static List<User> Users{ get; private set; }

    public User(string email, string password)
    {
        Email = email;
        Password = password;
        ID++;
    }

    public static User FindUser(string email, string password)
    {
        foreach (User existingUser in Users)
        {
            if (existingUser.Email == email && existingUser.Password == password)
            {
                return existingUser;
            }
            return null;
        }
        return null;
    }

    public static void CreateUser(string email, string password)
    {
        User newUser = new User(email, password);
        Users.Add(newUser);
    }

    public void EditEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void EditPassword(string newPassword)
    {
        Password = newPassword;
    }
}