public class Seat
{
    public char Row { get; set; }
    public int SeatNumber { get; set;}
    public bool IsReserved { get; set; }

    public Seat(char row, int seatNumber)
    {
        Row = row;
        SeatNumber = seatNumber;
        IsReserved = false; // Initially, the seat is not reserved
    }
}