using Cinema.Data;
using Cinema.Services;
using Spectre.Console;

public static class PresentCustomerReservationProgress
{

    public static void Start(Customer customer, CinemaContext db)
    {
        PresentAdminOptions.UpdateVouchers(db);
        AnsiConsole.Clear();
        var rule = new Rule("[bold blue]Reservatie Progress:[/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("blue")
        };
        AnsiConsole.Write(rule);
        AnsiConsole.MarkupLine($"[bold blue]Hier kan je bekijken hoeveel reserveringen je verwijderd bent van het behalen van een verassing![/]");
        AnsiConsole.MarkupLine($"[bold blue]Het gaat hier om het aantal reserveringen dat je voltooid hebt binnen 2 maanden![/]");
        AnsiConsole.MarkupLine($"[bold blue]Bereik het aantal binnen 2 maanden en geniet van het voordeel![/]");
        ReservationProgress(customer, db);
        var optionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("")
                .PageSize(10)
                .AddChoices(new[] { "Terug" })
            );
        AnsiConsole.Clear();
        PresentCustomerOptions.Start(customer, db);
    }
    public static void ReservationProgress(Customer customer, CinemaContext db)
    {
        DateTimeOffset today = DateTimeOffset.UtcNow.AddHours(2);
        DateTimeOffset startdate;
        Voucher v;
        if (today.Month % 2 == 1)
        {
            startdate = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, today.Offset);
        }
        else
        {
            startdate = new DateTimeOffset(today.Year, today.Month - 1, 1, 0, 0, 0, today.Offset);
        }
        DateTimeOffset enddate = startdate.AddMonths(2).AddDays(-1);
        double amount = db.Tickets.Where(x => x.CustomerEmail == customer.Email && x.PurchasedAt <= enddate && x.PurchasedAt >= startdate && (x.CancelledAt == null || x.CancelledAt == DateTimeOffset.MinValue)).Count();
        if (amount == 0)
        {
            Voucher vouchersWithReward = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            if (vouchersWithReward != null)
            {
                vouchersWithReward.IsReward = "false";
                db.SaveChanges();
            }

        }
        double max = 10;
        RenderProgressBar(amount / max * 100);
        AnsiConsole.MarkupLine($"[green]{amount} van de {max} reserveringen behaald! (tussen {startdate.ToString("dd-MM-yyyy")} en {enddate.ToString("dd-MM-yyyy")})[/]");
        if (amount < max && db.Vouchers.Any(v => v.IsReward == "true" && v.CustomerEmail == customer.Email))
        {
            Voucher revert = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            revert.Active = false;
            db.SaveChanges();
        }
        if (amount >= max)
        {
            Voucher check = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            if (check != null)
            {
                v = check;
                if (v.Active == false && v.ExpirationDate > today) v.Active = true;
            }
            else
            {
                v = new PercentVoucher(LogicLayerVoucher.GenerateRandomCode(db), 10, DateTimeOffset.UtcNow.AddMonths(6).AddHours(2), customer.Email, "true");
                db.Vouchers.Add(v);
            }
            db.SaveChanges();
            if (!v.Active)
            {
                AnsiConsole.MarkupLine($"[green]Je hebt de verkregen Voucher successvol gebruikt! Tot de volgende campagne![/]");
                return;
            }
            AnsiConsole.MarkupLine($"[green]Maak eenmalig gebruik van de VoucherCode '{v.Code}' om {v.Discount}% korting te krijgen bij je volgende boeking![/]");
            return;
        }
        AnsiConsole.MarkupLine($"[grey]Nog {max - amount} boekingen over tot het behalen van een voordelige kortingscode![/]");
    }

    public static void RenderProgressBar(double percentage)
    {
        // Create a task description
        var taskDescription = "[green]Progress[/]";

        // Render the progress bar using the Spectre.Console Progress API
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new PercentageColumn()          // Percentage
            })
            .Start(ctx =>
            {
                // Add a task with the calculated progress
                var task = ctx.AddTask(taskDescription);
                task.Increment(percentage);
            });
    }

    public static void UpdateTrueProgress(Customer customer, CinemaContext db)
    {
        DateTimeOffset today = DateTimeOffset.UtcNow.AddHours(2);
        DateTimeOffset startdate;
        Voucher v;
        if (today.Month % 2 == 1)
        {
            startdate = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, today.Offset);
        }
        else
        {
            startdate = new DateTimeOffset(today.Year, today.Month - 1, 1, 0, 0, 0, today.Offset);
        }
        DateTimeOffset enddate = startdate.AddMonths(2).AddDays(-1);
        double amount = db.Tickets.Where(x => x.CustomerEmail == customer.Email && x.PurchasedAt <= enddate && x.PurchasedAt >= startdate && (x.CancelledAt == null || x.CancelledAt == DateTimeOffset.MinValue)).Count();
        if (amount == 0)
        {
            Voucher vouchersWithReward = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            if (vouchersWithReward != null)
            {
                vouchersWithReward.IsReward = "false";
                vouchersWithReward.ExpirationDate = DateTimeOffset.UtcNow.AddHours(1);
                db.SaveChanges();
            }

        }
        double max = 10;
        if (amount < max && db.Vouchers.Any(v => v.IsReward == "true" && v.CustomerEmail == customer.Email))
        {
            Voucher revert = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            revert.Active = false;
            db.SaveChanges();
        }
        if (amount >= max)
        {
            Voucher check = db.Vouchers.FirstOrDefault(v => v.IsReward == "true" && v.CustomerEmail == customer.Email);
            if (check != null)
            {
                v = check;
                if (v.Active == false && v.ExpirationDate > today) v.Active = true;
            }
        }
    }
}