using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain.ActeurKlassen
{
    public class ActeurFilm
    {
        [Key]
        [Column("ActeurID", Order = 0)]
        public int ActeurID { get; set; }
        [Key]
        [Column("FilmID", Order = 1)]
        public int FilmID { get; set; }

        public string Karakter { get; set; }
        public int Sort { get; set; }

        public virtual Acteur Acteur { get; set; }
        public virtual Film Film { get; set; }

        public override string ToString()
        {
            return Karakter;
        }
    }
}
