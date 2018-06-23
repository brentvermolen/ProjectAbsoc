using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class FilmDetailsViewModel
    {
        public Film Film { get; set; }
        public List<Film> GelijkaardigeFilms { get; set; }
    }
}