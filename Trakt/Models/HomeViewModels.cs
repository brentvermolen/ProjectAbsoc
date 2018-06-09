﻿using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trakt.Models
{
    public class IndexViewModel
    {
        public List<Film> LaatsteFilms { get; set; }
        public List<Film> NieuwsteFilms { get; set; }
        public List<Aflevering> Afleveringen { get; set; }
    }
}