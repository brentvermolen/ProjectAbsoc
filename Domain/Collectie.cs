using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Collectie
    {
        [Key]
        public int ID { get; set; }
        public string Naam { get; set; }
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
