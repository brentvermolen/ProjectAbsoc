using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class GebruikerManager
    {
        private readonly GebruikerRepository repo = new GebruikerRepository();

        public Gebruiker GetGebruiker(int id)
        {
            return repo.ReadGebruiker(id);
        }

        public void ChangeGebruiker(Gebruiker gebruiker)
        {
            repo.UpdateGebruiker(gebruiker);
        }

        public bool ValidPostcode(string postcode)
        {
            return repo.ValidPostcode(postcode);
        }

        public List<Gebruiker> GetGebruikers()
        {
            return repo.ReadGebruikers();
        }

        public void DeleteGebruiker(Gebruiker gebruiker)
        {
            repo.RemoveGebruiker(gebruiker);
        }

        public Archief GetArchief(int iD)
        {
            return repo.ReadArchief(iD);
        }

        public void AddLogin(int user)
        {
            repo.CreateLogin(user);
        }
    }
}
