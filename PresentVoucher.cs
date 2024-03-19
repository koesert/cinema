public class PresentVoucher
{
    public static void Start()
    {
        while (true)
        {
            Console.WriteLine("\n1. Make a voucher");
            Console.WriteLine("2. Delete a voucher");
            Console.WriteLine("3. Display vouchers");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MakeVoucher();
                    break;
                case "2":
                    DeleteVoucher();
                    break;
                case "3":
                    DisplayVouchers();
                    break;
                case "4":
                    Console.WriteLine("'Till next time!");
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    public static void MakeVoucher()
    {
        List<Voucher> vouchers = Voucher.Vouchers;
        Console.Write("Voucher code must be atleast 5 characters, with a maximum length of 15 characters. Both letters and/or numbers may be used.\nEnter the voucher code: ");
        string code = LogicLayerVoucher.CodeCheck(Console.ReadLine());
        Console.Write("Choose discount type (% or -): ");
        string discountType = LogicLayerVoucher.DiscountTypeCheck(Console.ReadLine());
        if (discountType == "%")
        {
            Console.Write($"Enter {discountType} discount: ");
            int discount = LogicLayerVoucher.CheckPercentDiscount();
            vouchers.Add(new PercentVoucher(code, discount));
            Console.WriteLine("Voucher created successfully");
        }
        else if (discountType == "-")
        {
            Console.Write($"Enter {discountType} discount: ");
            int discount = LogicLayerVoucher.CheckDiscount();
            vouchers.Add(new Voucher(code, discount));
            Console.WriteLine("Voucher created successfully"); 
        }
    }

    public static void DeleteVoucher()
    {
        List<Voucher> vouchers = Voucher.Vouchers;
        if (vouchers.Count != 0)
        {
            DisplayVouchers();
        }
        Console.Write("Enter the number of the voucher you wish to delete: ");
        int indextoremove = LogicLayerVoucher.IndexToRemoveCheck(Convert.ToInt32(Console.ReadLine()));
        vouchers.Remove(vouchers[indextoremove-1]);
        Console.WriteLine("Voucher deleted successfully");
    }

    public static void DisplayVouchers()
    {
        List<Voucher> vouchers = Voucher.Vouchers;
        if (vouchers.Count == 0)
        {
            Console.WriteLine("Currently no existing vouchers...");
            return;
        }
        Console.WriteLine("Current vouchers:");
        foreach (Voucher voucher in vouchers)
        {
            if (voucher is PercentVoucher percentvoucher)
            {
                Console.WriteLine($"{vouchers.IndexOf(percentvoucher)+1}. Code: '{percentvoucher.Code}', Discount: {percentvoucher.Factor}%");
            }
            else 
            {
                Console.WriteLine($"{vouchers.IndexOf(voucher)+1}. Code: '{voucher.Code}', Discount: ${voucher.Discount}"); 
            }
        }
    }
}

