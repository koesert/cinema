public class Customer
{
    public int ID;
    public string Email { get; set; }
    public bool IsSubscribed;

    public Customer(string email)
    {
        ID++;
        Email = email;
        IsSubscribed = false;
    }
}