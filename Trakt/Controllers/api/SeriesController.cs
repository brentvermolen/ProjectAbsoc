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
    public class SeriesController : ApiController
    {
        private readonly SerieManager SerieMng = new SerieManager();

        [Route("~/api/Series/GetSerie/{id}")]
        public IHttpActionResult GetSerie(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                var serie = SerieMng.ReadSerie(intId);
                var retSerie = new Serie()
                {
                    Naam = serie.Naam,
                    PosterPath = serie.PosterPath,
                    BannerPath = serie.BannerPath,
                    Omschrijving = serie.Omschrijving,
                    AirDate = serie.AirDate,
                    Netwerk = serie.Netwerk
                };

                return Ok(retSerie);
            }

            return NotFound();
        }
    }
}
