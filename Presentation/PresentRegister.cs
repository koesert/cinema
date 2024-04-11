public class PresentRegistration
{
    public static void Start()
    {
        string email = AskEmail();
        string password = AskPassword();

        User.CreateUser(email, password);

        
    }

    public static string AskEmail()
    {
        string ?email = null;
        while(email == null)
        {
            Console.WriteLine("Enter you email:");
            email = Console.ReadLine();
            if (email == null)
            {
                Console.WriteLine("Invalid email, try again.");
                continue;
            }
            if (email == "")
            {
                Console.WriteLine("Invalid email, try again.");
                continue;
            }
            if (!LoginValidity.CheckEmail(email))
            {
                Console.WriteLine("Invalid email, try again.");
                continue;
            }
        }
        return email;
    }

    public static string AskPassword()
    {
        string ?password = null;
        while(password == null)
        {
            Console.WriteLine("Enter you password:");
            password = Console.ReadLine();
            if (password == null)
            {
                Console.WriteLine("Invalid password, try again.");
                continue;
            }
            if (password == "")
            {
                Console.WriteLine("Invalid password, try again.");
                continue;
            }
            if (!LoginValidity.CheckPassword(password))
            {
                Console.WriteLine("Invalid password, try again.");
                continue;
            }
        }
        return password;
    }
}