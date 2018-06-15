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

        public List<Aflevering> GetAfleveringen()
        {
            return ctx.Afleveringen.ToList();
        }

        public List<Aflevering> GetAfleveringen(AfleveringSortEnum toegevoegd, int top)
        {
            switch (toegevoegd)
            {
                default:
                    return ctx.Afleveringen.OrderByDescending(a => a.Toegevoegd).ThenBy(a => a.Nummer).Take(top).ToList();
            }
        }

        public List<Serie> GetSeries(Func<Serie, bool> predicate, SerieSortEnum sort, int maxFilms)
        {
            switch (sort)
            {
                case SerieSortEnum.Aantal_Afleveringen:
                    return ctx.Series.Where(predicate).OrderBy(s => s.Afleveringen.Count).Take(maxFilms).ToList();
                case SerieSortEnum.Aantal_Afleveringen_Desc:
                    return ctx.Series.Where(predicate).OrderByDescending(s => s.Afleveringen.Count).Take(maxFilms).ToList();
                case SerieSortEnum.Naam:
                default:
                    return ctx.Series.Where(predicate).OrderBy(s => s.Naam).Take(maxFilms).ToList();
            }
        }

        public void CreateSerie(Serie serie)
        {
            ctx.Series.Add(serie);
            ctx.SaveChanges();
        }

        public List<Serie> GetSeries(SerieSortEnum sort, int top)
        {
            switch (sort)
            {
                default:
                    return ctx.Series.OrderBy(s => s.Naam).Take(top).ToList();
            }
        }
    }
}
