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
    public class ActeursController : Controller
    {
        private readonly ActeurManager ActeurMng = new ActeurManager();

        private const FilterMax DEFAULT_MAX = FilterMax.Twintig;

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

            var acteurs = ActeurMng.ReadActeurs().OrderBy(f => f.Naam).Where(f => count++ < (int)DEFAULT_MAX).ToList();
            acteurs = ActeurMng.ReadActeurs(ActeurSortEnum.Naam, (int)DEFAULT_MAX);

            ActeurViewModel model = new ActeurViewModel()
            {
                Acteurs = acteurs,
                Sorteren = ActeurSortEnum.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ActeurViewModel model)
        {
            string filter = "";
            if (model.Filter != null)
            {
                filter = model.Filter;
            }

            Func<Acteur, bool> predicate;
            switch (model.FilterOp)
            {
                case ActeurFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }


            model.Acteurs = ActeurMng.ReadActeurs(predicate, model.Sorteren, (int)model.MaxFilms);

            return View(model);
        }

        private FilmManager FilmMng = new FilmManager();

        public ActionResult Details(int id)
        {
            ActeurDetailsViewModel model = new ActeurDetailsViewModel()
            {
                Acteur = ActeurMng.ReadActeur(id),
                Films = new List<Film>()
            };

            if (model.Acteur.Films.Count != 0)
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/person/{0}?api_key={1}&append_to_response=movie_credits", id, ApiKey.MovieDB));
                    JObject obj = JObject.Parse(json);

                    model.Geboortedatum = (DateTime)obj.SelectToken("birthday");
                    try
                    {
                        model.Sterftedatum = obj.SelectToken("deathday").ToObject<DateTime>();
                    }
                    catch (Exception) { model.Sterftedatum = null; }
                    model.Omschrijving = (string)obj.SelectToken("biography");
                    model.Geboorteplaats = (string)obj.SelectToken("place_of_birth");

                    foreach (var film in obj.SelectToken("movie_credits.cast"))
                    {
                        if ((int)film.SelectToken("vote_average") > 5)
                        {
                            Film f = film.ToObject<Film>();


                            if (FilmMng.ReadFilm(f.ID) == null)
                            {
                                f.Duur = -1;

                                model.Films.Add(f);
                            }
                        }
                    }
                }

                foreach (var film in model.Acteur.Films)
                {
                    if (model.Films.FirstOrDefault(f => f.ID == film.FilmID) == null)
                    {
                        model.Films.Add(film.Film);
                    }
                }
            }
            else if (model.Acteur.Series.Count != 0)
            {
                model.Geboortedatum = DateTime.Today;
                try
                {
                    model.Sterftedatum = DateTime.Today;
                }
                catch (Exception) { model.Sterftedatum = null; }
                model.Omschrijving = "";
                model.Geboorteplaats = "";
            }

            return View(model);
        }
    }
}