using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class SerieManager
    {
        private readonly SerieRepository repo = new SerieRepository();

        public List<Serie> ReadSeries()
        {
            return repo.GetSeries().ToList();
        }
    }
}
