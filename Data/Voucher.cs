public class Voucher
{
    public string Code { get; set; }
    public double Discount { get; set; }
    public static List<Voucher> Vouchers = new List<Voucher>();
    public Voucher(string code, double discount)
    {
        Code = code;
        Discount = Math.Round(Math.Round(discount, 2) / 0.05) * 0.05;
    }
    public void Remove()
    {
        Vouchers.Remove(this);
    }
    public virtual double ApplyDiscount(double price)
    {
        return price - Discount < 0 ? 0 : price - Discount;
    }
    public override string ToString() => $"Code: '{Code}', Korting {Discount},-";
}