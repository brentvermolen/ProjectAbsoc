using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CollectieRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Collectie> GetCollecties()
        {
            return ctx.Collecties.ToList();
        }

        public Collectie GetCollectie(int id)
        {
            return ctx.Collecties.Find(id);
        }

        public List<Collectie> GetCollecties(CollectieSortEnum order, int top)
        {
            switch (order)
            {
                case CollectieSortEnum.Naam:
                default:
                    return ctx.Collecties.OrderBy(c => c.Naam).Take(top).ToList();
            }
        }

        public List<Collectie> GetCollecties(Func<Collectie, bool> predicate, CollectieSortEnum sorteren, int maxFilms)
        {
            switch (sorteren)
            {
                case CollectieSortEnum.Naam:
                default:
                    return ctx.Collecties.Where(predicate).OrderBy(c => c.Naam).Take(maxFilms).ToList();
            }
        }

        public void CreateCollectie(Collectie collectie)
        {
            ctx.Collecties.Add(collectie);
            ctx.SaveChanges();
        }
    }
}
