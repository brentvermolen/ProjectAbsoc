using BL.Domain;
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

        public object ReadActeur(int id)
        {
            return repo.GetActeur(id);
        }
    }
}
