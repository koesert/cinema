public class PresentOptions
{
    public static void Start()
    {
        string userInput = "";
        while (userInput.ToLower() != "quit")
        {
            Console.WriteLine("\nWelcome to the Admin options. Make your choice. 'back' to go back.");
            Console.WriteLine("1. Manage Vouchers");
            Console.WriteLine("2. View Mailing list");
            Console.WriteLine("3. View Statistics");

            userInput = Console.ReadLine() ?? "";

            if (userInput == "1")
            {
                Console.WriteLine("This feature has not been made yet.");
                break;
            }
            else if (userInput == "2")
            {
                Console.WriteLine("This feature has not been made yet.");
                break;
            }
            else if (userInput == "3")
            {
                Console.WriteLine("This feature has not been made yet.");
                break;
            }
            else if (userInput == "back")
            {
                break;
            }
            else
            {
                Console.WriteLine("Not a valid option. Try again.");
            }
        }
    }
}