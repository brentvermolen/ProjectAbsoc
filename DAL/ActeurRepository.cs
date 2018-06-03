﻿using BL.Domain;
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

        public Acteur GetActeur(int id)
        {
            return ctx.Acteurs.Find(id);
        }

        public void CreateActeur(Acteur a)
        {
            ctx.Acteurs.Add(a);
            ctx.SaveChanges();
        }
    }
}
