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
                Films = new List<Film>(),
                Series = new List<Serie>()
            };

            if (model.Acteur.Films.Count != 0)
            {
                return RedirectToAction("DetailsAndere", new { id });
            }
            else if (model.Acteur.Series.Count != 0)
            {
                //model.Geboortedatum = DateTime.Today;
                //try
                //{
                //    model.Sterftedatum = null;
                //}
                //catch (Exception) { model.Sterftedatum = null; }
                //model.Omschrijving = "";
                //model.Geboorteplaats = "";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/search/person?api_key={0}&query={1}", ApiKey.MovieDB, model.Acteur.Naam));
                    var obj = JObject.Parse(json);

                    if ((int)obj.SelectToken("total_results") == 1)
                    {
                        return RedirectToAction("DetailsAndere", new { id = (int)obj.SelectToken("results[0].id") });
                    }

                }
            }

            return View(model);
        }

        private SerieManager SerieMng = new SerieManager();

        public ActionResult DetailsAndere(int id)//Id moet van MovieDb komen om te werken
        {
            ActeurDetailsViewModel model = new ActeurDetailsViewModel()
            {
                Films = new List<Film>()
            };

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                var json = client.DownloadString(string.Format("https://api.themoviedb.org/3/person/{0}?api_key={1}&append_to_response=movie_credits,tv_credits", id, ApiKey.MovieDB));
                JObject obj = JObject.Parse(json);

                model.Acteur = new Acteur()
                {
                    ID = (int)obj.SelectToken("id"),
                    Naam = (string)obj.SelectToken("name"),
                    ImagePath = (string)obj.SelectToken("profile_path")
                };

                try
                {
                    model.Geboortedatum = (DateTime)obj.SelectToken("birthday");
                }
                catch (Exception) { model.Geboortedatum = null; }

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
                        Film f = FilmMng.ReadFilm((int)film.SelectToken("id"));

                        if (f == null)
                        {
                            f = film.ToObject<Film>(new Newtonsoft.Json.JsonSerializer() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                            f.Duur = -1;
                        }

                        model.Films.Add(f);
                    }
                }

                model.Series = new List<Serie>();

                foreach (var tvshow in obj.SelectToken("tv_credits.cast").OrderByDescending(t => t.SelectToken("episode_count")))
                {
                    Serie serie = new Serie()
                    {
                        ID = (int)tvshow.SelectToken("id"),
                        AirDate = (string)tvshow.SelectToken("first_air_date"),
                        Naam = (string)tvshow.SelectToken("original_name"),
                        PosterPath = (string)tvshow.SelectToken("poster_path"),
                        Netwerk = "0"
                    };

                    json = client.DownloadString(string.Format("https://api.themoviedb.org/3/tv/{0}/external_ids?api_key={1}", serie.ID, ApiKey.MovieDB));
                    var externalIds = JObject.Parse(json);

                    var tvdbId = -1;
                    try
                    {
                        tvdbId = (int)externalIds.SelectToken("tvdb_id");
                        serie.ID = tvdbId;
                    }
                    catch (Exception) { serie.Netwerk = "-2"; }

                    if (SerieMng.ReadSerie(tvdbId) == null)
                    {
                        serie.Netwerk = "-1";
                    }

                    model.Series.Add(serie);
                }
            }

            return View("Details", model);
        }
    }
}