using BL;
using BL.Domain;
using BL.Domain.ActeurKlassen;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Trakt.Models;

namespace Trakt.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private readonly SerieManager SerieMng = new SerieManager();

        private const FilterMax DEFAULT_MAX = FilterMax.Vijftig;

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
            var series = SerieMng.ReadSeries(SerieSortEnum.Naam, (int)DEFAULT_MAX);

            SerieViewModel model = new SerieViewModel()
            {
                Series = series,
                Sorteren = SerieSortEnum.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SerieViewModel model)
        {
            string filter = "";
            if (model.Filter != null)
            {
                filter = model.Filter;
            }

            Func<Serie, bool> predicate;
            switch (model.FilterOp)
            {
                case SerieFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }

            model.Series = SerieMng.ReadSeries(predicate, model.Sorteren, (int)model.MaxFilms);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            SerieDetailsViewModel model = new SerieDetailsViewModel()
            {
                Serie = SerieMng.ReadSerie(id),
                Acteurs = new Dictionary<Acteur, bool>()
            };

            foreach (var acteur in model.Serie.Acteurs)
            {
                model.Acteurs.Add(acteur.Acteur, true);
            }

            return View(model);
        }

        private ActeurManager ActeurMng = new ActeurManager();

        public ActionResult DetailsAndere(int id)
        {
            if (SerieMng.ReadSerie(id) != null)
            {
                return Details(id);
            }

            SerieDetailsViewModel model = new SerieDetailsViewModel()
            {
                Acteurs = new Dictionary<Acteur, bool>()
            };

            string token = GetLoginTokenAsync().Result;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + id);

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Accept-Language", "en");
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var sr = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = sr.ReadToEnd();
                var json = JObject.Parse(result);

                model.Serie = json.SelectToken("data").ToObject<Serie>();
                model.Serie.BannerPath = "https://thetvdb.com/banners/" + (string)json.SelectToken("data.banner");
            }

            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + model.Serie.ID + "/images/query?keyType=poster");

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Accept-Language", "en");
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var sr = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = sr.ReadToEnd();
                    var json = JObject.Parse(result);

                    try
                    {
                        model.Serie.PosterPath = "https://thetvdb.com/banners/" + (string)json.SelectToken("data[0].fileName");
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { model.Serie.PosterPath = null; }

            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + id + "/actors");

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JObject json = JObject.Parse(result);

                model.Serie.Acteurs = new List<ActeurSerie>();
                foreach (var acteur in json.SelectToken("data"))
                {
                    int acteurId = (int)acteur.SelectToken("id");
                    Acteur a = ActeurMng.ReadActeur(acteurId);

                    if (a == null)
                    {
                        a = new Acteur()
                        {
                            ID = acteurId,
                            Naam = (string)acteur.SelectToken("name"),
                            ImagePath = "https://thetvdb.com/banners/" + (string)acteur.SelectToken("image")
                        };
                    }

                    model.Acteurs.Add(a, (a == null ? true : false));

                    ActeurSerie acteurFilm = new ActeurSerie()
                    {
                        ActeurID = acteurId,
                        SerieID = model.Serie.ID,
                        Acteur = a,
                        Serie = model.Serie,
                        Sort = (int)acteur.SelectToken("sortOrder"),
                        Karakter = (string)acteur.SelectToken("role")
                    };

                    model.Serie.Acteurs.Add(acteurFilm);
                }
            }

            httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://api.thetvdb.com/series/{0}/episodes", model.Serie.ID));

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JObject json = JObject.Parse(result);

                model.Serie.Afleveringen = new List<Aflevering>();
                foreach (var aflev in json.SelectToken("data"))
                {
                    Aflevering aflevering = aflev.ToObject<Aflevering>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                    aflevering.ImagePath = string.Format("https://www.thetvdb.com/banners/episodes/{0}/{1}.jpg", model.Serie.ID, aflevering.ID);
                    model.Serie.Afleveringen.Add(aflevering);
                }
            }

            return View("Details", model);
        }

        public ActionResult Lijst(int id)
        {
            var series = SerieMng.ReadSeries(f => f.Acteurs.FirstOrDefault(a => a.ActeurID == id) != null, SerieSortEnum.Naam, (int)DEFAULT_MAX);

            SerieViewModel model = new SerieViewModel()
            {
                Series = series,
                Sorteren = SerieSortEnum.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View("Index", model);
        }

        public async Task<string> GetLoginTokenAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/login");

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"apikey\":\"" + ApiKey.TvDB + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JObject json = JObject.Parse(result);

                return (string)json.SelectToken("token");
            }
        }
    }
}