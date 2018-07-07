using BL;
using BL.Domain;
using BL.Domain.ActeurKlassen;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Trakt.Models;

namespace Trakt.Controllers
{
    [Authorize]
    public class AfleveringController : Controller
    {
        private readonly SerieManager SerieMng = new SerieManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Account");
                return;
            }
        }

        public ActionResult Details(int id)
        {
            AfleveringDetailsViewModel model = new AfleveringDetailsViewModel()
            {
                GastActeurs = new List<ActeurSerie>()
            };

            Aflevering aflDb = SerieMng.ReadAflevering(id);

            //if (aflDb != null)
            //{
            //    model.Aflevering = aflDb;
            //    return View(model);
            //}

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/find/{0}?api_key={1}&external_source=tvdb_id", id, ApiKey.MovieDB));
                var obj = JObject.Parse(json).SelectToken("tv_episode_results[0]");

                model.Aflevering = new Aflevering();

                model.Aflevering.Naam = (string)obj.SelectToken("name");
                model.Aflevering.Omschrijving = (string)obj.SelectToken("overview");
                model.Aflevering.ImagePath = "https://image.tmdb.org/t/p/w300" + (string)obj.SelectToken("still_path");
                model.Aflevering.Seizoen = (int)obj.SelectToken("season_number");
                model.Aflevering.Nummer = (int)obj.SelectToken("episode_number");
                model.Aflevering.AirDate = (string)obj.SelectToken("air_date");

                json = client.DownloadString(string.Format("https://api.themoviedb.org/3/tv/{0}?api_key={1}&append_to_response=external_ids", (int)obj.SelectToken("show_id"), ApiKey.MovieDB));
                obj = JObject.Parse(json);

                int tvdb_id = (int)obj.SelectToken("external_ids.tvdb_id");

                if (tvdb_id != 0)
                {
                    Serie serie = SerieMng.ReadSerie(tvdb_id);
                    if (serie != null)
                    {
                        model.Aflevering.Serie = new Serie()
                        {
                            ID = serie.ID,
                            Naam = serie.Naam,
                            PosterPath = serie.PosterPath,
                            BannerPath = serie.BannerPath
                        };
                        model.Aflevering.SerieID = 0;
                    }
                    else
                    {
                        model.Aflevering.Serie = new Serie()
                        {
                            ID = tvdb_id,
                            Naam = (string)obj.SelectToken("original_name"),
                            PosterPath = "https://image.tmdb.org/t/p/w300" + (string)obj.SelectToken("poster_path")
                        };
                        model.Aflevering.SerieID = -1;
                    }
                }

                json = client.DownloadString(string.Format("https://api.themoviedb.org/3/tv/{0}/season/{1}/episode/{2}?api_key={3}", (int)obj.SelectToken("id"), model.Aflevering.Seizoen, model.Aflevering.Nummer, ApiKey.MovieDB));
                obj = JObject.Parse(json);

                foreach (var guest in obj.SelectToken("guest_stars"))
                {
                    var karakter = (string)guest.SelectToken("character");
                    //if (!karakter.Equals(""))
                    //{
                    ActeurSerie acteur = new ActeurSerie()
                    {
                        Karakter = karakter,
                        Acteur = new Acteur() { Naam = (string)guest.SelectToken("name"), ImagePath = "https://image.tmdb.org/t/p/w300" + (string)guest.SelectToken("profile_path") },
                        ActeurID = (int)guest.SelectToken("id")
                    };

                    model.GastActeurs.Add(acteur);
                    //}
                }
            }

            return View(model);
        }
    }
}
