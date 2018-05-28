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
    }
}
