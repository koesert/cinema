using System.ComponentModel.DataAnnotations.Schema;
using Cinema.Data;

public class CinemaSeat
{
    public int Id { get; set; }
    public string Layout { get; set; }
    public char Row { get; set; }
    public int SeatNumber { get; set; }
    public string Color { get; set; }
    public bool IsReserved { get; set; } = false;
    public decimal Price { get; set; }
    public Showtime Showtime { get; set; }
    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public bool IsSelected { get; set; } = false;
    public int Type { get; set; } = 0;
    public string ReservedBy { get; internal set; }
    public CinemaSeat() { }
    public CinemaSeat(char row, int seatnumber, string color)
    {
        Layout = " X";
        Row = row;
        SeatNumber = seatnumber;
        Color = color;
        Price = color == "red" ? 30 : color == "orange" ? 25 : 20;
    }
    public CinemaSeat(char row, int seatnumber, string color, int type) : this(row, seatnumber, color)
    {
        Type = type;
        if (Type == 1)
        {
            Price += 5;
            Layout = " E";
        }
        if (Type == 2)
        {
            Price = Price * 2;
            Layout = " L";
        }
    }

    public CinemaSeat(char row, int seatnumber) : this(row, seatnumber, "blue") { }
    public CinemaSeat(char row, int seatnumber, int type) : this(row, seatnumber, "blue", type) { }
    public CinemaSeat(int row, int seatnumber, string color) : this((char)(row - 1 + 'A'), seatnumber, color) { }
    public CinemaSeat(int row, int seatnumber, int type) : this((char)(row - 1 + 'A'), seatnumber, "blue", type) { }
    public CinemaSeat(int row, int seatnumber) : this((char)(row - 1 + 'A'), seatnumber) { }
    public CinemaSeat(int row, int seatnumber, string color, int type) : this((char)(row - 1 + 'A'), seatnumber, color, type) { }

    public CinemaSeat(char row, int seatnumber, string color, int type, Showtime showtime) : this(row, seatnumber, color, type)
    {
        Showtime = showtime;
    }
}