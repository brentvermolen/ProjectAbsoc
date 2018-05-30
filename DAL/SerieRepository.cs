using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Domain;

namespace DAL
{
    public class SerieRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Aflevering> GetAfleveringen(Func<Aflevering, bool> predicate)
        {
            return ctx.Afleveringen.Where(predicate).ToList();
        }

        public List<Serie> GetSeries()
        {
            return ctx.Series.ToList();
        }

        public Aflevering GetAflevering(int id)
        {
            return ctx.Afleveringen.Find(id);
        }

        public Serie GetSerie(int id)
        {
            return ctx.Series.Find(id);
        }
    }
}
