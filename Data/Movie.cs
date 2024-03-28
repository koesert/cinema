using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public int MinAgeRating { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> Genres { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> Cast { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> Directors { get; set; }
        public virtual ICollection<Showtime> Showtimes { get; set; }
    }
}
