public class PresentLogin
{
    public static void Main()
    {
        string ?userChoice = "";
        bool loginSuccesful = false;
        while(userChoice.ToLower() != "quit")
        {
            Console.WriteLine("Do you wish to login or continue as guest? '+' to create  new superuser.");
            userChoice = Console.ReadLine();
            
            if (userChoice.ToLower() == "login")
            {
                // Ask for details
                Console.WriteLine("Enter your email:");
                string email = Console.ReadLine();
                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine();

                // Try login
                try
                {
                    loginSuccesful = Login.TryLogin(email, password);

                    if (loginSuccesful)
                    {
                        Console.WriteLine("Login successful!");
                        
                    }
                    else
                    {
                        Console.WriteLine("Invalid email or password.");
                    }
                }
                catch (Exception ex) // Catch a general Exception for errors
                {
                    Console.WriteLine("Error during login: {0}", ex.Message);
                }
            }
            else if (userChoice == "+")
            {
                // Ask for details
                Console.WriteLine("Enter your email:");
                string email = Console.ReadLine();
                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine();


                // Try creating superuser
                try
                {
                    Superuser.CreateSuperuser(email, password);
                    Console.WriteLine("Superuser created successfully!");
                }
                catch (Exception ex) // Catch a general Exception for errors
                {
                    Console.WriteLine("Error creating superuser: {0}", ex.Message);
                }

            }
        }
    }
}