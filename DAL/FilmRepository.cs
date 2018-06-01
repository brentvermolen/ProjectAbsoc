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

        public List<Film> GetFilms()
        {
            return ctx.Films.ToList();
        }

        public List<Film> GetFilms(Func<Film, bool> predicate)
        {
            return ctx.Films.Where(predicate).ToList();
        }

        public Film GetFilm(int id)
        {
            return ctx.Films.Find(id);
        }

        public void CreateFilm(Film film)
        {
            ctx.Films.Add(film);
            ctx.SaveChanges();
        }

        public void DeleteFilm(int ID)
        {
            ctx.Films.Remove(GetFilm(ID));
        }
    }
}
