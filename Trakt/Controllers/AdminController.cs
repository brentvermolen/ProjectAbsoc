using BL;
using BL.Domain;
using BL.Domain.ActeurKlassen;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Trakt.Models;

namespace Trakt.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly GebruikerManager GebruikerMng = new GebruikerManager();
        private readonly ArchiefManager ArchiefMng = new ArchiefManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Account");
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            var gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));

            if (gebruiker == null)
            {
                return RedirectToAction("Index", "Account");
            }

            if (gebruiker.IsAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }

            var gebruikers = GebruikerMng.GetGebruikers().ToList();
            gebruikers.Sort((g1, g2) => g1.Voornaam.CompareTo(g2.Voornaam));
            var archieven = ArchiefMng.GetArchieven();
            archieven.Sort((a1, a2) => a1.Naam.CompareTo(a2.Naam));


            AdminViewModel model = new AdminViewModel()
            {
                Gebruikers = gebruikers,
                Archieven = archieven
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeGebruikerState(AdminViewModel model, string id)
        {
            if (int.TryParse(id, out int intId))
            {
                var gebruiker = GebruikerMng.GetGebruiker(intId);

                if (gebruiker != null)
                {
                    gebruiker.EmailConfirmed = !gebruiker.EmailConfirmed;

                    GebruikerMng.ChangeGebruiker(gebruiker);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult VerwijderGebruiker(AdminViewModel model, string id)
        {
            if (int.TryParse(id, out int intId))
            {
                var gebruiker = GebruikerMng.GetGebruiker(intId);

                if (gebruiker != null)
                {
                    GebruikerMng.DeleteGebruiker(gebruiker);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateGebruikerArchief(AdminViewModel model)
        {
            foreach (Archief archief in model.Archieven)
            {
                Archief aDb = GebruikerMng.GetArchief(archief.ID);

                foreach (Gebruiker gebruiker in archief.Gebruikers)
                {
                    List<int> ids = new List<int>();
                    foreach (Gebruiker archiefGebruiker in aDb.Gebruikers)
                    {
                        ids.Add(archiefGebruiker.ID);
                    }

                    if (!ids.Contains(gebruiker.ID))
                    {
                        Gebruiker gDb = GebruikerMng.GetGebruiker(gebruiker.ID);
                        gDb.Archief.Add(aDb);

                        GebruikerMng.ChangeGebruiker(gDb);
                        ids.Add(gebruiker.ID);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        private readonly FilmManager FilmMng = new FilmManager();
        private readonly ActeurManager ActeurMng = new ActeurManager();

        [HttpPost]
        public ActionResult FilmToevoegen(AdminViewModel model, string id)
        {
            TempData["msg"] = "<script>alert('Er ging iets mis bij het toevoegen van de film...')</script>";

            if (int.TryParse(id, out int intId) == true)
            {
                if (FilmMng.ReadFilm(intId) != null)
                {
                    TempData["msg"] = "";
                }
                else
                {
                    string request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=en-EN&append_to_response=videos",
                            intId, "2719fd17f1c54d219dedc3aa9309a1e2");

                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        var json = client.DownloadString(request);
                        JObject obj = JObject.Parse(json);
                        Film film = obj.ToObject<Film>();
                        try
                        {
                            film.CollectieID = (int)obj.SelectToken("belongs_to_collection.id");
                        }
                        catch (Exception) { film.CollectieID = 0; }

                        try
                        {
                            film.TrailerId = (string)obj.SelectToken("videos.results[0].key");
                        }
                        catch (Exception) { }
                        film.Toegevoegd = DateTime.Today;

                        request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=nl-BE&append_to_response=videos",
                           intId, "2719fd17f1c54d219dedc3aa9309a1e2");

                        json = client.DownloadString(request);
                        obj = JObject.Parse(json);

                        string nlOmsch = (string)obj.SelectToken("overview");
                        string nlTagline = (string)obj.SelectToken("tagline");
                        string nlTrailer = (string)obj.SelectToken("videos.results[0].key");

                        if (!nlOmsch.Equals(""))
                        {
                            film.Omschrijving = nlOmsch;
                        }
                        if (!nlTagline.Equals(""))
                        {
                            film.Tagline = nlTagline;
                        }
                        if (!nlTrailer.Equals(""))
                        {
                            film.TrailerId = nlTrailer;
                        }

                        FilmMng.AddFilm(film);

                        using (WebClient clientActeurs = new WebClient())
                        {
                            request = string.Format("https://api.themoviedb.org/3/movie/{0}/credits?api_key={1}", film.ID, "2719fd17f1c54d219dedc3aa9309a1e2");
                            clientActeurs.Encoding = Encoding.UTF8;
                            json = client.DownloadString(request);

                            obj = JObject.Parse(json);
                            film.Acteurs = new List<ActeurFilm>();
                            foreach (var acteur in obj.SelectToken("cast"))
                            {
                                if ((int)acteur.SelectToken("order") < 15)
                                {
                                    int acteurId = (int)acteur.SelectToken("id");
                                    Acteur a = ActeurMng.ReadActeur(acteurId);

                                    if (a == null)
                                    {
                                        a = new Acteur
                                        {
                                            ID = acteurId,
                                            Naam = (string)acteur.SelectToken("name"),
                                            ImagePath = (string)acteur.SelectToken("profile_path")
                                        };

                                        ActeurMng.AddActeur(a);
                                    }

                                    ActeurFilm acteurFilm = new ActeurFilm()
                                    {
                                        ActeurID = acteurId,
                                        FilmID = film.ID,
                                        Sort = (int)acteur.SelectToken("order"),
                                        Karakter = (string)acteur.SelectToken("character")
                                    };

                                    film.Acteurs.Add(acteurFilm);
                                }
                            }
                        }

                        FilmMng.ChangeFilm(film);

                        TempData["msg"] = "";
                    }
                }
            }

            return RedirectToAction("Index");
        }

        private class ActeurJson
        {
            [JsonProperty("cast")]
            public List<Acteur> Acteurs { get; set; }
        }

        private SerieManager SerieMng = new SerieManager();

        public ActionResult ZoekSerie(string naamSerie)
        {
            ResultatenViewModel model = new ResultatenViewModel()
            {
                Series = new List<Serie>(),
                Afleveringen = new List<Aflevering>()
            };

            string token = getLoginTokenAsync().Result;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/search/series?name=" + naamSerie);
            
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);

                    foreach (var res in json.SelectToken("data"))
                    {
                        res.ToString();

                        Serie s = res.ToObject<Serie>();
                        s.BannerPath = "https://thetvdb.com/banners/" + res.SelectToken("banner");

                        Serie sDb = SerieMng.ReadSerie(s.ID);
                        if (sDb != null)
                        {
                            s.ID = -1;
                        }
                        model.Series.Add(s);
                    }
                }
                model.Series.Sort((s1, s2) => s2.AirDate.CompareTo(s1.AirDate));
            }
            catch (Exception) { }

            return View("Resultaten", model);
        }

        public ActionResult SerieToevoegen(ResultatenViewModel model, string id)
        {
            if (int.TryParse(id, out int intId))
            {
                string token = getLoginTokenAsync().Result;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + id);

                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Accept-Language", "en");
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Serie serie = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);

                    serie = json.SelectToken("data").ToObject<Serie>();
                    serie.BannerPath = "https://thetvdb.com/banners/" + json.SelectToken("data.banner");
                }

                if (serie != null)
                {
                    httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + id);

                    httpWebRequest.Accept = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers.Add("Accept-Language", "nl");
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        JObject json = JObject.Parse(result);

                        string nlOmsch = (string)json.SelectToken("data.overview");

                        if (nlOmsch != null)
                        {
                            serie.Omschrijving = nlOmsch;
                        }
                    }
                }

                httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/series/" + id + "/actors");

                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject json = JObject.Parse(result);

                    serie.Acteurs = new List<ActeurSerie>();
                    foreach (var acteur in json.SelectToken("data"))
                    {
                        int acteurId = (int)acteur.SelectToken("id");
                        Acteur a = ActeurMng.ReadActeur(acteurId);

                        if (a == null)
                        {
                            a = new Acteur
                            {
                                ID = acteurId,
                                Naam = (string)acteur.SelectToken("name"),
                                ImagePath = "https://thetvdb.com/banners/" + (string)acteur.SelectToken("image")
                            };

                            ActeurMng.AddActeur(a);
                        }

                        ActeurSerie acteurFilm = new ActeurSerie()
                        {
                            ActeurID = acteurId,
                            SerieID = serie.ID,
                            Sort = (int)acteur.SelectToken("sortOrder"),
                            Karakter = (string)acteur.SelectToken("role")
                        };

                        serie.Acteurs.Add(acteurFilm);
                    }
                }

                SerieMng.AddSerie(serie);
            }

            TempData["msg"] = "Serie toevoegen mislukt!";
            return RedirectToAction("Index");
        }

        public async System.Threading.Tasks.Task<string> getLoginTokenAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/login");

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"apikey\":\"U5YZ7ESIBVSADAZ0\"}";

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