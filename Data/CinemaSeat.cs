public class CinemaSeat
{
    public string Layout = " X";
    public char Row;
    public int SeatNumber;
    public string Color;
    public bool IsReserved = false;
    public int Price;
    public CinemaSeat(char row, int seatnumber, string color)
    {
        Row = row;
        SeatNumber = seatnumber;
        Color = color;
        Price = color == "red" ? 30 : color == "orange" ? 25 : 20;
    }
    public CinemaSeat(char row, int seatnumber) : this(row, seatnumber, "blue") { }
    public CinemaSeat(int row, int seatnumber, string color) : this((char)(row - 1 + 'A'), seatnumber, color) { }
    public CinemaSeat(int row, int seatnumber) : this((char)(row - 1 + 'A'), seatnumber) { }
}