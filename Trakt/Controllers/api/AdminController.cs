using BL;
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
        
        private FilmManager FilmMng = new FilmManager();
        
        [Route("~/api/Admin/AanvraagVerwijderen/{id}")]
        [HttpGet]
        public IHttpActionResult AanvraagVerwijderen(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                if (FilmMng.ReadAanvragen().FirstOrDefault(f => f.FilmId == intId) != null)
                {
                    FilmMng.RemoveAanvraag(intId);
                    return Ok(true);
                }
            }
        
            return NotFound();
        }
    }
}
