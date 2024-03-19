public class Reservation
{
    public int ReservationId { get; set; }
    public Showtime Showtime { get; set; }
    public Customer Customer { get; set; }
    public string CustomerCode { get; set; }
    public DateTime ReservationTime { get; set; }
    public List<Seat> ReservedSeats { get; set; }

    public Reservation(Showtime showtime, Customer customer, DateTime time, List<Seat> seats)
    {
        Showtime = showtime;
        Customer = customer;
        CustomerCode = GenerateUniqueCode();
        ReservationTime = time;
        ReservedSeats = seats;
        
    }
    public class ReservationSeats
{
    public List<List<Seat>> Seats { get; }

    public ReservationSeats(int rows, int seatsPerRow, string seatingPlan)
    {
        Seats = new List<List<Seat>>();

        InitializeSeats(rows, seatsPerRow);
    }

    private void InitializeSeats(int rows, int seatsPerRow)
    {
        for (char row = 'A'; row <= (char)('A' + rows - 1); row++)
        {
            List<Seat> rowSeats = new List<Seat>();
            for (int seatNumber = 1; seatNumber <= seatsPerRow; seatNumber++)
            {
                rowSeats.Add(new Seat(row, seatNumber));
            }
            Seats.Add(rowSeats);
        }
    }

    public bool ReserveSeat(char row, int seatNumber)
    {
        // Find the seat based on row and number
        Seat seatToReserve = FindSeat(row, seatNumber);

        if (seatToReserve == null || seatToReserve.IsReserved)
        {
            return false; // Seat not found or already reserved
        }

        seatToReserve.IsReserved = true;
        return true; // Reservation successful
    }

    private Seat FindSeat(char row, int seatNumber)
    {
        int rowIndex = row - 'A'; // Convert row char to index
        if (rowIndex < 0 || rowIndex >= Seats.Count)
        {
            return null; // Row index out of bounds
        }

        List<Seat> rowSeats = Seats[rowIndex];
        foreach (Seat seat in rowSeats)
        {
            if (seat.Row == row && seat.SeatNumber == seatNumber)
            {
                return seat;
            }
        }
        return null; // Seat not found in the row
    }
}


    
    // Generates a unique 16-character code for this reservation.
    private string GenerateUniqueCode()
    {
        Random random = new Random();
        string randomChars = string.Join("", Enumerable.Range(0, 16).Select(i => (char)random.Next(48, 123)));
        return randomChars;
    }
}