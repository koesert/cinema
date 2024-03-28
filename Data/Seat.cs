using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data
{
    public class Seat
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public bool IsReserved { get; set; }
    }
}
