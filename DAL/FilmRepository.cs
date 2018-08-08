using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BL.Domain;
using BL.Domain.FilmKlassen;

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
                case FilmSortEnum.Toegevoegd:
                    return ctx.Films.OrderByDescending(f => f.Toegevoegd).Take(top).ToList();
                case FilmSortEnum.Naam:
                default:
                    return ctx.Films.OrderBy(f => f.Naam).Take(top).ToList();
            }
        }

        public List<Aanvraag> GetAanvragen()
        {
            return ctx.Aanvragen.OrderByDescending(a => a.AangevraagOp).ToList();
        }

        public Tag GetTag(string tag)
        {
            Tag t = ctx.Tags.FirstOrDefault(f => f.Naam.ToLower() == tag.ToLower());

            if (t == null)
            {
                t = CreateTag(tag);
            }

            return t;
        }

        public List<Tag> GetTags()
        {
            return ctx.Tags.ToList();
        }

        public Tag CreateTag(string tag)
        {
            Tag t = new Tag()
            {
                Naam = tag
            };
            
            ctx.Tags.Add(t);
            ctx.SaveChanges();

            return t;
        }

        public void DeleteTag(int tag)
        {
            ctx.Tags.Remove(GetTag(tag));
        }

        public Tag GetTag(int id)
        {
            return ctx.Tags.Find(id);
        }

        public void DeleteAanvraag(int id)
        {
            ctx.Aanvragen.Remove(ctx.Aanvragen.Find(id));
            ctx.SaveChanges();
        }

        public void VraagFilmAan(int intId, int gebruiker)
        {
            ctx.Aanvragen.Add(new Aanvraag()
            {
                GebruikerId = gebruiker,
                AangevraagOp = DateTime.Now,
                FilmId = intId
            });

            ctx.SaveChanges();
        }

        public bool IsAangevraagd(int intId)
        {
            if (ctx.Aanvragen.Find(intId) == null)
            {
                return false;
            }

            return true;
        }

        public List<Film> GetFilms(Func<Film, bool> predicate, FilmSortEnum sort, int top)
        {
            switch (sort)
            {
                case FilmSortEnum.Release:
                    return ctx.Films.Where(predicate).OrderBy(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Release_Desc:
                    return ctx.Films.Where(predicate).OrderByDescending(f => f.ReleaseDate).Take(top).ToList();
                case FilmSortEnum.Toegevoegd:
                    return ctx.Films.Where(predicate).OrderByDescending(f => f.Toegevoegd).Take(top).ToList();
                case FilmSortEnum.Naam:
                default:
                    return ctx.Films.Where(predicate).OrderBy(f => f.Naam).Take(top).ToList();
            }
        }

        public void CreateFilm(Film film)
        {
            if (IsAangevraagd(film.ID))
            {
                ctx.Aanvragen.Remove(ctx.Aanvragen.Find(film.ID));
            }

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
