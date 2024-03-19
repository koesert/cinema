using System.Linq;
public class LogicLayerVoucher
{
    public static string CodeCheck(string code)
    {
        List<Voucher> vouchers = Voucher.Vouchers;
        bool check = false;
        while (!check)
        {
            if (code.Length >= 5 && code.Length <= 15)
            {
                foreach (Voucher voucher in vouchers)
                {
                    if (voucher.Code == code)
                    {
                        Console.WriteLine("There already appears to be an existing voucher using this code. Please try a different one.");
                        code = CodeCheck(Console.ReadLine());
                        break;
                    }
                }
                return code;
            }
            else
            {
                Console.WriteLine("Code does not appear to match the given restrictions. Please try again:");
                code = Console.ReadLine();
            }
        }
        return "";
    }
    public static string DiscountTypeCheck(string discountType)
    {
        bool check = false;
        while (!check)
        {
            if (discountType == "%" || discountType == "-")
            {
                return discountType;
            }
            Console.WriteLine("Invalid choice. Please try again:");
            discountType = Console.ReadLine();
        }
        return "";
    }
    public static int IndexToRemoveCheck(int indextoremove)
    {
        List<Voucher> vouchers = Voucher.Vouchers;
        bool check = false;
        while (!check)
        {
            if (indextoremove <= vouchers.Count && indextoremove > 0)
            {
                return indextoremove;
            }
            Console.WriteLine("Invalid option. That was not a spot on the list. Please try again:");
            indextoremove = Convert.ToInt32(Console.ReadLine());
        }
        return 1;
    }
    public static int CheckDiscount()
    {
        string input = Console.ReadLine();
        bool check = false;
        while (!check)
        {
            if (int.TryParse(input, out int number))
            {
                return number;
            }
            Console.WriteLine("Number contains non digital characters. Please try again:");
            input = Console.ReadLine();
        }
        return 1;
    }
    public static int CheckPercentDiscount()
    {
        string input = Console.ReadLine();
        bool check = false;
        while (!check)
        {
            if (int.TryParse(input, out int number) && number < 100)
            {
                return number;
            }
            Console.WriteLine("Number exceeds the limit (100), or contains non digital characters. Please try again:");
            input = Console.ReadLine();
        }
        return 1;
    }
}