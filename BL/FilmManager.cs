using BL.Domain;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class FilmManager
    {
        private readonly FilmRepository repo = new FilmRepository();

        public List<Film> ReadFilms(int top)
        {
            return repo.GetFilms(top);
        }

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

        public void ChangeFilm(Film film)
        {
            repo.UpdateFilm(film);
        }

        public List<Film> ReadFilms(FilmSortEnum sort, int top)
        {
            return repo.GetFilms(sort, top);
        }

        public List<Film> ReadFilms(Func<Film, bool> predicate, FilmSortEnum sort, int top)
        {
            return repo.GetFilms(predicate, sort, top);
        }

        public bool IsAangevraagd(int intId)
        {
            return repo.IsAangevraagd(intId);
        }

        public void VraagFilmAan(int intId, int gebruiker)
        {
            repo.VraagFilmAan(intId, gebruiker);
        }

        public List<Aanvraag> ReadAanvragen()
        {
            return repo.GetAanvragen();
        }
        
        public void RemoveAanvraag(int id)
        {
            repo.DeleteAanvraag(id);
        }
    }
}
