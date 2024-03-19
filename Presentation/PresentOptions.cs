using Sharprompt;

public class PresentOptions
{
    public static void Start()
    {
        ChoiceMethods choiceMethods = new ChoiceMethods();

        AdminChoice currentChoice = AdminChoice.Exit;

        while (currentChoice != AdminChoice.Exit)
        {
            currentChoice = Prompt.Select<AdminChoice>("What would you like to do?");

            switch (currentChoice)
            {
                case AdminChoice.ManageVouchers:
                    Console.WriteLine("This feature has not been made yet.");
                    break;
                case AdminChoice.MailingList:
                    Console.WriteLine("This feature has not been made yet.");
                    break;
                case AdminChoice.Statistics:
                    Console.WriteLine("This feature has not been made yet.");
                    break;
                case AdminChoice.Exit:
                    return;
            }
        }
    }
}