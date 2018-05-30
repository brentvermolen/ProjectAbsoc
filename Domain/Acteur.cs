using BL.Domain.ActeurKlassen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Acteur
    {
        [Key]
        public int ID { get; set; }
        public string Naam { get; set; }

        public string ImagePath { get; set; }

        public virtual List<ActeurFilm> Films { get; set; }
        public virtual List<ActeurSerie> Series { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Acteur))
            {
                return false;
            }

            Acteur a = (Acteur)obj;

            if (a.ID == ID)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Naam;
        }
    }
}
