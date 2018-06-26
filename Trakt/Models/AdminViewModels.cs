using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class AdminViewModel
    {
        public virtual List<Gebruiker> Gebruikers { get; set; }
        public virtual List<Archief> Archieven { get; set; }
        public virtual List<Serie> BestaandeSeries { get; set; }
        public virtual Dictionary<Aanvraag, Film> Aanvragen { get; set; }
    }

    public class ResultatenViewModel
    {
        public List<Serie> Series { get; set; }
        public List<Aflevering> Afleveringen { get; set; }
    }

    //public class WijzigArchiefModel
    //{
    //    public Archief Archief { get; set; }
    //    public List<Film> ArchiefFilms { get; set; }
    //    public Dictionary<string, List<int>> ArchiefSeries { get; set; }
    //    public List<Film> AndereFilms { get; set; }
    //    public Dictionary<string, List<int>> AndereSeries { get; set; }
    //}

    public class WijzigArchiefModel
    {
        public Archief Archief { get; set; }
        public List<Films> Films { get; set; }
        public List<Series> Series { get; set; }
    }

    public class Films
    {
        public Film Film { get; set; }
        public bool StaatOpArchief { get; set; }
    }

    public class Series
    {
        public Serie Serie { get; set; }
        public List<int> SeizoenenArchief { get; set; }
        public List<int> SeizoenenAndere { get; set; }
    }
}