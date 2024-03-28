using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cinema.Data;
public class JsonMovie
{
  public string Title { get; set; }
  public int Duration { get; set; }
  public string Description { get; set; }
  public string ReleaseDate { get; set; }
  public int MinAgeRating { get; set; }

  [Column(TypeName = "jsonb")]
  public List<string> Genres { get; set; }

  [Column(TypeName = "jsonb")]
  public List<string> Cast { get; set; }

  [Column(TypeName = "jsonb")]
  public List<string> Directors { get; set; }
}