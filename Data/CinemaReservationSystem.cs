public class CinemaReservationSystem
{
    public List<List<CinemaSeat>> Seats { get; }

    public CinemaReservationSystem(int rows, int seatsPerRow, string seatingPlan)
    {
        Seats = new List<List<CinemaSeat>>();

        InitializeSeats(rows, seatsPerRow);
    }

    private void InitializeSeats(int rows, int seatsPerRow)
    {
        for (char row = 'A'; row <= (char)('A' + rows - 1); row++)
        {
            List<CinemaSeat> rowSeats = new List<CinemaSeat>();
            for (int seatNumber = 1; seatNumber <= seatsPerRow; seatNumber++)
            {
                rowSeats.Add(new CinemaSeat(row, seatNumber));
            }
            Seats.Add(rowSeats);
        }
    }

    public bool ReserveSeat(char row, int seatNumber)
    {
        CinemaSeat seatToReserve = FindSeat(row, seatNumber);

        if (seatToReserve == null)
        {
            return false; // Seat not found
        }

        if (seatToReserve.IsReserved)
        {
            Console.WriteLine($"Stoel {row}{seatNumber} is al gereserveerd.");
            return false; // Seat already reserved
        }

        seatToReserve.IsReserved = true;
        return true; // Reservation successful
    }



    public CinemaSeat FindSeat(char row, int seatNumber)
    {
        int rowIndex = row - 'A';
        if (rowIndex < 0 || rowIndex >= Seats.Count)
        {
            return null;
        }

        List<CinemaSeat> rowSeats = Seats[rowIndex];
        foreach (CinemaSeat seat in rowSeats)
        {
            if (seat.Row == row && seat.SeatNumber == seatNumber)
            {
                return seat;
            }
        }
        return null;
    }

    public static CinemaReservationSystem GetCinemaReservationSystem(string choice)
    {
        switch (choice)
        {
            case "1":
                return new CinemaReservationSystem(14, 12, "Zaal150");
            case "2":
                return new CinemaReservationSystem(19, 18, "Zaal300");
            case "3":
                return new CinemaReservationSystem(20, 30, "Zaal500");
            default:

                Console.WriteLine("Ongeldige keuze. Programma afsluiten.");
                Environment.Exit(0);
                return null;
        }
    }


    public static void DrawPlan(CinemaReservationSystem cinemaSystem)
    {
        foreach (List<CinemaSeat> rowSeats in cinemaSystem.Seats)
        {
            foreach (CinemaSeat seat in rowSeats)
            {
                if (seat.IsReserved)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[X]");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("[O]");
                }
            }
            Console.WriteLine(); // Move to the next line
        }
    }
}
