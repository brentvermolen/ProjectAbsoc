using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class AdminViewModel
    {
        public virtual List<Gebruiker> Gebruikers { get; set; }
        public virtual List<Archief> Archieven { get; set; }
    }

    public class ResultatenViewModel
    {
        public List<Serie> Series { get; set; }
        public List<Aflevering> Afleveringen { get; set; }
    }
}