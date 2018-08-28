using Trakt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using BL.Domain;
using Antlr.Runtime.Misc;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Absoc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly FilmManager FilmMng = new FilmManager();
        private readonly SerieManager SerieMng = new SerieManager();
        private readonly GebruikerManager GebruikerMng = new GebruikerManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Index()
        {
            Random rnd = new Random();

            IndexViewModel model = new IndexViewModel()
            {
                RandomFilms = FilmMng.ReadFilms().OrderBy(f => rnd.Next()).Take(12).ToList(),
                LaatsteFilms = FilmMng.ReadFilms(FilmSortEnum.Toegevoegd, 12),
                NieuwsteFilms = FilmMng.ReadFilms(FilmSortEnum.Release_Desc, 12),
                Afleveringen = SerieMng.ReadAfleveringen(AfleveringSortEnum.Toegevoegd, 6),
                Gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()))
            };

            return View(model);
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        public ActionResult Zoeken(string q)
        {
            ZoekenViewModel model = new ZoekenViewModel()
            {
                Acteurs = ZoekActeurs(q),
                Collecties = ZoekCollecties(q),
                Films = ZoekFilms(q),
                Series = ZoekSeries(q)
            };

            return View(model);
        }

        private List<Acteur> ZoekActeurs(string q)
        {
            List<Acteur> acteurs = new List<Acteur>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                int page = 1;
                int max_page = 0;

                do
                {
                    var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/search/person?api_key={0}&query={1}&page={2}", ApiKey.MovieDB, q, page++));

                    var obj = JObject.Parse(json);
                    max_page = (int)obj.SelectToken("total_pages");

                    foreach (var acteur in obj.SelectToken("results"))
                    {
                        Acteur a = new Acteur()
                        {
                            ID = (int)acteur.SelectToken("id"),
                            ImagePath = (string)acteur.SelectToken("profile_path"),
                            Naam = (string)acteur.SelectToken("name")
                        };

                        acteurs.Add(a);
                    }
                } while (page <= max_page && acteurs.Count < 100);
            }

            return acteurs;
        }

        private List<Collectie> ZoekCollecties(string q)
        {
            List<Collectie> collecties = new List<Collectie>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/search/collection?api_key={0}&query={1}", ApiKey.MovieDB, q));
                var obj = JObject.Parse(json);

                collecties = obj.SelectToken("results").ToObject<List<Collectie>>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            }
           
            return collecties;
        }

        private List<Film> ZoekFilms(string q)
        {
            List<Film> films = new List<Film>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                int page = 1;
                int max_page = 0;

                do
                {
                    var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/search/movie?api_key={0}&query={1}&page={2}", ApiKey.MovieDB, q, page++));

                    var obj = JObject.Parse(json);

                    max_page = (int)obj.SelectToken("total_pages");

                    foreach (var film in obj.SelectToken("results"))
                    {
                        Film f = film.ToObject<Film>();

                        if (FilmMng.ReadFilm(f.ID) == null)
                        {
                            f.Duur = -1;

                            if (FilmMng.IsAangevraagd(f.ID))
                            {
                                f.Tagline = "Yes";
                            }
                            else
                            {
                                f.Tagline = "No";
                            }
                        }

                        films.Add(f);
                    }
                } while (page <= max_page && films.Count < 100);
            }

            return films;
        }

        private List<Serie> ZoekSeries(string q)
        {
            List<Serie> series = new List<Serie>();

            string token = GetLoginTokenAsync().Result;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://api.thetvdb.com/search/series?name={0}", q));

                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);

                    foreach (var serie in json.SelectToken("data"))
                    {
                        Serie s = serie.ToObject<Serie>();

                        httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + s.ID);

                        httpWebRequest.Accept = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Headers.Add("Accept-Language", "en");
                        httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                        httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        using (var sr = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            result = sr.ReadToEnd();
                            json = JObject.Parse(result);

                            s = json.SelectToken("data").ToObject<Serie>();
                        }

                        httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + s.ID + "/images/query?keyType=poster");

                        httpWebRequest.Accept = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Headers.Add("Accept-Language", "en");
                        httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                        try
                        {
                            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                            using (var sr = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                result = sr.ReadToEnd();
                                json = JObject.Parse(result);

                                try
                                {
                                    s.PosterPath = "https://thetvdb.com/banners/" + (string)json.SelectToken("data[0].fileName");
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { s.PosterPath = null; }

                        if (SerieMng.ReadSerie(s.ID) == null)
                        {
                            s.Netwerk = "-1";
                        }

                        series.Add(s);
                    }
                }
            }
            catch (Exception) { }

            return series;
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