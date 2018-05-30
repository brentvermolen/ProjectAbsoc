using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Domain;

namespace DAL
{
    public class FilmRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Film> ReadFilms(Func<Film, bool> predicate)
        {
            return ctx.Films.Where(predicate).ToList();
        }
    }
}
