using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            AfleveringDetailsViewModel model = new AfleveringDetailsViewModel();
            
            Aflevering aflDb = SerieMng.ReadAflevering(id);
                       
            if (aflDb != null)
            {
                model.Aflevering = aflDb;
                return View(model);
            }
                        
            using(WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/find/{0}?api_key={1}&external_source=tvdb_id", id, ApiKey.MovieDB));
                var obj = JObject.Parse(json).SelectToken("tv_episode_results");
                
                model.Aflevering = new Aflevering()
                {
                    Naam = (string)obj.SelectToken("name"),
                    Omschrijving = (string)obj.SelectToken("overview"),
                    ImagePath = (string)obj.SelectToken("still_path"),
                    Seizoen = (int)obj.SelectToken("season_number"),
                    Nummer = (int)obj.SelectToken("episode_number"),
                    AirDate = (string)obj.SelectToken("air_date")
                };
                
                json = client.DownloadString(string.Format("https://api.themoviedb.org/3/tv/{0}/external_ids?api_key={1}", (int)obj.SelectToken("show_id"), ApiKey.MovieDB)):
                obj = JObject.Parse(json);
                
                int tvdb_id = (int)obj.SelectToken("tvdb_id");
                
                if (tvdb_id != 0)
                {
                    Serie serie = SerieMng.ReadSerie(tvdb_id);
                    if (serie != null)
                    {
                        model.Aflevering.Serie = new Serie() { ID = serie.ID };
                        model.Aflevering.SerieID = 0;
                    }
                    else
                    {
                        model.Aflevering.Serie = new Serie() { ID = tvdb_id };
                        model.Aflevering.SerieID = -1;
                    }
                }
            }
            
            return View(model);
        }
    }
}
