using Sharprompt;

public class PresentReservation
{
    // Add method Start() to confirm reservation and to prompt user for email to put in reservation and after, call PromptNewsletter
    private static void PromptNewsletter(Customer customer)
    {
        while (true)
        {
            string userInput = Prompt.Select("Would you like to subscribe to our Newletter?", new[] { "Yes", "No" });

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