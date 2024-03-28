using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
    public enum InitialStateChoice
    {
        [Display(Name = "Blader door films & vertoningen")]
        ListMovies,

        [Display(Name = "Inloggen")]
        Login,

        [Display(Name = "Afsluiten")]
        Exit
    }
}
