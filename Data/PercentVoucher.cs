class PercentVoucher : Voucher
{
    public PercentVoucher(string code, double discount) : base(code, discount) 
    { 
        DiscountType = "%";

    }
    public override double ApplyDiscount(double price)
    {
        Discount = price * (Discount / 100);
        return base.ApplyDiscount(price);
    }
    public override string ToString() => $"Code: '{Code}', Korting {Discount}%";
} 