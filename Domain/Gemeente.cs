using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Gemeente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Postcode { get; set; }
        public string Plaatsnaam { get; set; }
        public string Deelgemeente { get; set; }
        public string Provincie { get; set; }

        public List<Gebruiker> Gebruikers { get; set; }
    }
}
