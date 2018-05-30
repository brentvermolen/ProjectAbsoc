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

        public object GetCollectie(int id)
        {
            return ctx.Collecties.Find(id);
        }
    }
}
