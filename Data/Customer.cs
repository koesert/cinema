public class Customer
{
    public static int CustomerId { get; set; }
    public string Email { get; set; }
    public bool IsSubscribed;

    public Customer(string email)
    {
        CustomerId++;
        Email = email;
        IsSubscribed = false;
    }
}