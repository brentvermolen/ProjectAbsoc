using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ActeurRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Acteur> GetActeurs()
        {
            return ctx.Acteurs.ToList();
        }

        public object GetActeur(int id)
        {
            return ctx.Acteurs.Find(id);
        }
    }
}
