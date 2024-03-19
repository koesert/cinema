public class PresentReservation
{
    public static void PromptNewsletter()
    {
        Console.WriteLine("Would you like to subscribe to our Newletter? 'yes'");
        string userInput = Console.ReadLine() ?? "";

        while (userInput == "")
        {
            if (userInput.ToLower() == "no")
            {
                break;
            }
        }
    }
}