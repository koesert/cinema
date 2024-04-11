using System.ComponentModel.DataAnnotations.Schema;
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