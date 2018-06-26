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
    public class Collectie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Naam { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        public virtual List<Film> Films { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            Collectie collectie = (Collectie)obj;

            if (collectie.ID == ID)
            {
                return true;
            }

            return false;
        }
    }
}
