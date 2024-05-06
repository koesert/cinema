using System.Data.Common;
using Cinema.Data;
using Spectre.Console;

public class LogicLayerVoucher
{
    public static string CodeCheck(CinemaContext db ,string code)
    {
        List<Voucher> vouchers = db.Vouchers.ToList();
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
                        code = CodeCheck(db, Console.ReadLine());
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
    public static int IndexToRemoveCheck(CinemaContext db, int indextoremove)
    {
        List<Voucher> vouchers = db.Vouchers.ToList();
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
    public static double CheckDiscount(string input)
    {
        bool check = false;
        while (!check)
        {
            if (double.TryParse(input, out double number) && number > 0)
            {
                return number;
            }
            AnsiConsole.Markup("[red]Input bevat letters/symbolen of is kleiner dan 0. Probeer opnieuw:\n[/]");
            input = Console.ReadLine();
            Console.Clear();
        }
        return 1;
    }

    public static double CheckPercentDiscount(string input)
    {
        bool check = false;
        while (!check)
        {
            if (double.TryParse(input, out double number) && number <= 100 && number > 0)
            {
                return number;
            }
            AnsiConsole.Markup("[red]Input voldoet niet aan grenswaarden (0-100), of bevat letters/symbolen. Probeer opnieuw:\n[/]");
            input = Console.ReadLine();
            Console.Clear();
        }
        return 1;
    }

    public static string GenerateRandomCode(CinemaContext db)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        
        for (int i = 0; i < 15; i++)
        {
            code += chars[random.Next(chars.Length)];
        }
        
        if (db.Vouchers.ToList().Any(x => x.Code == code))
        {
            return GenerateRandomCode(db);
        }
        return code;
    }
}