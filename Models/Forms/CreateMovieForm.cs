using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Forms
{
    public class CreateMovieForm
    {
        [Required]
        [Display(Name = "Filmtitel")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Filmduur")]
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Filmomschrijving")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Releasedatum (DD-MM-JJJJ)")]
        public string ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Minimum leeftijdsclassificatie")]
        public int MinAgeRating { get; set; }

        [Required]
        [Display(Name = "Genres")]
        public List<string> Genres { get; set; }

        [Required]
        [Display(Name = "Cast")]
        public List<string> Cast { get; set; }

        [Required]
        [Display(Name = "Regisseurs")]
        public List<string> Directors { get; set; }

        [Display(Name = "Weet je zeker dat je deze film wilt toevoegen?")]
        public bool? Ready { get; set; }
    }
}
