public class Movie
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int Duration { get; set; }
    
    public int Year { get; set; }
    
    public List<string> Genres { get; set; }
    
    public List<string> Cast { get; set; }
    
    public List<string> Directors { get; set; } 

    public bool IsRated16Plus { get; set; } 
}
