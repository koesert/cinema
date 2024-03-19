using Sharprompt;

public class PresentReservation
{
    public static void PromptNewsletter(Customer customer)
    {
        string userInput = "";
        userInput = Prompt.Select("Would you like to subscribe to our Newletter?", new[] { "Yes", "No"});

        while (userInput == "")
        {
            if (userInput == "No")
            {
                PresentOrder.Start();
                return;
            }
            if (userInput == "")
            {
                Console.WriteLine("Invalid input. Try again.");
                return;
            }
            customer.IsSubscribed = true;
            Console.WriteLine("Thank you for subscribing!");
            PresentOrder.Start();
        }
    }
}