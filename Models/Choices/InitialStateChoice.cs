using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.Choices
{
    public enum InitialStateChoice
    {
        ListMovies,
        Login,
        Register,
        reservering,

        [Description("Bioscoop informatie")]
        Information,

        [Description("Afsluiten")]
        Exit
    }
}
