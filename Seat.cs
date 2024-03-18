public class Seat
{
    public int SeatNumber { get; set; }
    public bool IsReserved { get; set; }

    public void ReserveSeat()
    {
        if (!IsReserved)
        {
            IsReserved = true;
            Console.WriteLine("Seat " + SeatNumber + " has been reserved.");
        }
        else
        {
            Console.WriteLine("Seat " + SeatNumber + " is already reserved.");
        }
    }
}
