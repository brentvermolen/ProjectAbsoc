using BL;
using BL.Domain;
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
    public class CollectiesController : Controller
    {
        private readonly CollectieManager CollectieMng = new CollectieManager();

        private const FilterMax DEFAULT_MAX = FilterMax.Dertig;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Account");
                return;
            }
        }

        // GET: Films
        public ActionResult Index()
        {
            var collecties = CollectieMng.ReadCollecties(CollectieSortEnum.Naam, (int)DEFAULT_MAX);

            CollectieViewModel model = new CollectieViewModel()
            {
                Collecties = collecties,
                Sorteren = CollectieSortEnum.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CollectieViewModel model)
        {
            string filter = "";
            if (model.Filter != null)
            {
                filter = model.Filter;
            }
            
            Func<Collectie, bool> predicate;
            switch (model.FilterOp)
            {
                case CollectieFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }

            model.Collecties = CollectieMng.ReadCollecties(predicate, model.Sorteren, (int)model.MaxFilms);

            return View(model);
        }

        private FilmManager FilmMng = new FilmManager();

        public ActionResult Details(int id)
        {
            Collectie c = CollectieMng.ReadCollectie(id);

            CollectieDetailsViewModel model = new CollectieDetailsViewModel()
            {
                Collectie = c,
                Films = new List<Film>()
            };

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/collection/{0}?api_key={1}", id, ApiKey.MovieDB));
                var obj = JObject.Parse(json);

                if (c == null)
                {
                    model.Collectie = obj.ToObject<Collectie>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                }

                foreach(var film in obj.SelectToken("parts"))
                {
                    Film f = FilmMng.ReadFilm((int)film.SelectToken("id"));

                    if (f == null)
                    {
                        f = film.ToObject<Film>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                        f.Duur = -1;
                    }
                    else
                    {
                        if (f.CollectieID == 0 && c != null)
                        {
                            f.CollectieID = id;
                            FilmMng.ChangeFilm(f);
                        }
                    }

                    model.Films.Add(f);
                }
            }

            return View(model);
        }
    }
}