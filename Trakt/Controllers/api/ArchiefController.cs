using BL;
using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Trakt.Controllers.api
{
    public class ArchiefController : ApiController
    {
        private ArchiefManager ArchiefMng = new ArchiefManager();

        [HttpGet]
        [Route("~/api/Archief/NaarArchief/{archief}/{serie}/{seizoen}")]
        public IHttpActionResult GetNaarArchief(string archief, string serie, string seizoen)
        {
            Archief a = ArchiefMng.GetArchief(int.Parse(archief));
            List<Aflevering> afleveringen = ArchiefMng.GetAfleveringen(ar => ar.Seizoen == int.Parse(seizoen));

            foreach(var aflevering in afleveringen)
            {
                if (a.Afleveringen.Find(afl => afl.ID == aflevering.ID) != null)
                {
                    a.Afleveringen.Add(aflevering);
                }
            }

            ArchiefMng.ChangeArchief(a);

            return Ok(true);
        }

        [HttpGet]
        [Route("~/api/Archief/NaarAndere/{archief}/{serie}/{seizoen}")]
        public IHttpActionResult GetNaarAndere(string archief, string serie, string seizoen)
        {
            return Ok(true);
        }
    }
}