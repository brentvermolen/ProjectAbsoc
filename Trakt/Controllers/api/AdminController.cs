using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Trakt.Controllers.api
{
    public class AdminController : ApiController
    {
        [Route("~/api/Admin/FilmToevoegen/{naam}")]
        [HttpGet]
        public IHttpActionResult FilmToevoegen(string naam)
        {
            return Ok(true);
        }
    }
}
