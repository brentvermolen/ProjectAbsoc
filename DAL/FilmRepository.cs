using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public List<Film> GetFilms(int top)
        {
            return ctx.Films.Take(top).ToList();
        }

        public List<Film> GetFilms(Func<Film, bool> predicate)
        {
            return ctx.Films.Where(predicate).ToList();
        }

        public List<Film> GetFilms(Func<Film, bool> predicate, int top)
        {
            return ctx.Films.Where(predicate).Take(top).ToList();
        }

        public Film GetFilm(int id)
        {
            return ctx.Films.Find(id);
        }

        public void UpdateFilm(Film film)
        {
            ctx.Entry(film).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public List<Film> GetFilms(FilmSortEnum sort, int top)
        {
            switch (sort)
            {
                case FilmSortEnum.Release:
                    return ctx.Films.OrderBy(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Release_Desc:
                    return ctx.Films.OrderByDescending(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Collectie:
                    return ctx.Films.OrderBy(f => f.CollectieID).Take(top).ToList();
                case FilmSortEnum.Toegevoegd:
                    return ctx.Films.OrderByDescending(f => f.Toegevoegd).Take(top).ToList();
                case FilmSortEnum.Naam:
                default:
                    return ctx.Films.OrderBy(f => f.Naam).Take(top).ToList();
            }
        }

        public List<Film> GetFilms(Func<Film, bool> predicate, FilmSortEnum sort, int top)
        {
            switch (sort)
            {
                case FilmSortEnum.Release:
                    return ctx.Films.Where(predicate).OrderBy(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Release_Desc:
                    return ctx.Films.Where(predicate).OrderByDescending(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Collectie:
                    return ctx.Films.Where(predicate).OrderBy(f => f.CollectieID).Take(top).ToList();
                case FilmSortEnum.Toegevoegd:
                    return ctx.Films.Where(predicate).OrderByDescending(f => f.Toegevoegd).Take(top).ToList();
                case FilmSortEnum.Naam:
                default:
                    return ctx.Films.Where(predicate).OrderBy(f => f.Naam).Take(top).ToList();
            }
        }

        public void CreateFilm(Film film)
        {
            ctx.Films.Add(film);
            ctx.SaveChanges();
        }

        public void DeleteFilm(int ID)
        {
            ctx.Films.Remove(GetFilm(ID));
            ctx.SaveChanges();
        }
    }
}
