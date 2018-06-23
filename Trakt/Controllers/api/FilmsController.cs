using BL;
using BL.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Trakt.Controllers.api
{
    public class FilmsController : ApiController
    {
        private readonly FilmManager FilmMng = new FilmManager();
        
        [Route("~/api/Films/GetFilm/{id}")]
        public IHttpActionResult GetFilm(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                var film = FilmMng.ReadFilm(intId);
                var retFilm = new Film()
                {
                    Naam = film.Naam,
                    Tagline = film.Tagline,
                    Duur = film.Duur,
                    PosterPath = film.PosterPath,
                    Omschrijving = film.Omschrijving,
                    ReleaseDate = film.ReleaseDate
                };

                return Ok(retFilm);
            }

            return NotFound();
        }

        [Route("~/api/Films/GetFilmAanvraag/{id}")]
        public IHttpActionResult GetFilmAanvraag(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                if (!FilmMng.IsAangevraagd(intId))
                {
                    FilmMng.VraagFilmAan(intId, int.Parse(User.Identity.GetUserId()));
                }

                return Ok(true);
            }

            return NotFound();
        }
    }
}
