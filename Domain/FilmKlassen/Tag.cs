using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain.FilmKlassen
{
    public class Tag
    {
        [Key]
        public int ID { get; set; }
        public string Naam { get; set; }

        public virtual List<Film> Films { get; set; }

        public override string ToString()
        {
            return Naam;
        }
    }
}
