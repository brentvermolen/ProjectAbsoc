using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Absoc.Models
{
    public class BoekModel
    {
        [Required]
        [JsonProperty("title")]
        public string Titel { get; set; }
        [Required]
        [JsonProperty("author")]
        public string Auteur { get; set; }
        [Required]
        [JsonProperty("productform")]
        public string Omslag { get; set; }
        [Required]
        [JsonProperty("isbn")]
        public string ISBN { get; set; }
        [Required]
        [JsonProperty("flaptext")]
        public string FlapTekst { get; set; }
        [Required]
        [JsonProperty("image")]
        public string ImageLink { get; set; }

        public bool alInDb { get; set; }
    }
}