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

        public Aflevering ReadAflevering(int id)
        {
            return repo.GetAflevering(id);
        }

        public Serie ReadSerie(int id)
        {
            return repo.GetSerie(id);
        }

        public List<Aflevering> ReadAfleveringen()
        {
            return repo.GetAfleveringen();
        }

        public List<Aflevering> ReadAfleveringen(AfleveringSortEnum toegevoegd, int top)
        {
            return repo.GetAfleveringen(toegevoegd, top);
        }

        public List<Serie> ReadSeries(SerieSortEnum sort, int top)
        {
            return repo.GetSeries(sort, top);
        }

        public List<Serie> ReadSeries(Func<Serie, bool> wherePred, SerieSortEnum sort, int maxFilms)
        {
            return repo.GetSeries(wherePred, sort, maxFilms);
        }

        public void AddSerie(Serie serie)
        {
            throw new NotImplementedException();
        }
    }
}
