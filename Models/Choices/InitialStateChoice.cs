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
        [Description("Blader door films & vertoningen")]
        ListMovies,
        [Description("Inloggen")]
        Login,

        [Description("Registreren")]
        Register,

        [Description("Gast-reservering bekijken")]
        reservering,

        [Description("Afsluiten")]
        Exit
    }
}
