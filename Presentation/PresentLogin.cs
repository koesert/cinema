using Sharprompt;

public class PresentLogin
{
    public static void Start()
    {
        LoginChoice currentChoice = LoginChoice.GuestLogin;

        while (true) // Loop until explicitly broken out
        {
            var userInput = Prompt.Select<LoginChoice>("Select an option");

            switch (userInput)
            {
                case LoginChoice.GuestLogin:
                    PromptGuest();
                    break;
                case LoginChoice.AdminLogin:
                    AdminLogin();
                    break;
                case LoginChoice.Exit:
                    return; // Exit the method and the loop
                default:
                    Console.WriteLine("Not a valid option. Try again.");
                    break;
            }
        }
    }

    public static void AdminLogin()
    {
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            var email = Prompt.Input<string>("Enter your email");
            var password = Prompt.Password("Enter your password");

            loginSuccessful = Login.CheckSuperuserCredentials(email, password);

            if (!loginSuccessful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Your credentials do not exist in our database. Try again.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login successful!");
                Console.ResetColor();
                PresentOptions.Start();
                break;
            }
        }
    }
    public static void PromptGuest()
    {
        Console.WriteLine("Functionaliteit moet nog komen voor dit met generated code + email");
        // PresentMovies.Start(); // Call to present movies (implementation not shown)
        return;
    }
}