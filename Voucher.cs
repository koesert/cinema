public class Voucher
{
    public string Code { get; set; }
    public int Discount { get; set; }
    public static List<Voucher> Vouchers = new List<Voucher>();
    public Voucher(string code, int discount)
    {
        Code = code;
        Discount = discount;
    }
    public void Remove()
    {
        Vouchers.Remove(this);
    }
    public virtual int ApplyDiscount(int price)
    {
        return price - Discount < 0 ? 0 : price - Discount;
    }
}