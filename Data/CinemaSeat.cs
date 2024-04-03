public class CinemaSeat
{
    public char Row { get; }
    public int SeatNumber { get; }
    public bool IsReserved { get; set; }

    public CinemaSeat(char row, int seatNumber)
    {
        Row = row;
        SeatNumber = seatNumber;
        IsReserved = false; // Initially, the seat is not reserved
    }
}