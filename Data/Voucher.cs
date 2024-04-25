using System.ComponentModel.DataAnnotations.Schema;
public class Voucher
{
    public int Id { get; set; }
    public string Code { get; set; }
    public double Discount { get; set; }
    public Voucher(string code, double discount)
    {
        Code = code;
        Discount = Math.Round(Math.Round(discount, 2) / 0.05) * 0.05;
    }
    public virtual double ApplyDiscount(double price)
    {
        return price - Discount < 0 ? 0 : price - Discount;
    }
    public override string ToString() => $"Code: '{Code}', Korting {Discount},-";
}