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
        public string Naam { get; set; }
    }
}
