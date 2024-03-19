using Sharprompt;

public class PresentReservation
{
    public static void PromptNewsletter(Customer customer)
    {
        string userInput = "";
        while (true)
        {
            userInput = Prompt.Select("Would you like to subscribe to our Newletter?", new[] { "Yes", "No" });

            if (userInput == "No")
            {
                PresentOrder.Start();
                break;
            }
            customer.IsSubscribed = true;
            Console.WriteLine("Thank you for subscribing!");
            PresentOrder.Start();
            break;
        }
    }
}