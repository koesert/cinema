class PercentVoucher : Voucher
{
    public int Factor { get; set; }
    public PercentVoucher(string code, int factor) : base(code, 0)
    {
        Factor = factor;
    }
    public override int ApplyDiscount(int price)
    {
        Discount = price * (Factor / 100);
        return base.ApplyDiscount(price);
    }
} 