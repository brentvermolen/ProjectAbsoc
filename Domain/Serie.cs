using BL.Domain.ActeurKlassen;
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
    public class Serie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("seriesid")]
        public int ID { get; set; }
        [JsonProperty("SeriesName")]
        public string Naam { get; set; }
        [JsonProperty("Overview")]
        public string Omschrijving { get; set; }
        [JsonProperty("FirstAired")]
        public string AirDate { get; set; }
        [JsonProperty("Network")]
        public string Netwerk { get; set; }
        
        public string PosterPath { get; set; }
        public string BannerPath { get; set; }
        public string BannerLocation { get; set; }

        public virtual List<ActeurSerie> Acteurs { get; set; }
        public virtual List<Aflevering> Afleveringen { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Serie))
            {
                return false;
            }

            Serie serie = (Serie)obj;
            if (serie.ID == ID)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Naam + " " + AirDate;
        }
    }
}
