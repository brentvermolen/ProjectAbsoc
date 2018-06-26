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

        public Dictionary<Acteur, bool> Acteurs { get; set; }

        public bool IsActeurInDb(int id)
        {
            foreach (var acteur in Acteurs)
            {
                if (acteur.Key.ID == id)
                {
                    return acteur.Value;
                }
            }

            return false;
        }
    }

    public class ActeurDetailsViewModel
    {
        public Acteur Acteur { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public string Geboorteplaats { get; set; }
        public DateTime? Sterftedatum { get; set; }
        public string Omschrijving { get; set; }
        public List<Film> Films { get; set; }
        public List<Serie> Series { get; set; }
    }

    public class SerieDetailsViewModel
    {
        public Serie Serie { get; set; }
        public Dictionary<Acteur, bool> Acteurs { get; set; }

        public bool IsActeurInDb(int id)
        {
            foreach (var acteur in Acteurs)
            {
                if (acteur.Key.ID == id)
                {
                    return acteur.Value;
                }
            }

            return false;
        }
    }

    public class CollectieDetailsViewModel
    {
        public Collectie Collectie { get; set; }
        public List<Film> Films { get; set; }
    }
}