using BL.Domain.ActeurKlassen;
using BL.Domain.FilmKlassen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Film
    {
        [Key]
        public int ID { get; set; }
        public string Naam { get; set; }
        public string ReleaseDate { get; set; }
        public string Tagline { get; set; }
        public int Duur { get; set; }
        public string Omschrijving { get; set; }
        public string TrailerId { get; set; }
        public DateTime? Toegevoegd { get; set; }
        public string PosterPath { get; set; }
        public int CollectieID { get; set; }
        public virtual Collectie Collectie { get; set; }

        public virtual List<Tag> Tags { get; set; }

        public virtual List<ActeurFilm> Acteurs { get; set; }
        public virtual List<Archief> Archieven { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            Film film = (Film)obj;
            if (film.ID == ID)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Naam + " " + ReleaseDate;
        }
    }
}
