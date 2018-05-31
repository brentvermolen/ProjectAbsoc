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
}