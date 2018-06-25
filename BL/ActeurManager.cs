﻿using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ActeurManager
    {
        private readonly ActeurRepository repo = new ActeurRepository();

        public List<Acteur> ReadActeurs()
        {
            return repo.GetActeurs().ToList();
        }

        public Acteur ReadActeur(int id)
        {
            return repo.GetActeur(id);
        }

        public void AddActeur(Acteur a)
        {
            repo.CreateActeur(a);
        }

        public List<Acteur> ReadActeurs(ActeurSortEnum sorteren, int top)
        {
            return repo.GetActeurs(sorteren, top);
        }

        public void RemoveActeur(int iD)
        {
            repo.DeleteActeur(iD);
        }

        public List<Acteur> ReadActeurs(Func<Acteur, bool> predicate, ActeurSortEnum sorteren, int top)
        {
            return repo.GetActeurs(predicate, sorteren, top);
        }
    }
}
