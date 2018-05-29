using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Boek
    {
        [Key]
        public int ID { get; set; }
        public string Titel { get; set; }
        public string Auteur { get; set; }
        public string Omslag { get; set; }
        public string ISBN { get; set; }
        public string FlapTekst { get; set; }
        
        public string ImageLink { get; set; }
    }
}
