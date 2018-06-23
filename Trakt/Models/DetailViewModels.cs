using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class FilmDetailsViewModel
    {
        public Film Film { get; set; }
        public List<Film> GelijkaardigeFilms { get; set; }
    }

    public class ActeurDetailsViewModel
    {
        public Acteur Acteur { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public string Geboorteplaats { get; set; }
        public DateTime? Sterftedatum { get; set; }
        public string Omschrijving { get; set; }
        public List<Film> Films { get; set; }
    }
}