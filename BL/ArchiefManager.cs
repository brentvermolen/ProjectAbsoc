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
    }
}
