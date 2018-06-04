using BL;
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
        
        public IHttpActionResult GetFilm(string id)
        {
            int intId;
            if (int.TryParse(id, out intId))
            {
                return Ok(FilmMng.ReadFilm(intId));
            }

            return NotFound();
        }
    }
}
