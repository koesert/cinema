using System.Text;
using Cinema.Data;

public class CinemaReservationSystem
{
    public string Name { get; set; }
    public int TotalSeatNum { get; set; }
    public List<List<CinemaSeat>> Seats;
    private Showtime Showtime;
    public CinemaReservationSystem(string name, int totalseatnum, Showtime showtime, CinemaContext db)
    {
        Name = name;
        TotalSeatNum = totalseatnum;
        Showtime = showtime;
        if (totalseatnum == 150)
        {
            Configure150(db);
        }
        else if (totalseatnum == 300)
        {
            Configure300();
        }
        else
        {
            Configure500();
        }
    }
    private void Configure500()
    {
        Seats = new List<List<CinemaSeat>>
        {
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                null,
                new CinemaSeat(1, 5),
                new CinemaSeat(1, 6),
                new CinemaSeat(1, 7),
                new CinemaSeat(1, 8),
                new CinemaSeat(1, 9),
                new CinemaSeat(1, 10),
                new CinemaSeat(1, 11),
                new CinemaSeat(1, 12),
                new CinemaSeat(1, 13),
                new CinemaSeat(1, 14),
                new CinemaSeat(1, 15),
                new CinemaSeat(1, 16),
                new CinemaSeat(1, 17),
                new CinemaSeat(1, 18),
                new CinemaSeat(1, 19),
                new CinemaSeat(1, 20),
                new CinemaSeat(1, 21),
                new CinemaSeat(1, 22),
                new CinemaSeat(1, 23),
                new CinemaSeat(1, 24),
                new CinemaSeat(1, 25),
                new CinemaSeat(1, 26),
                null,
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(2, 4),
                new CinemaSeat(2, 5),
                new CinemaSeat(2, 6),
                new CinemaSeat(2, 7),
                new CinemaSeat(2, 8),
                new CinemaSeat(2, 9),
                new CinemaSeat(2, 10, "orange"),
                new CinemaSeat(2, 11, "orange"),
                new CinemaSeat(2, 12, "orange"),
                new CinemaSeat(2, 13, "orange"),
                new CinemaSeat(2, 14, "orange"),
                new CinemaSeat(2, 15, "orange"),
                new CinemaSeat(2, 16, "orange"),
                new CinemaSeat(2, 17, "orange"),
                new CinemaSeat(2, 18, "orange"),
                new CinemaSeat(2, 19, "orange"),
                new CinemaSeat(2, 20, "orange"),
                new CinemaSeat(2, 21, "orange"),
                new CinemaSeat(2, 22),
                new CinemaSeat(2, 23),
                new CinemaSeat(2, 24),
                new CinemaSeat(2, 25),
                new CinemaSeat(2, 26),
                new CinemaSeat(2, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(3, 4),
                new CinemaSeat(3, 5),
                new CinemaSeat(3, 6),
                new CinemaSeat(3, 7),
                new CinemaSeat(3, 8),
                new CinemaSeat(3, 9, "orange"),
                new CinemaSeat(3, 10, "orange"),
                new CinemaSeat(3, 11, "orange"),
                new CinemaSeat(3, 13, "orange"),
                new CinemaSeat(3, 13, "orange"),
                new CinemaSeat(3, 14, "orange"),
                new CinemaSeat(3, 15, "orange"),
                new CinemaSeat(3, 16, "orange"),
                new CinemaSeat(3, 17, "orange"),
                new CinemaSeat(3, 18, "orange"),
                new CinemaSeat(3, 19, "orange"),
                new CinemaSeat(3, 20, "orange"),
                new CinemaSeat(3, 21, "orange"),
                new CinemaSeat(3, 22, "orange"),
                new CinemaSeat(3, 23),
                new CinemaSeat(3, 24),
                new CinemaSeat(3, 25),
                new CinemaSeat(3, 26),
                new CinemaSeat(3, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(4, 4),
                new CinemaSeat(4, 5),
                new CinemaSeat(4, 6),
                new CinemaSeat(4, 7),
                new CinemaSeat(4, 8),
                new CinemaSeat(4, 9, "orange"),
                new CinemaSeat(4, 10, "orange"),
                new CinemaSeat(4, 11, "orange"),
                new CinemaSeat(4, 12, "orange"),
                new CinemaSeat(4, 13, "orange"),
                new CinemaSeat(4, 14, "orange"),
                new CinemaSeat(4, 15, "orange"),
                new CinemaSeat(4, 16, "orange"),
                new CinemaSeat(4, 17, "orange"),
                new CinemaSeat(4, 18, "orange"),
                new CinemaSeat(4, 19, "orange"),
                new CinemaSeat(4, 20, "orange"),
                new CinemaSeat(4, 21, "orange"),
                new CinemaSeat(4, 22, "orange"),
                new CinemaSeat(4, 23),
                new CinemaSeat(4, 24),
                new CinemaSeat(4, 25),
                new CinemaSeat(4, 26),
                new CinemaSeat(4, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat> {
                null,
                null,
                null,
                new CinemaSeat(5, 4),
                new CinemaSeat(5, 5),
                new CinemaSeat(5, 6),
                new CinemaSeat(5, 7),
                new CinemaSeat(5, 8, "orange"),
                new CinemaSeat(5, 9, "orange"),
                new CinemaSeat(5, 10, "orange"),
                new CinemaSeat(5, 11, "orange"),
                new CinemaSeat(5, 12, "orange"),
                new CinemaSeat(5, 13, "orange"),
                new CinemaSeat(5, 14, "red"),
                new CinemaSeat(5, 15, "red"),
                new CinemaSeat(5, 16, "red"),
                new CinemaSeat(5, 17, "red"),
                new CinemaSeat(5, 18, "orange"),
                new CinemaSeat(5, 19, "orange"),
                new CinemaSeat(5, 20, "orange"),
                new CinemaSeat(5, 21, "orange"),
                new CinemaSeat(5, 22, "orange"),
                new CinemaSeat(5, 23, "orange"),
                new CinemaSeat(5, 24),
                new CinemaSeat(5, 25),
                new CinemaSeat(5, 26),
                new CinemaSeat(5, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat> {
                null,
                null,
                new CinemaSeat(6, 3),
                new CinemaSeat(6, 4),
                new CinemaSeat(6, 5),
                new CinemaSeat(6, 6),
                new CinemaSeat(6, 7),
                new CinemaSeat(6, 8, "orange"),
                new CinemaSeat(6, 9, "orange"),
                new CinemaSeat(6, 10, "orange"),
                new CinemaSeat(6, 11, "orange"),
                new CinemaSeat(6, 12, "orange"),
                new CinemaSeat(6, 13, "red"),
                new CinemaSeat(6, 14, "red"),
                new CinemaSeat(6, 15, "red"),
                new CinemaSeat(6, 16, "red"),
                new CinemaSeat(6, 17, "red"),
                new CinemaSeat(6, 18, "red"),
                new CinemaSeat(6, 19, "orange"),
                new CinemaSeat(6, 20, "orange"),
                new CinemaSeat(6, 21, "orange"),
                new CinemaSeat(6, 22, "orange"),
                new CinemaSeat(6, 23, "orange"),
                new CinemaSeat(6, 24),
                new CinemaSeat(6, 25),
                new CinemaSeat(6, 26),
                new CinemaSeat(6, 27),
                new CinemaSeat(6, 28),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(7, 2),
                new CinemaSeat(7, 3),
                new CinemaSeat(7, 4),
                new CinemaSeat(7, 5),
                new CinemaSeat(7, 6),
                new CinemaSeat(7, 7, "orange"),
                new CinemaSeat(7, 8, "orange"),
                new CinemaSeat(7, 9, "orange"),
                new CinemaSeat(7, 10, "orange"),
                new CinemaSeat(7, 11, "orange"),
                new CinemaSeat(7, 12, "red"),
                new CinemaSeat(7, 13, "red"),
                new CinemaSeat(7, 14, "red"),
                new CinemaSeat(7, 15, "red"),
                new CinemaSeat(7, 16, "red"),
                new CinemaSeat(7, 17, "red"),
                new CinemaSeat(7, 18, "red"),
                new CinemaSeat(7, 19, "red"),
                new CinemaSeat(7, 20, "orange"),
                new CinemaSeat(7, 21, "orange"),
                new CinemaSeat(7, 22, "orange"),
                new CinemaSeat(7, 23, "orange"),
                new CinemaSeat(7, 24, "orange"),
                new CinemaSeat(7, 25),
                new CinemaSeat(7, 26),
                new CinemaSeat(7, 27),
                new CinemaSeat(7, 28),
                new CinemaSeat(7, 29),
                null
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(8, 1),
                new CinemaSeat(8, 2),
                new CinemaSeat(8, 3),
                new CinemaSeat(8, 4),
                new CinemaSeat(8, 5),
                new CinemaSeat(8, 6),
                new CinemaSeat(8, 7, "orange"),
                new CinemaSeat(8, 8, "orange"),
                new CinemaSeat(8, 9, "orange"),
                new CinemaSeat(8, 10, "orange"),
                new CinemaSeat(8, 11, "orange"),
                new CinemaSeat(8, 12, "red"),
                new CinemaSeat(8, 13, "red"),
                new CinemaSeat(8, 14, "red"),
                new CinemaSeat(8, 15, "red"),
                new CinemaSeat(8, 16, "red"),
                new CinemaSeat(8, 17, "red"),
                new CinemaSeat(8, 18, "red"),
                new CinemaSeat(8, 19, "red"),
                new CinemaSeat(8, 20, "orange"),
                new CinemaSeat(8, 21, "orange"),
                new CinemaSeat(8, 22, "orange"),
                new CinemaSeat(8, 23, "orange"),
                new CinemaSeat(8, 24, "orange"),
                new CinemaSeat(8, 25),
                new CinemaSeat(8, 26),
                new CinemaSeat(8, 27),
                new CinemaSeat(8, 28),
                new CinemaSeat(8, 29),
                new CinemaSeat(8, 30)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(9, 1),
                new CinemaSeat(9, 2),
                new CinemaSeat(9, 3),
                new CinemaSeat(9, 4),
                new CinemaSeat(9, 5),
                new CinemaSeat(9, 6, "orange"),
                new CinemaSeat(9, 7, "orange"),
                new CinemaSeat(9, 8, "orange"),
                new CinemaSeat(9, 9, "orange"),
                new CinemaSeat(9, 10, "orange"),
                new CinemaSeat(9, 11, "orange"),
                new CinemaSeat(9, 12, "red"),
                new CinemaSeat(9, 13, "red"),
                new CinemaSeat(9, 14, "red"),
                new CinemaSeat(9, 15, "red"),
                new CinemaSeat(9, 16, "red"),
                new CinemaSeat(9, 17, "red"),
                new CinemaSeat(9, 18, "red"),
                new CinemaSeat(9, 19, "red"),
                new CinemaSeat(9, 20, "orange"),
                new CinemaSeat(9, 21, "orange"),
                new CinemaSeat(9, 22, "orange"),
                new CinemaSeat(9, 23, "orange"),
                new CinemaSeat(9, 24, "orange"),
                new CinemaSeat(9, 25, "orange"),
                new CinemaSeat(9, 26),
                new CinemaSeat(9, 27),
                new CinemaSeat(9, 28),
                new CinemaSeat(9, 29),
                new CinemaSeat(9, 30)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(10, 1),
                new CinemaSeat(10, 2),
                new CinemaSeat(10, 3),
                new CinemaSeat(10, 4),
                new CinemaSeat(10, 5),
                new CinemaSeat(10, 6, "orange"),
                new CinemaSeat(10, 7, "orange"),
                new CinemaSeat(10, 8, "orange"),
                new CinemaSeat(10, 9, "orange"),
                new CinemaSeat(10, 10, "orange"),
                new CinemaSeat(10, 11, "orange"),
                new CinemaSeat(10, 12, "red"),
                new CinemaSeat(10, 13, "red"),
                new CinemaSeat(10, 14, "red"),
                new CinemaSeat(10, 15, "red"),
                new CinemaSeat(10, 16, "red"),
                new CinemaSeat(10, 17, "red"),
                new CinemaSeat(10, 18, "red"),
                new CinemaSeat(10, 19, "red"),
                new CinemaSeat(10, 20, "orange"),
                new CinemaSeat(10, 21, "orange"),
                new CinemaSeat(10, 22, "orange"),
                new CinemaSeat(10, 23, "orange"),
                new CinemaSeat(10, 24, "orange"),
                new CinemaSeat(10, 25, "orange"),
                new CinemaSeat(10, 26),
                new CinemaSeat(10, 27),
                new CinemaSeat(10, 28),
                new CinemaSeat(10, 29),
                new CinemaSeat(10, 30)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(11, 1),
                new CinemaSeat(11, 2),
                new CinemaSeat(11, 3),
                new CinemaSeat(11, 4),
                new CinemaSeat(11, 5),
                new CinemaSeat(11, 6),
                new CinemaSeat(11, 7, "orange"),
                new CinemaSeat(11, 8, "orange"),
                new CinemaSeat(11, 9, "orange"),
                new CinemaSeat(11, 10, "orange"),
                new CinemaSeat(11, 11, "orange"),
                new CinemaSeat(11, 12, "red"),
                new CinemaSeat(11, 13, "red"),
                new CinemaSeat(11, 14, "red"),
                new CinemaSeat(11, 15, "red"),
                new CinemaSeat(11, 16, "red"),
                new CinemaSeat(11, 17, "red"),
                new CinemaSeat(11, 18, "red"),
                new CinemaSeat(11, 19, "red"),
                new CinemaSeat(11, 20, "orange"),
                new CinemaSeat(11, 21, "orange"),
                new CinemaSeat(11, 22, "orange"),
                new CinemaSeat(11, 23, "orange"),
                new CinemaSeat(11, 24, "orange"),
                new CinemaSeat(11, 25),
                new CinemaSeat(11, 26),
                new CinemaSeat(11, 27),
                new CinemaSeat(11, 28),
                new CinemaSeat(11, 29),
                new CinemaSeat(11, 30)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(12, 1),
                new CinemaSeat(12, 2),
                new CinemaSeat(12, 3),
                new CinemaSeat(12, 4),
                new CinemaSeat(12, 5),
                new CinemaSeat(12, 6),
                new CinemaSeat(12, 7),
                new CinemaSeat(12, 8, "orange"),
                new CinemaSeat(12, 9, "orange"),
                new CinemaSeat(12, 10, "orange"),
                new CinemaSeat(12, 11, "orange"),
                new CinemaSeat(12, 12, "red"),
                new CinemaSeat(12, 13, "red"),
                new CinemaSeat(12, 14, "red"),
                new CinemaSeat(12, 15, "red"),
                new CinemaSeat(12, 16, "red"),
                new CinemaSeat(12, 17, "red"),
                new CinemaSeat(12, 18, "red"),
                new CinemaSeat(12, 19, "red"),
                new CinemaSeat(12, 20, "orange"),
                new CinemaSeat(12, 21, "orange"),
                new CinemaSeat(12, 22, "orange"),
                new CinemaSeat(12, 23, "orange"),
                new CinemaSeat(12, 24),
                new CinemaSeat(12, 25),
                new CinemaSeat(12, 26),
                new CinemaSeat(12, 27),
                new CinemaSeat(12, 28),
                new CinemaSeat(12, 29),
                new CinemaSeat(12, 30)
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(13, 2),
                new CinemaSeat(13, 3),
                new CinemaSeat(13, 4),
                new CinemaSeat(13, 5),
                new CinemaSeat(13, 6),
                new CinemaSeat(13, 7),
                new CinemaSeat(13, 8),
                new CinemaSeat(13, 9, "orange"),
                new CinemaSeat(13, 10, "orange"),
                new CinemaSeat(13, 11, "orange"),
                new CinemaSeat(13, 12, "orange"),
                new CinemaSeat(13, 13, "orange"),
                new CinemaSeat(13, 14, "red"),
                new CinemaSeat(13, 15, "red"),
                new CinemaSeat(13, 16, "red"),
                new CinemaSeat(13, 17, "red"),
                new CinemaSeat(13, 18, "orange"),
                new CinemaSeat(13, 19, "orange"),
                new CinemaSeat(13, 20, "orange"),
                new CinemaSeat(13, 21, "orange"),
                new CinemaSeat(13, 22, "orange"),
                new CinemaSeat(13, 23),
                new CinemaSeat(13, 24),
                new CinemaSeat(13, 25),
                new CinemaSeat(13, 26),
                new CinemaSeat(13, 27),
                new CinemaSeat(13, 28),
                new CinemaSeat(13, 29),
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                new CinemaSeat(14, 3),
                new CinemaSeat(14, 4),
                new CinemaSeat(14, 5),
                new CinemaSeat(14, 6),
                new CinemaSeat(14, 7),
                new CinemaSeat(14, 8),
                new CinemaSeat(14, 9, "orange"),
                new CinemaSeat(14, 10, "orange"),
                new CinemaSeat(14, 11, "orange"),
                new CinemaSeat(14, 12, "orange"),
                new CinemaSeat(14, 13, "orange"),
                new CinemaSeat(14, 14, "orange"),
                new CinemaSeat(14, 15, "orange"),
                new CinemaSeat(14, 16, "orange"),
                new CinemaSeat(14, 17, "orange"),
                new CinemaSeat(14, 18, "orange"),
                new CinemaSeat(14, 19, "orange"),
                new CinemaSeat(14, 20, "orange"),
                new CinemaSeat(14, 21, "orange"),
                new CinemaSeat(14, 22, "orange"),
                new CinemaSeat(14, 23),
                new CinemaSeat(14, 24),
                new CinemaSeat(14, 25),
                new CinemaSeat(14, 26),
                new CinemaSeat(14, 27),
                new CinemaSeat(14, 28),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                new CinemaSeat(15, 3),
                new CinemaSeat(15, 4),
                new CinemaSeat(15, 5),
                new CinemaSeat(15, 6),
                new CinemaSeat(15, 7),
                new CinemaSeat(15, 8),
                new CinemaSeat(15, 9),
                new CinemaSeat(15, 10, "orange"),
                new CinemaSeat(15, 11, "orange"),
                new CinemaSeat(15, 12, "orange"),
                new CinemaSeat(15, 13, "orange"),
                new CinemaSeat(15, 14, "orange"),
                new CinemaSeat(15, 15, "orange"),
                new CinemaSeat(15, 16, "orange"),
                new CinemaSeat(15, 17, "orange"),
                new CinemaSeat(15, 18, "orange"),
                new CinemaSeat(15, 19, "orange"),
                new CinemaSeat(15, 20, "orange"),
                new CinemaSeat(15, 21, "orange"),
                new CinemaSeat(15, 22),
                new CinemaSeat(15, 23),
                new CinemaSeat(15, 24),
                new CinemaSeat(15, 25),
                new CinemaSeat(15, 26),
                new CinemaSeat(15, 27),
                new CinemaSeat(15, 28),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(16, 4),
                new CinemaSeat(16, 5),
                new CinemaSeat(16, 6),
                new CinemaSeat(16, 7),
                new CinemaSeat(16, 8),
                new CinemaSeat(16, 9),
                new CinemaSeat(16, 10),
                new CinemaSeat(16, 11, "orange"),
                new CinemaSeat(16, 12, "orange"),
                new CinemaSeat(16, 13, "orange"),
                new CinemaSeat(16, 14, "orange"),
                new CinemaSeat(16, 15, "orange"),
                new CinemaSeat(16, 16, "orange"),
                new CinemaSeat(16, 17, "orange"),
                new CinemaSeat(16, 18, "orange"),
                new CinemaSeat(16, 19, "orange"),
                new CinemaSeat(16, 20, "orange"),
                new CinemaSeat(16, 21),
                new CinemaSeat(16, 22),
                new CinemaSeat(16, 23),
                new CinemaSeat(16, 24),
                new CinemaSeat(16, 25),
                new CinemaSeat(16, 26),
                new CinemaSeat(16, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(17, 4),
                new CinemaSeat(17, 5),
                new CinemaSeat(17, 6),
                new CinemaSeat(17, 7),
                new CinemaSeat(17, 8),
                new CinemaSeat(17, 9),
                new CinemaSeat(17, 10),
                new CinemaSeat(17, 11),
                new CinemaSeat(17, 12),
                new CinemaSeat(17, 13, "orange"),
                new CinemaSeat(17, 14, "orange"),
                new CinemaSeat(17, 15, "orange"),
                new CinemaSeat(17, 16, "orange"),
                new CinemaSeat(17, 17, "orange"),
                new CinemaSeat(17, 18, "orange"),
                new CinemaSeat(17, 19),
                new CinemaSeat(17, 20),
                new CinemaSeat(17, 21),
                new CinemaSeat(17, 22),
                new CinemaSeat(17, 23),
                new CinemaSeat(17, 24),
                new CinemaSeat(17, 25),
                new CinemaSeat(17, 26),
                new CinemaSeat(17, 27),
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                null,
                null,
                new CinemaSeat(18, 6),
                new CinemaSeat(18, 7),
                new CinemaSeat(18, 8),
                new CinemaSeat(18, 9),
                new CinemaSeat(18, 10),
                new CinemaSeat(18, 11),
                new CinemaSeat(18, 12),
                new CinemaSeat(18, 13),
                new CinemaSeat(18, 14),
                new CinemaSeat(18, 15),
                new CinemaSeat(18, 16),
                new CinemaSeat(18, 17),
                new CinemaSeat(18, 18),
                new CinemaSeat(18, 19),
                new CinemaSeat(18, 20),
                new CinemaSeat(18, 21),
                new CinemaSeat(18, 22),
                new CinemaSeat(18, 23),
                new CinemaSeat(18, 24),
                new CinemaSeat(18, 25),
                null,
                null,
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new CinemaSeat(19, 8),
                new CinemaSeat(19, 9),
                new CinemaSeat(19, 10),
                new CinemaSeat(19, 11),
                new CinemaSeat(19, 12),
                new CinemaSeat(19, 13),
                new CinemaSeat(19, 14),
                new CinemaSeat(19, 15),
                new CinemaSeat(19, 16),
                new CinemaSeat(19, 17),
                new CinemaSeat(19, 18),
                new CinemaSeat(19, 19),
                new CinemaSeat(19, 20),
                new CinemaSeat(19, 21),
                new CinemaSeat(19, 22),
                new CinemaSeat(19, 23),
                null,
                null,
                null,
                null,
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new CinemaSeat(20, 9),
                new CinemaSeat(20, 10),
                new CinemaSeat(20, 11),
                new CinemaSeat(20, 12),
                new CinemaSeat(20, 13),
                new CinemaSeat(20, 14),
                new CinemaSeat(20, 15),
                new CinemaSeat(20, 16),
                new CinemaSeat(20, 17),
                new CinemaSeat(20, 18),
                new CinemaSeat(20, 19),
                new CinemaSeat(20, 20),
                new CinemaSeat(20, 21),
                new CinemaSeat(20, 22),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            }
        };
    }
    private void Configure300()
    {
        Seats = new List<List<CinemaSeat>>
        {
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(1, 2),
                new CinemaSeat(1, 3),
                new CinemaSeat(1, 4),
                new CinemaSeat(1, 5),
                new CinemaSeat(1, 6),
                new CinemaSeat(1, 7),
                new CinemaSeat(1, 8),
                new CinemaSeat(1, 9),
                new CinemaSeat(1, 10),
                new CinemaSeat(1, 11),
                new CinemaSeat(1, 12),
                new CinemaSeat(1, 13),
                new CinemaSeat(1, 14),
                new CinemaSeat(1, 15),
                new CinemaSeat(1, 16),
                new CinemaSeat(1, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(2, 2),
                new CinemaSeat(2, 3),
                new CinemaSeat(2, 4),
                new CinemaSeat(2, 5),
                new CinemaSeat(2, 6),
                new CinemaSeat(2, 7, "orange"),
                new CinemaSeat(2, 8, "orange"),
                new CinemaSeat(2, 9, "orange"),
                new CinemaSeat(2, 10, "orange"),
                new CinemaSeat(2, 11, "orange"),
                new CinemaSeat(2, 12, "orange"),
                new CinemaSeat(2, 13),
                new CinemaSeat(2, 14),
                new CinemaSeat(2, 15),
                new CinemaSeat(2, 16),
                new CinemaSeat(2, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(3, 2),
                new CinemaSeat(3, 3),
                new CinemaSeat(3, 4),
                new CinemaSeat(3, 5),
                new CinemaSeat(3, 6, "orange"),
                new CinemaSeat(3, 7, "orange"),
                new CinemaSeat(3, 8, "orange"),
                new CinemaSeat(3, 9, "orange"),
                new CinemaSeat(3, 10, "orange"),
                new CinemaSeat(3, 11, "orange"),
                new CinemaSeat(3, 12, "orange"),
                new CinemaSeat(3, 13, "orange"),
                new CinemaSeat(3, 14),
                new CinemaSeat(3, 15),
                new CinemaSeat(3, 16),
                new CinemaSeat(3, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(4, 2),
                new CinemaSeat(4, 3),
                new CinemaSeat(4, 4),
                new CinemaSeat(4, 5),
                new CinemaSeat(4, 6, "orange"),
                new CinemaSeat(4, 7, "orange"),
                new CinemaSeat(4, 8, "orange"),
                new CinemaSeat(4, 9, "orange"),
                new CinemaSeat(4, 10, "orange"),
                new CinemaSeat(4, 11, "orange"),
                new CinemaSeat(4, 12, "orange"),
                new CinemaSeat(4, 13, "orange"),
                new CinemaSeat(4, 14),
                new CinemaSeat(4, 15),
                new CinemaSeat(4, 16),
                new CinemaSeat(4, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(5, 2),
                new CinemaSeat(5, 3),
                new CinemaSeat(5, 4),
                new CinemaSeat(5, 5, "orange"),
                new CinemaSeat(5, 6, "orange"),
                new CinemaSeat(5, 7, "orange"),
                new CinemaSeat(5, 8, "orange"),
                new CinemaSeat(5, 9, "orange"),
                new CinemaSeat(5, 10, "orange"),
                new CinemaSeat(5, 11, "orange"),
                new CinemaSeat(5, 12, "orange"),
                new CinemaSeat(5, 13, "orange"),
                new CinemaSeat(5, 14, "orange"),
                new CinemaSeat(5, 15),
                new CinemaSeat(5, 16),
                new CinemaSeat(5, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(6, 2),
                new CinemaSeat(6, 3),
                new CinemaSeat(6, 4),
                new CinemaSeat(6, 5, "orange"),
                new CinemaSeat(6, 6, "orange"),
                new CinemaSeat(6, 7, "orange"),
                new CinemaSeat(6, 8, "orange"),
                new CinemaSeat(6, 9, "red"),
                new CinemaSeat(6, 10, "red"),
                new CinemaSeat(6, 11, "orange"),
                new CinemaSeat(6, 12, "orange"),
                new CinemaSeat(6, 13, "orange"),
                new CinemaSeat(6, 14, "orange"),
                new CinemaSeat(6, 15),
                new CinemaSeat(6, 16),
                new CinemaSeat(6, 17),
                null
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(7, 1),
                new CinemaSeat(7, 2),
                new CinemaSeat(7, 3),
                new CinemaSeat(7, 4, "orange"),
                new CinemaSeat(7, 5, "orange"),
                new CinemaSeat(7, 6, "orange"),
                new CinemaSeat(7, 7, "orange"),
                new CinemaSeat(7, 8, "red"),
                new CinemaSeat(7, 9, "red"),
                new CinemaSeat(7, 10, "red"),
                new CinemaSeat(7, 11, "red"),
                new CinemaSeat(7, 12, "orange"),
                new CinemaSeat(7, 13, "orange"),
                new CinemaSeat(7, 14, "orange"),
                new CinemaSeat(7, 15, "orange"),
                new CinemaSeat(7, 16),
                new CinemaSeat(7, 17),
                new CinemaSeat(7, 18)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(8, 1),
                new CinemaSeat(8, 2),
                new CinemaSeat(8, 3),
                new CinemaSeat(8, 4, "orange"),
                new CinemaSeat(8, 5, "orange"),
                new CinemaSeat(8, 6, "orange"),
                new CinemaSeat(8, 7, "red"),
                new CinemaSeat(8, 8, "red"),
                new CinemaSeat(8, 9, "red"),
                new CinemaSeat(8, 10, "red"),
                new CinemaSeat(8, 11, "red"),
                new CinemaSeat(8, 12, "red"),
                new CinemaSeat(8, 13, "orange"),
                new CinemaSeat(8, 14, "orange"),
                new CinemaSeat(8, 15, "orange"),
                new CinemaSeat(8, 16),
                new CinemaSeat(8, 17),
                new CinemaSeat(8, 18)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(9, 1),
                new CinemaSeat(9, 2),
                new CinemaSeat(9, 3, "orange"),
                new CinemaSeat(9, 4, "orange"),
                new CinemaSeat(9, 5, "orange"),
                new CinemaSeat(9, 6, "orange"),
                new CinemaSeat(9, 7, "red"),
                new CinemaSeat(9, 8, "red"),
                new CinemaSeat(9, 9, "red"),
                new CinemaSeat(9, 10, "red"),
                new CinemaSeat(9, 11, "red"),
                new CinemaSeat(9, 12, "red"),
                new CinemaSeat(9, 13, "orange"),
                new CinemaSeat(9, 14, "orange"),
                new CinemaSeat(9, 15, "orange"),
                new CinemaSeat(9, 16, "orange"),
                new CinemaSeat(9, 17),
                new CinemaSeat(9, 18)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(10, 1),
                new CinemaSeat(10, 2),
                new CinemaSeat(10, 3, "orange"),
                new CinemaSeat(10, 4, "orange"),
                new CinemaSeat(10, 5, "orange"),
                new CinemaSeat(10, 6, "orange"),
                new CinemaSeat(10, 7, "red"),
                new CinemaSeat(10, 8, "red"),
                new CinemaSeat(10, 9, "red"),
                new CinemaSeat(10, 10, "red"),
                new CinemaSeat(10, 11, "red"),
                new CinemaSeat(10, 12, "red"),
                new CinemaSeat(10, 13, "orange"),
                new CinemaSeat(10, 14, "orange"),
                new CinemaSeat(10, 15, "orange"),
                new CinemaSeat(10, 16, "orange"),
                new CinemaSeat(10, 17),
                new CinemaSeat(10, 18)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(11, 1),
                new CinemaSeat(11, 2),
                new CinemaSeat(11, 3, "orange"),
                new CinemaSeat(11, 4, "orange"),
                new CinemaSeat(11, 5, "orange"),
                new CinemaSeat(11, 6, "orange"),
                new CinemaSeat(11, 7, "red"),
                new CinemaSeat(11, 8, "red"),
                new CinemaSeat(11, 9, "red"),
                new CinemaSeat(11, 10, "red"),
                new CinemaSeat(11, 11, "red"),
                new CinemaSeat(11, 12, "red"),
                new CinemaSeat(11, 13, "orange"),
                new CinemaSeat(11, 14, "orange"),
                new CinemaSeat(11, 15, "orange"),
                new CinemaSeat(11, 16, "orange"),
                new CinemaSeat(11, 17),
                new CinemaSeat(11, 18)
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(12, 2),
                new CinemaSeat(12, 3),
                new CinemaSeat(12, 4, "orange"),
                new CinemaSeat(12, 5, "orange"),
                new CinemaSeat(12, 6, "orange"),
                new CinemaSeat(12, 7, "orange"),
                new CinemaSeat(12, 8, "red"),
                new CinemaSeat(12, 9, "red"),
                new CinemaSeat(12, 10, "red"),
                new CinemaSeat(12, 11, "red"),
                new CinemaSeat(12, 12, "orange"),
                new CinemaSeat(12, 13, "orange"),
                new CinemaSeat(12, 14, "orange"),
                new CinemaSeat(12, 15, "orange"),
                new CinemaSeat(12, 16),
                new CinemaSeat(12, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(13, 2),
                new CinemaSeat(13, 3),
                new CinemaSeat(13, 4),
                new CinemaSeat(13, 5, "orange"),
                new CinemaSeat(13, 6, "orange"),
                new CinemaSeat(13, 7, "orange"),
                new CinemaSeat(13, 8, "orange"),
                new CinemaSeat(13, 9, "red"),
                new CinemaSeat(13, 10, "red"),
                new CinemaSeat(13, 11, "orange"),
                new CinemaSeat(13, 12, "orange"),
                new CinemaSeat(13, 13, "orange"),
                new CinemaSeat(13, 14, "orange"),
                new CinemaSeat(13, 15),
                new CinemaSeat(13, 16),
                new CinemaSeat(13, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                new CinemaSeat(14, 2),
                new CinemaSeat(14, 3),
                new CinemaSeat(14, 4),
                new CinemaSeat(14, 5),
                new CinemaSeat(14, 6, "orange"),
                new CinemaSeat(14, 7, "orange"),
                new CinemaSeat(14, 8, "orange"),
                new CinemaSeat(14, 9, "orange"),
                new CinemaSeat(14, 10, "orange"),
                new CinemaSeat(14, 11, "orange"),
                new CinemaSeat(14, 12, "orange"),
                new CinemaSeat(14, 13, "orange"),
                new CinemaSeat(14, 14),
                new CinemaSeat(14, 15),
                new CinemaSeat(14, 16),
                new CinemaSeat(14, 17),
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                new CinemaSeat(15, 3),
                new CinemaSeat(15, 4),
                new CinemaSeat(15, 5),
                new CinemaSeat(15, 6),
                new CinemaSeat(15, 7, "orange"),
                new CinemaSeat(15, 8, "orange"),
                new CinemaSeat(15, 9, "orange"),
                new CinemaSeat(15, 10, "orange"),
                new CinemaSeat(15, 11, "orange"),
                new CinemaSeat(15, 12, "orange"),
                new CinemaSeat(15, 13),
                new CinemaSeat(15, 14),
                new CinemaSeat(15, 15),
                new CinemaSeat(15, 16),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                new CinemaSeat(16, 3),
                new CinemaSeat(16, 4),
                new CinemaSeat(16, 5),
                new CinemaSeat(16, 6),
                new CinemaSeat(16, 7, "orange"),
                new CinemaSeat(16, 8, "orange"),
                new CinemaSeat(16, 9, "orange"),
                new CinemaSeat(16, 10, "orange"),
                new CinemaSeat(16, 11, "orange"),
                new CinemaSeat(16, 12, "orange"),
                new CinemaSeat(16, 13),
                new CinemaSeat(16, 14),
                new CinemaSeat(16, 15),
                new CinemaSeat(16, 16),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                new CinemaSeat(17, 3),
                new CinemaSeat(17, 4),
                new CinemaSeat(17, 5),
                new CinemaSeat(17, 6),
                new CinemaSeat(17, 7),
                new CinemaSeat(17, 8),
                new CinemaSeat(17, 9),
                new CinemaSeat(17, 10),
                new CinemaSeat(17, 11),
                new CinemaSeat(17, 12),
                new CinemaSeat(17, 13),
                new CinemaSeat(17, 14),
                new CinemaSeat(17, 15),
                new CinemaSeat(17, 16),
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(18, 4),
                new CinemaSeat(18, 5),
                new CinemaSeat(18, 6),
                new CinemaSeat(18, 7),
                new CinemaSeat(18, 8),
                new CinemaSeat(18, 9),
                new CinemaSeat(18, 10),
                new CinemaSeat(18, 11),
                new CinemaSeat(18, 12),
                new CinemaSeat(18, 13),
                new CinemaSeat(18, 14),
                new CinemaSeat(18, 15),
                null,
                null,
                null
            },
            new List<CinemaSeat>
            {
                null,
                null,
                null,
                new CinemaSeat(19, 4),
                new CinemaSeat(19, 5),
                new CinemaSeat(19, 6),
                new CinemaSeat(19, 7),
                new CinemaSeat(19, 8),
                new CinemaSeat(19, 9),
                new CinemaSeat(19, 10),
                new CinemaSeat(19, 11),
                new CinemaSeat(19, 12),
                new CinemaSeat(19, 13),
                new CinemaSeat(19, 14),
                new CinemaSeat(19, 15),
                null,
                null,
                null
            }
        };
    }
    private void Configure150(CinemaContext db)
    {
        Seats = new List<List<CinemaSeat>>
        {
            new List<CinemaSeat>
            {
                new CinemaSeat(1, 0),
                new CinemaSeat(1, 0),
                new CinemaSeat(1, 3),
                new CinemaSeat(1, 4),
                new CinemaSeat(1, 5),
                new CinemaSeat(1, 6),
                new CinemaSeat(1, 7),
                new CinemaSeat(1, 8),
                new CinemaSeat(1, 9),
                new CinemaSeat(1, 10),
                new CinemaSeat(1, 0),
                new CinemaSeat(1, 0)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(2, 0),
                new CinemaSeat(2, 2),
                new CinemaSeat(2, 3),
                new CinemaSeat(2, 4),
                new CinemaSeat(2, 5),
                new CinemaSeat(2, 6),
                new CinemaSeat(2, 7),
                new CinemaSeat(2, 8),
                new CinemaSeat(2, 9),
                new CinemaSeat(2, 10),
                new CinemaSeat(2, 11),
                new CinemaSeat(2, 0)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(3, 0),
                new CinemaSeat(3, 2),
                new CinemaSeat(3, 3),
                new CinemaSeat(3, 4),
                new CinemaSeat(3, 5),
                new CinemaSeat(3, 6),
                new CinemaSeat(3, 7),
                new CinemaSeat(3, 8),
                new CinemaSeat(3, 9),
                new CinemaSeat(3, 10),
                new CinemaSeat(3, 11),
                new CinemaSeat(3, 0)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(4, 1),
                new CinemaSeat(4, 2),
                new CinemaSeat(4, 3),
                new CinemaSeat(4, 4),
                new CinemaSeat(4, 5),
                new CinemaSeat(4, 6, "orange"),
                new CinemaSeat(4, 7, "orange"),
                new CinemaSeat(4, 8),
                new CinemaSeat(4, 9),
                new CinemaSeat(4, 10),
                new CinemaSeat(4, 11),
                new CinemaSeat(4, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(5, 1),
                new CinemaSeat(5, 2),
                new CinemaSeat(5, 3),
                new CinemaSeat(5, 4),
                new CinemaSeat(5, 5, "orange"),
                new CinemaSeat(5, 6, "orange"),
                new CinemaSeat(5, 7, "orange"),
                new CinemaSeat(5, 8, "orange"),
                new CinemaSeat(5, 9),
                new CinemaSeat(5, 10),
                new CinemaSeat(5, 11),
                new CinemaSeat(5, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(6, 1),
                new CinemaSeat(6, 2),
                new CinemaSeat(6, 3),
                new CinemaSeat(6, 4, "orange"),
                new CinemaSeat(6, 5, "orange"),
                new CinemaSeat(6, 6, "red"),
                new CinemaSeat(6, 7, "red"),
                new CinemaSeat(6, 8, "orange"),
                new CinemaSeat(6, 9, "orange"),
                new CinemaSeat(6, 10),
                new CinemaSeat(6, 11),
                new CinemaSeat(6, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(7, 1),
                new CinemaSeat(7, 2),
                new CinemaSeat(7, 3),
                new CinemaSeat(7, 4, "orange"),
                new CinemaSeat(7, 5, "orange"),
                new CinemaSeat(7, 6, "red"),
                new CinemaSeat(7, 7, "red"),
                new CinemaSeat(7, 8, "orange"),
                new CinemaSeat(7, 9, "orange"),
                new CinemaSeat(7, 10),
                new CinemaSeat(7, 11),
                new CinemaSeat(7, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(8, 1),
                new CinemaSeat(8, 2),
                new CinemaSeat(8, 3),
                new CinemaSeat(8, 4, "orange"),
                new CinemaSeat(8, 5, "orange"),
                new CinemaSeat(8, 6, "red"),
                new CinemaSeat(8, 7, "red"),
                new CinemaSeat(8, 8, "orange"),
                new CinemaSeat(8, 9, "orange"),
                new CinemaSeat(8, 10),
                new CinemaSeat(8, 11),
                new CinemaSeat(8, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(9, 1),
                new CinemaSeat(9, 2),
                new CinemaSeat(9, 3),
                new CinemaSeat(9, 4, "orange"),
                new CinemaSeat(9, 5, "orange"),
                new CinemaSeat(9, 6, "red"),
                new CinemaSeat(9, 7, "red"),
                new CinemaSeat(9, 8, "orange"),
                new CinemaSeat(9, 9, "orange"),
                new CinemaSeat(9, 10),
                new CinemaSeat(9, 11),
                new CinemaSeat(9, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(10, 1),
                new CinemaSeat(10, 2),
                new CinemaSeat(10, 3),
                new CinemaSeat(10, 4),
                new CinemaSeat(10, 5, "orange"),
                new CinemaSeat(10, 6, "orange"),
                new CinemaSeat(10, 7, "orange"),
                new CinemaSeat(10, 8, "orange"),
                new CinemaSeat(10, 9),
                new CinemaSeat(10, 10),
                new CinemaSeat(10, 11),
                new CinemaSeat(10, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(11, 1),
                new CinemaSeat(11, 2),
                new CinemaSeat(11, 3),
                new CinemaSeat(11, 4),
                new CinemaSeat(11, 5),
                new CinemaSeat(11, 6, "orange"),
                new CinemaSeat(11, 7, "orange"),
                new CinemaSeat(11, 8),
                new CinemaSeat(11, 9),
                new CinemaSeat(11, 10),
                new CinemaSeat(11, 11),
                new CinemaSeat(11, 12)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(12, 0),
                new CinemaSeat(12, 2),
                new CinemaSeat(12, 3),
                new CinemaSeat(12, 4),
                new CinemaSeat(12, 5),
                new CinemaSeat(12, 6),
                new CinemaSeat(12, 7),
                new CinemaSeat(12, 8),
                new CinemaSeat(12, 9),
                new CinemaSeat(12, 10),
                new CinemaSeat(12, 11),
                new CinemaSeat(12, 0)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(13, 0),
                new CinemaSeat(13, 0),
                new CinemaSeat(13, 3),
                new CinemaSeat(13, 4),
                new CinemaSeat(13, 5),
                new CinemaSeat(13, 6),
                new CinemaSeat(13, 7),
                new CinemaSeat(13, 8),
                new CinemaSeat(13, 9),
                new CinemaSeat(13, 10),
                new CinemaSeat(13, 0),
                new CinemaSeat(13, 0)
            },
            new List<CinemaSeat>
            {
                new CinemaSeat(14, 0),
                new CinemaSeat(14, 0),
                new CinemaSeat(14, 3),
                new CinemaSeat(14, 4),
                new CinemaSeat(14, 5),
                new CinemaSeat(14, 6),
                new CinemaSeat(14, 7),
                new CinemaSeat(14, 8),
                new CinemaSeat(14, 9),
                new CinemaSeat(14, 10),
                new CinemaSeat(14, 0),
                new CinemaSeat(14, 0)
            }
        };

        foreach (var row in Seats)
        {
            foreach (var seat in row)
            {
                if (seat is null)
                {
                    var cinemaSeat = new CinemaSeat(Showtime);
                    db.CinemaSeats.Add(cinemaSeat);

                }
                else
                {
                    var Row = seat.Row;
                    var SeatNumber = seat.SeatNumber;
                    var Color = seat.Color;
                    var cinemaSeat = new CinemaSeat(Row, SeatNumber, Color, Showtime);
                    db.CinemaSeats.Add(cinemaSeat);


                }

            }

        }
        db.SaveChanges();
    }







    public static void DrawPlan(CinemaContext db, Showtime Showtime)
    {
        Console.Write("  ");
        var highestSeatNumber = db.CinemaSeats
            .Where(s => s.Showtime.Id == Showtime.Id)
            .Max(s => s.SeatNumber) - 1;

        for (int i = 1; i <= highestSeatNumber; i++)
        {
            Console.Write($"{(i < 10 ? i.ToString() + " " : i.ToString())}");
        }
        Console.WriteLine();

        char rowchar = 'A';
        Console.Write(rowchar);
        var showtimes = db.CinemaSeats
            .Where(s => s.Showtime.Id == Showtime.Id)
            .OrderBy(s => s.Row)
            .ThenBy(s => s.SeatNumber)
            .ToList();

        foreach (var seat in showtimes)
        {
            if (seat.Row > rowchar)
            {
                Console.WriteLine();
                rowchar++;
                Console.Write(rowchar);
            }

            if (seat.SeatNumber == 0)
            {
                Console.Write(" ");
            }
            else if (seat.IsReserved)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(seat.Layout);
                Console.ResetColor();
            }
            else if (seat.Color == "orange")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(seat.Layout);
                Console.ResetColor();
            }
            else if (seat.Color == "red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(seat.Layout);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(seat.Layout);
                Console.ResetColor();
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Blauw: 20,-");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("     Geel: 25,-");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("     Rood: 30,-");
        Console.WriteLine();
        Console.ResetColor();
    }
    private void MovieScreenPrint()
    {
        var counts = Seats.Select(innerList => innerList.Count(seat => seat != null)).ToList();
        int len = counts.Max();
        string line = "__";
        string screentext = " ";
        for (int i = 1; i <= counts.Max(); i++)
        {
            line += "_";
            if (i >= 6)
            {
                screentext += " ";
            }
        }
        screentext += "SCHERM";
        Console.WriteLine();
        Console.WriteLine(screentext);
        Console.Write(line);
        Console.WriteLine(line);
    }

    public static CinemaReservationSystem GetCinemaReservationSystem(Showtime showtime, CinemaContext db)
    {
        switch (showtime.RoomId)
        {
            case "1":
                return new CinemaReservationSystem("Zaal 1", 150, showtime, db);
            case "2":
                return new CinemaReservationSystem("Zaal 2", 300, showtime, db);
            case "3":
                return new CinemaReservationSystem("Zaal 3", 500, showtime, db);
            default:

                Console.WriteLine("Ongeldige keuze. Programma afsluiten.");
                Environment.Exit(0);
                return null;
        }
    }

    public static CinemaSeat FindSeat(char row, int seatNumber, Showtime showtime, CinemaContext db)
    {
        var highestRowChar = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id)
        .Max(s => s.Row);

        int highestRow = highestRowChar - 'A' + 1;
        int rowIndex = row - 'A';
        if (rowIndex < 0 || rowIndex >= highestRow)
        {
            return null;
        }

        var seatCheck = db.CinemaSeats
        .Where(s => s.Showtime.Id == showtime.Id && s.Row == row && s.SeatNumber == seatNumber)
        .ToList();

        if (seatCheck.Any() && seatCheck.First().SeatNumber > 0)
        {

            return seatCheck.First();
        }
        else
        {
            return null;
        }
    }

    public static bool ReserveSeat(char row, int seatNumber, Showtime showtime, CinemaContext db)
    {
        CinemaSeat seatToReserve = FindSeat(row, seatNumber, showtime, db);

        if (seatToReserve == null)
        {
            return false;
        }

        if (seatToReserve.IsReserved)
        {
            Console.WriteLine($"Stoel {row}{seatNumber} is al gereserveerd.");
            return false;
        }

        seatToReserve.IsReserved = true;
        return true;
    }
}