public class PresentLogin
{
    /// <summary>
    /// This function starts the login process.
    /// </summary>
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
    /// <summary>
    /// This function prompts the user for login credentials and validates them.
    /// If successful, it calls the PresentOptions.Start() function.
    /// </summary>
    public static void PromptLogin()
    {
        bool loginSuccessful;
        string email = "";

        while (email == "")
        {
            Console.WriteLine("Enter your email:");
            email = Console.ReadLine() ?? "";

            bool validEmail;
            validEmail = Login.IsValidEmail(email);

            if (email == "")
            {
                Console.WriteLine("Email must not be empty. Try again");
                continue;
            }
            if (!validEmail)
            {
                Console.WriteLine("Invalid email. Try again");
                email = "";
                continue;
            }

            string password = "";
            while (password == "")
            {
                Console.WriteLine("Enter your password:");
                password = Console.ReadLine() ?? "";

                if (password == "")
                {
                    Console.WriteLine("Password must not be empty. Try again");
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
    /// <summary>
    /// This function informs the user that they are continuing as a guest.
    /// </summary>
    public static void PromptGuest()
    {
        Console.WriteLine("Continuing as a guest.");
        // PresentMovies.Start(); // This line is commented out, because the function PresentMovies.Start() is not implemented yet.
        return;
    }
    /// <summary>
    /// This function informs the user that the program is quitting.
    /// </summary>
    public static void PromptQuit()
    {
        Console.WriteLine("Sad to see you go, have a nice day!");
        return;
    }
}