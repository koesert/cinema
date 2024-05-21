using System.ComponentModel.DataAnnotations.Schema;
public class Voucher
{
    public int Id { get; set; }
    public string Code { get; set; }
    public double Discount { get; set; }
    public bool Active { get; set; } = true;
    public string DiscountType { get; set; } = "-";
    public DateTimeOffset ExpirationDate { get; set; }
    public string CustomerEmail { get; set; }
    public string IsReward { get; set; } = "false";
    public Voucher(string code, double discount, DateTimeOffset expirationDate, string customerEmail)
    {
        Code = code;
        Discount = Math.Round(Math.Round(discount / 0.05) * 0.05, 3);
        ExpirationDate = expirationDate;
        CustomerEmail = customerEmail;
    }

    public Voucher(string code, double discount, DateTimeOffset expirationDate, string customerEmail, string isreward) : this(code, discount, expirationDate, customerEmail)
    {
        IsReward = isreward;
    } 
    public virtual double ApplyDiscount(double price)
    {
        if (DiscountType == "-") return price - Discount < 0 ? 0 : price - Discount;
        double olddiscount = Discount;
        Discount = price * (Discount / 100);
        double returnvalue = price - Discount < 0 ? 0 : price - Discount;
        Discount = olddiscount;
        return returnvalue;
    }
    public override string ToString() => DiscountType == "-" ? $"Code: '{Code}', Korting {Discount},-, Vervaldatum: '{ExpirationDate.ToString("dd-MM-yyyy HH:mm")}, Gebonden aan Klant met Email: '{CustomerEmail}'" : $"Code: '{Code}', Korting {Discount}%, Vervaldatum: '{ExpirationDate.ToString("dd-MM-yyyy HH:mm")}, Gebonden aan Klant met Email: '{CustomerEmail}'";
}