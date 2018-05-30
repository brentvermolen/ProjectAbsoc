using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BoekRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public Boek ReadBoek(int id)
        {
            return ctx.Boeken.Find(id);
        }

        public Boek ReadBoek(string ISBN)
        {
            return ctx.Boeken.FirstOrDefault(b => b.ISBN.Equals(ISBN));
        }

        public void CreateBoek(Boek boek)
        {
            ctx.Boeken.Add(boek);
            ctx.SaveChanges();
        }

        public void RemoveBoek(Boek boek)
        {
            ctx.Boeken.Remove(boek);
            ctx.SaveChanges();
        }
    }
}
