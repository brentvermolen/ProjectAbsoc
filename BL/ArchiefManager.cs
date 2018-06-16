using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ArchiefManager
    {
        private readonly ArchiefRepository repo = new ArchiefRepository();

        public List<Archief> GetArchieven()
        {
            return repo.ReadArchieven();
        }

        public List<Archief> GetArchievenFromGebruiker(int gebruikerID)
        {
            return repo.ReadArchievenFromGebruiker(gebruikerID);
        }

        public Archief GetArchief(int iD)
        {
            return repo.ReadArchief(iD);
        }

        public List<Aflevering> GetAfleveringen(Func<Aflevering, bool> predicate)
        {
            return repo.ReadAfleveringen(predicate);
        }

        public void ChangeArchief(Archief archief)
        {
            repo.UpdateArchief(archief);
        }

        public Film GetFilm(int film)
        {
            return repo.ReadFilm(film);
        }
    }
}
