using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class FilmViewModel
    {
        public virtual List<Film> Films { get; set; }
        public virtual FilterMax MaxFilms { get; set; }
        public virtual FilmSortEnum Sorteren { get; set; }
        public virtual FilmFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class CollectieViewModel
    {
        public virtual List<Collectie> Collecties { get; set; }
        public virtual FilterMax MaxFilms { get; set; }
        public virtual CollectieSortEnum Sorteren { get; set; }
        public virtual CollectieFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class SerieViewModel
    {
        public virtual List<Serie> Series { get; set; }
        public virtual FilterMax MaxFilms { get; set; }
        public virtual SerieSortEnum Sorteren { get; set; }
        public virtual SerieFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class ActeurViewModel
    {
        public virtual List<Acteur> Acteurs { get; set; }
        public virtual FilterMax MaxFilms { get; set; }
        public virtual ActeurSortEnum Sorteren { get; set; }
        public virtual ActeurFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }

    public enum FilterMax
    {
        Dertig = 30,
        Vijftig = 50,
        Honderd = 100,
        Tweehonderd = 200,
        Duizend = 1000
    }

    public enum FilmFilterOp
    {
        Naam = 0,
        Acteur,
        Jaar
    }

    public enum CollectieFilterOp
    {
        Naam = 0
    }

    public enum SerieFilterOp
    {
        Naam = 0
    }

    public enum ActeurFilterOp
    {
        Naam = 0
    }
}