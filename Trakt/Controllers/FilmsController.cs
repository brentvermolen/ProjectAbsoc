using BL;
using BL.Domain;
using BL.Domain.ActeurKlassen;
using BL.Domain.FilmKlassen;
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

        private const FilterMax DEFAULT_MAX_FILMS = FilterMax.Dertig;

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
            FilmViewModel model = new FilmViewModel()
            {
                Films = FilmMng.ReadFilms(FilmSortEnum.Naam, (int)DEFAULT_MAX_FILMS),
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
                        filter = model.Filter;
                    }

                    predicate = f => f.ReleaseDate.StartsWith(filter.ToLower());
                    break;
                case FilmFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }

            model.Films = FilmMng.ReadFilms(predicate, model.Sorteren, (int)model.MaxFilms);

            return View(model);
        }

        private ArchiefManager ArchiefMng = new ArchiefManager();
        private GebruikerManager GebruikerMng = new GebruikerManager();

        public ActionResult Details(int id)
        {
            if (FilmMng.ReadFilm(id) == null)
            {
                return RedirectToAction("DetailsAndere", new { id });
            }

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

            gelijkaardige = gelijkaardige.OrderByDescending(g => g.ReleaseDate).Take(10).ToList();

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
                GelijkaardigeFilms = gelijkaardige,
                Acteurs = new Dictionary<Acteur, bool>()
            };

            foreach (var acteur in model.Film.Acteurs)
            {
                model.Acteurs.Add(acteur.Acteur, true);
            }

            return View(model);
        }

        private ActeurManager ActeurMng = new ActeurManager();
        private CollectieManager CollectieMng = new CollectieManager();

        public ActionResult DetailsAndere(int id)
        {
            if (FilmMng.ReadFilm(id) != null)
            {
                return RedirectToAction("Details", new { id });
            }

            Gebruiker gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));
            ViewBag.Archieven = new List<string>();

            List<Film> gelijkaardige = new List<Film>();
            var acteurs = new Dictionary<Acteur, bool>();

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
                }
                catch (Exception) { film.CollectieID = 0; }
                
                var collectie = CollectieMng.ReadCollectie(film.CollectieID);
                if (collectie == null)
                {
                    film.Collectie = obj.SelectToken("belongs_to_collection").ToObject<Collectie>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                    film.PosterPath = obj.SelectToken("belongs_to_collection.poster_path").ToObject<string>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                }
                else
                {
                    film.Collectie = collectie;
                }

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
                            bool inDb = true;

                            if (a == null)
                            {
                                inDb = false;

                                a = new Acteur
                                {
                                    ID = acteurId,
                                    Naam = (string)acteur.SelectToken("name"),
                                    ImagePath = (string)acteur.SelectToken("profile_path")
                                };
                            }

                            acteurs.Add(a, inDb);

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
                GelijkaardigeFilms = gelijkaardige.OrderByDescending(g => g.ReleaseDate).Take(10).ToList(),
                Acteurs = acteurs
            };

            return View("Details", model);
        }

        public ActionResult Lijst(int id) //Acteur id
        {
            FilmViewModel model = new FilmViewModel()
            {
                Films = FilmMng.ReadFilms(f => f.Acteurs.FirstOrDefault(a => a.ActeurID == id) != null, FilmSortEnum.Naam, (int)FilterMax.Duizend),
                Sorteren = FilmSortEnum.Naam,
                MaxFilms = FilterMax.Duizend
            };

            return View("Index", model);
        }

        public ActionResult Tag(int id)
        {
            var tag = FilmMng.ReadTag(id);

            return View(tag);
        }

        public ActionResult NietOpArchief()
        {
            var gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));
            var films = FilmMng.ReadFilms();

            foreach(var archief in gebruiker.Archief)
            {
                films.RemoveAll(f => archief.Films.FirstOrDefault(a => a.ID == f.ID) != null);
            }

            films.Sort((s1, s2) => s1.Naam.CompareTo(s2.Naam));

            FilmViewModel model = new FilmViewModel()
            {
                Films = films,
                Sorteren = FilmSortEnum.Naam,
                MaxFilms = FilterMax.Duizend
            };

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult VerwijderFilm(Film model)
        {
            FilmMng.RemoveFilm(model.ID);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult VerwijderTag(int tag)
        {
            FilmMng.RemoveTag(tag);

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult TagToevoegen(string tag, int film)
        {
            Film f = FilmMng.ReadFilm(film);
            
            if (f == null)
            {
                return RedirectToAction("Details", "Films", new { id = film });
            }
        
            Tag t = FilmMng.ReadTag(tag);
            
            f.Tags.Add(t);
            
            FilmMng.ChangeFilm(f);
            
            return RedirectToAction("Details", "Films", new { id = film });
        }
    }
}
