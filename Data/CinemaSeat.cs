using Cinema.Data;

public class CinemaSeat
{
    public int Id { get; set; }
    public string Layout { get; set; }
    public char Row { get; set; }
    public int SeatNumber { get; set; }
    public string Color { get; set; }
    public bool IsReserved { get; set; } = false;
    public int Price { get; set; }
    public Showtime Showtime { get; set; }
    public CinemaSeat() { }
    public CinemaSeat(char row, int seatnumber, string color)
    {
        Layout = " X";
        Row = row;
        SeatNumber = seatnumber;
        Color = color;
        Price = color == "red" ? 30 : color == "orange" ? 25 : 20;
    }
    public CinemaSeat(char row, int seatnumber) : this(row, seatnumber, "blue") { }
    public CinemaSeat(int row, int seatnumber, string color) : this((char)(row - 1 + 'A'), seatnumber, color) { }
    public CinemaSeat(int row, int seatnumber) : this((char)(row - 1 + 'A'), seatnumber) { }
}