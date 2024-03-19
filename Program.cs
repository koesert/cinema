public class Program
{
    static void Main()
    {
        Customer customer1 = new Customer("email@google.com");
        Customer customer2 = new Customer("email@google.com");
        PresentReservation.PromptNewsletter(customer1);
        Console.WriteLine(customer1.ID);
    }
}
