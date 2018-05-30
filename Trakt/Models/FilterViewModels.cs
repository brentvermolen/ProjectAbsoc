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
        public virtual FilterMaxFilms MaxFilms { get; set; }
        public virtual FilmSorterenOp Sorteren { get; set; }
        public virtual FilmFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class CollectieViewModel
    {
        public virtual List<Collectie> Collecties { get; set; }
        public virtual FilterMaxFilms MaxFilms { get; set; }
        public virtual CollectieSorterenOp Sorteren { get; set; }
        public virtual CollectieFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class SerieViewModel
    {
        public virtual List<Serie> Series { get; set; }
        public virtual FilterMaxFilms MaxFilms { get; set; }
        public virtual SerieSorterenOp Sorteren { get; set; }
        public virtual SerieFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }
    public class ActeurViewModel
    {
        public virtual List<Acteur> Acteurs { get; set; }
        public virtual FilterMaxFilms MaxFilms { get; set; }
        public virtual ActeurSorterenOp Sorteren { get; set; }
        public virtual ActeurFilterOp FilterOp { get; set; }
        public virtual string Filter { get; set; }
    }

    public enum FilterMaxFilms
    {
        Tien = 10,
        Twintig = 20,
        Dertig = 30,
        Vijftig = 50,
        Honderd = 100,
        Tweehonderd = 200
    }

    public enum FilmSorterenOp
    {
        Naam = 0,
        Release,
        Toegevoegd,
        Collectie
    }

    public enum FilmFilterOp
    {
        Naam = 0,
        Acteur,
        Jaar
    }

    public enum CollectieSorterenOp
    {
        Naam = 0
    }

    public enum CollectieFilterOp
    {
        Naam = 0
    }

    public enum SerieSorterenOp
    {
        Naam = 0,
        AantalAfleveringen
    }

    public enum SerieFilterOp
    {
        Naam = 0
    }

    public enum ActeurSorterenOp
    {
        Naam = 0,
        AantalVermeldingen
    }

    public enum ActeurFilterOp
    {
        Naam = 0
    }
}