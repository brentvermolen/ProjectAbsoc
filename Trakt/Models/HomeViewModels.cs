using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class IndexViewModel
    {
        public List<Film> LaatsteFilms { get; set; }
        public List<Film> NieuwsteFilms { get; set; }
        public List<Aflevering> Afleveringen { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }

    public class ZoekenViewModel
    {
        public List<Acteur> Acteurs { get; set; }
        public List<Film> Films { get; set; }
        public List<Serie> Series { get; set; }
        public List<Collectie> Collecties { get; set; }
    }
}