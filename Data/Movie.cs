public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }

    public int Duration { get; set; }

    public int Year { get; set; }

    public List<string> Genres { get; set; }

    public List<string> Cast { get; set; }
}
