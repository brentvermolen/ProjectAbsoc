using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GebruikerRepository
    {
        private readonly AbsocContext ctx = new AbsocContext();

        public Gebruiker ReadGebruiker(int id)
        {
            return ctx.Gebruikers.Find(id);
        }

        public void UpdateGebruiker(Gebruiker gebruiker)
        {
            ctx.Entry(gebruiker).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public bool ValidPostcode(string postcode)
        {
            return ctx.Gemeentes.Find(postcode) != null;
        }
    }
}
