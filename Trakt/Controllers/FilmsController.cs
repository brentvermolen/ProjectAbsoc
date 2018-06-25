using BL;
using BL.Domain;
using BL.Domain.ActeurKlassen;
using Microsoft.AspNet.Identity;
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
    public class FilmsController : Controller
    {
        private readonly FilmManager FilmMng = new FilmManager();

        private const FilterMax DEFAULT_MAX_FILMS = FilterMax.Twaalf;

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
            int count = 0;

            var films = FilmMng.ReadFilms().OrderBy(f => f.Naam).Where(f => count++ < (int)DEFAULT_MAX_FILMS).ToList();
            films = FilmMng.ReadFilms(FilmSortEnum.Naam, (int)DEFAULT_MAX_FILMS);

            FilmViewModel model = new FilmViewModel()
            {
                Films = films,
                Sorteren = FilmSortEnum.Naam,
                MaxFilms = DEFAULT_MAX_FILMS
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FilmViewModel model)
        {
            string filter = "";
            if (model.Filter != null)
            {
                filter = model.Filter;
            }

            Func<Film, bool> predicate;
            switch (model.FilterOp)
            {
                case FilmFilterOp.Acteur:
                    predicate = f => f.Acteurs.FirstOrDefault(a => a.Acteur.Naam.ToLower().Contains(filter.ToLower())) != null;
                    break;
                case FilmFilterOp.Jaar:
                    if (int.TryParse(model.Filter, out int jaartal) == false)
                    {
                        model.Filter = DateTime.Today.Year.ToString();
                    }

                    predicate = f => f.ReleaseDate.StartsWith(filter.ToLower());
                    break;
                case FilmFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }

            model.Films = FilmMng.ReadFilms(predicate, model.Sorteren, (int)model.MaxFilms);

            var count = 0;
            model.Films = model.Films.Where(f => count++ < (int)model.MaxFilms).ToList();

            return View(model);
        }

        private ArchiefManager ArchiefMng = new ArchiefManager();
        private GebruikerManager GebruikerMng = new GebruikerManager();

        public ActionResult Details(int id)
        {
            Gebruiker gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));
            ViewBag.Archieven = new List<string>();

            foreach (Archief archief in gebruiker.Archief)
            {
                if (archief != null)
                {
                    if (archief.Films.FirstOrDefault(f => f.ID == id) != null)
                    {
                        ViewBag.Archieven.Add(archief.Naam);
                    }
                }
            }

            List<Film> gelijkaardige = new List<Film>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString("https://api.themoviedb.org/3/movie/" + id + "/similar?api_key=" + ApiKey.MovieDB);
                JObject obj = JObject.Parse(json);

                gelijkaardige = obj.SelectToken("results").ToObject<List<Film>>();
            }

            foreach (Film film in gelijkaardige)
            {
                if (FilmMng.ReadFilm(film.ID) == null)
                {
                    film.Duur = -1;
                }
            }

            FilmDetailsViewModel model = new FilmDetailsViewModel()
            {
                Film = FilmMng.ReadFilm(id),
                GelijkaardigeFilms = gelijkaardige.OrderByDescending(g => g.ReleaseDate).Take(10).ToList()
            };

            return View(model);
        }

        public ActionResult Details(FilmDetailsViewModel model)
        {
            return View(model);
        }

        private ActeurManager ActeurMng = new ActeurManager();
        private CollectieManager CollectieMng = new CollectieManager();

        public ActionResult DetailsAndere(int id)
        {
            Gebruiker gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));
            ViewBag.Archieven = new List<string>();

            List<Film> gelijkaardige = new List<Film>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/movie/{0}/similar?api_key={1}", id, ApiKey.MovieDB));
                JObject obj = JObject.Parse(json);

                gelijkaardige = obj.SelectToken("results").ToObject<List<Film>>();
            }

            foreach (Film f in gelijkaardige)
            {
                if (FilmMng.ReadFilm(f.ID) == null)
                {
                    f.Duur = -1;
                }
            }

            string request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=en-EN&append_to_response=videos", id, ApiKey.MovieDB);

            Film film;
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(request);
                JObject obj = JObject.Parse(json);
                film = obj.ToObject<Film>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                try
                {
                    film.CollectieID = (int)obj.SelectToken("belongs_to_collection.id");
                    var collectie = CollectieMng.ReadCollectie(film.CollectieID);
                    if (collectie == null)
                    {
                        film.CollectieID = 0;
                    }
                    else
                    {
                        film.Collectie = collectie;
                    }
                }
                catch (Exception) { film.CollectieID = 0; }

                try
                {
                    film.TrailerId = (string)obj.SelectToken("videos.results[0].key");
                }
                catch (Exception) { }
                film.Toegevoegd = DateTime.Today;

                request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=nl-BE&append_to_response=videos", id, ApiKey.MovieDB);

                json = client.DownloadString(request);
                obj = JObject.Parse(json);

                string nlOmsch = (string)obj.SelectToken("overview");
                string nlTagline = (string)obj.SelectToken("tagline");
                string nlTrailer = (string)obj.SelectToken("videos.results[0].key");

                if (nlOmsch != null)
                {
                    if (!nlOmsch.Equals(""))
                    {
                        film.Omschrijving = nlOmsch;
                    }
                }
                if (nlTagline != null)
                {
                    if (!nlTagline.Equals(""))
                    {
                        film.Tagline = nlTagline;
                    }
                }
                if (nlTrailer != null)
                {
                    if (!nlTrailer.Equals(""))
                    {
                        film.TrailerId = nlTrailer;
                    }
                }

                using (WebClient clientActeurs = new WebClient())
                {
                    request = string.Format("https://api.themoviedb.org/3/movie/{0}/credits?api_key={1}", film.ID, ApiKey.MovieDB);
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
                                    ID = -1,
                                    Naam = (string)acteur.SelectToken("name"),
                                    ImagePath = (string)acteur.SelectToken("profile_path")
                                };
                            }

                            ActeurFilm acteurFilm = new ActeurFilm()
                            {
                                Acteur = a,
                                ActeurID = a.ID,
                                Film = film,
                                FilmID = film.ID,
                                Sort = (int)acteur.SelectToken("order"),
                                Karakter = (string)acteur.SelectToken("character")
                            };

                            film.Acteurs.Add(acteurFilm);
                        }
                    }
                }
            }

            ViewBag.FilmID = film.ID;

            //Check aanvraag
            ViewBag.Aangevraagd = FilmMng.IsAangevraagd(film.ID);            

            film.ID = -1;

            FilmDetailsViewModel model = new FilmDetailsViewModel()
            {
                Film = film,
                GelijkaardigeFilms = gelijkaardige.OrderByDescending(g => g.ReleaseDate).Take(10).ToList()
            };

            return View("Details", model);
        }

        public ActionResult Lijst(int id)
        {
            var films = FilmMng.ReadFilms().Where(f => f.Acteurs.FirstOrDefault(a => a.ActeurID == id) != null).OrderBy(f => f.Naam).ToList();

            FilmViewModel model = new FilmViewModel()
            {
                Films = films,
                Sorteren = FilmSortEnum.Naam,
                MaxFilms = DEFAULT_MAX_FILMS
            };

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult VerwijderFilm(Film model)
        {
            FilmMng.RemoveFilm(model.ID);

            return RedirectToAction("Index");
        }
    }
}