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
            List<Aflevering> afleveringen = ArchiefMng.GetAfleveringen(ar => ar.Seizoen == int.Parse(seizoen) && ar.SerieID == int.Parse(serie));

            foreach(var aflevering in afleveringen)
            {
                if (a.Afleveringen.Find(afl => afl.ID == aflevering.ID) == null)
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
            Archief a = ArchiefMng.GetArchief(int.Parse(archief));
            List<Aflevering> afleveringen = ArchiefMng.GetAfleveringen(ar => ar.Seizoen == int.Parse(seizoen) && ar.SerieID == int.Parse(serie));

            foreach (var aflevering in afleveringen)
            {
                a.Afleveringen.RemoveAll(afl => afl.ID == aflevering.ID);
            }

            ArchiefMng.ChangeArchief(a);

            return Ok(true);
        }

        [HttpGet]
        [Route("~/api/Archief/FilmNaarArchief/{archief}/{film}")]
        public IHttpActionResult GetFilmNaarArchief(string archief, string film)
        {
            Archief a = ArchiefMng.GetArchief(int.Parse(archief));
            Film f = ArchiefMng.GetFilm(int.Parse(film));

            if (f != null)
            {
                if (a.Films.FirstOrDefault(flm => flm.ID == f.ID) == null)
                {
                    a.Films.Add(f);
                    ArchiefMng.ChangeArchief(a);
                }
            }

            return Ok(true);
        }

        [HttpGet]
        [Route("~/api/Archief/FilmNaarAndere/{archief}/{film}")]
        public IHttpActionResult GetFilmNaarAndere(string archief, string film)
        {
            Archief a = ArchiefMng.GetArchief(int.Parse(archief));
            Film f = ArchiefMng.GetFilm(int.Parse(film));

            if (f != null)
            {
                a.Films.RemoveAll(flm => flm.ID == f.ID);
                ArchiefMng.ChangeArchief(a);
            }

            return Ok(true);
        }
    }
}