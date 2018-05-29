using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class BoekManager
    {
        private readonly BoekRepository repo = new BoekRepository();

        public void AddBoek(Boek boek)
        {
            repo.CreateBoek(boek);
        }

        public Boek GetBoek(int id)
        {
            return repo.ReadBoek(id);
        }

        public Boek GetBoek(string ISBN)
        {
            return repo.ReadBoek(ISBN);
        }

        public void DeleteBoek(Boek boek)
        {
            repo.RemoveBoek(boek);
        }
    }
}
