using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data
{
    public class Hall
    {
        public int Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Seat> Seats { get; set; }
    }
}
