using BL.Domain.ActeurKlassen;
using BL.Domain.FilmKlassen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Film
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("title")]
        public string Naam { get; set; }
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty("tagline")]
        public string Tagline { get; set; }
        [JsonProperty("runtime")]
        public int Duur { get; set; }
        [JsonProperty("overview")]
        public string Omschrijving { get; set; }
        public string TrailerId { get; set; }
        public DateTime? Toegevoegd { get; set; }
        [JsonProperty("poster_path")]
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

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
