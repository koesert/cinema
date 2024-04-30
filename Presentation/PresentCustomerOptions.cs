using Cinema.Data;
using Spectre.Console;

public class PresentCustomerOptions
{
    public static void Start(Customer loggedInCustomer)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("Dit zijn de customer options");
        Console.ReadLine();
    }
}