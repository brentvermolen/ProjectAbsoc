using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class FilmManager
    {
        private readonly FilmRepository repo = new FilmRepository();

        public List<Film> ReadFilms()
        {
            return repo.GetFilms();
        }

        public Film ReadFilm(int id)
        {
            return repo.GetFilm(id);
        }

        public void AddFilm(Film film)
        {
            repo.CreateFilm(film);
        }

        public void RemoveFilm(int ID)
        {
            repo.DeleteFilm(ID);
        }
    }
}
