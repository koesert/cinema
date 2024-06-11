public class PercentVoucher : Voucher
{
    public PercentVoucher(string code, double discount, DateTimeOffset expDate, string customerEmail) : base(code, discount, expDate, customerEmail) 
    { 
        DiscountType = "%";
    }
    public PercentVoucher(string code, double discount, DateTimeOffset expirationDate, string customerEmail, string isreward) : this(code, discount, expirationDate, customerEmail)
    {
        IsReward = isreward;
    } 
    public override double ApplyDiscount(double price)
    {
        double olddiscount = Discount;
        Discount = price * (Discount / 100);
        double returnvalue = base.ApplyDiscount(price);
        Discount = olddiscount;
        return returnvalue;
    }
    public override string ToString() => $"Code: '{Code}', Korting {Discount}%, Vervaldatum: '{ExpirationDate.ToString("dd-MM-yyyy HH:mm")}, Gebonden aan Klant met Email: '{CustomerEmail}'";
} 