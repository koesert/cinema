public class PresentLogin
{
    public static void Start()
    {
        string userInput = "";
        while (userInput.ToLower() != "quit")
        {
            Console.WriteLine("\nDo you wish to login or continue as guest? 'quit' to quit.");
            userInput = Console.ReadLine() ?? "";

            switch (userInput.ToLower())
            {
                case "login":
                    PromptLogin();
                    break;
                case "guest":
                    PromptGuest();
                    break;
                case "quit":
                    PromptQuit();
                    break;
                default:
                    Console.WriteLine("Not a valid option. Try again.");
                    break;
            }
        }
    }
    public static void PromptLogin()
    {
        bool loginSuccessful;
        string email = "";

        while (email == "")
        {
            Console.WriteLine("Enter your email:");
            email = Console.ReadLine() ?? "";
            if (email == "")
            {
                Console.WriteLine("Invalid email. Try again");
                continue;
            }
            string password = "";
            while (password == "")
            {
                Console.WriteLine("Enter your password:");
                password = Console.ReadLine() ?? "";
                if (password == "")
                {
                    Console.WriteLine("Invalid password. Try again");
                    continue;
                }
                loginSuccessful = Login.CheckSuperuserCredentials(email, password);
                if (!loginSuccessful)
                {
                    Console.WriteLine("Your credentials do not exist in our database. Try again.");
                    continue;
                }
                Console.WriteLine("Login succesful!");
                PresentOptions.Start();
            }
        }
    }
    public static void PromptGuest()
    {
        Console.WriteLine("Continuing as a guest.");
        // PresentMovies.Start(); // Call to present movies (implementation not shown)
        return;
    }
    public static void PromptQuit()
    {
        Console.WriteLine("Sad to see you go, have a nice day!");
        return;
    }
}