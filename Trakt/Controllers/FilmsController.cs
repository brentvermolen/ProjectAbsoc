using BL;
using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trakt.Models;

namespace Trakt.Controllers
{
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly FilmManager FilmMng = new FilmManager();

        private const FilterMaxFilms DEFAULT_MAX_FILMS = FilterMaxFilms.Tien;

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

            FilmViewModel model = new FilmViewModel()
            {
                Films = films,
                Sorteren = FilmSorterenOp.Naam,
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

            var count = 0;
            List<Film> films = new List<Film>();

            switch (model.FilterOp)
            {
                case FilmFilterOp.Acteur:
                    films = FilmMng.ReadFilms().Where(f => f.Acteurs.FirstOrDefault(a => a.Acteur.Naam.ToLower().Contains(filter)) != null).ToList();
                    break;
                case FilmFilterOp.Jaar:
                    int jaartal;
                    if (int.TryParse(model.Filter, out jaartal) == false)
                    { 
                        model.Filter = DateTime.Today.Year.ToString();
                    }

                    films = FilmMng.ReadFilms().Where(f => f.ReleaseDate.StartsWith(filter.ToLower())).ToList();
                    break;
                case FilmFilterOp.Naam:
                default:
                    films = FilmMng.ReadFilms().Where(f => f.Naam.ToLower().Contains(filter.ToLower())).ToList();
                    break;
            }

            model.Films = films;

            switch (model.Sorteren)
            {
                case FilmSorterenOp.Release:
                    model.Films.Sort((f1, f2) => f1.ReleaseDate.CompareTo(f2.ReleaseDate));
                    break;
                case FilmSorterenOp.Toegevoegd:
                    model.Films.Sort((f1, f2) =>
                    {
                        if (f1.Toegevoegd.HasValue && f2.Toegevoegd.HasValue)
                        {
                            return f1.Toegevoegd.Value.CompareTo(f2.Toegevoegd.Value);
                        }
                        else
                        {
                            if (f1.Toegevoegd.HasValue)
                            {
                                return -1;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    });
                    break;
                case FilmSorterenOp.Collectie:
                    model.Films.Sort((f1, f2) => f1.Collectie.Naam.CompareTo(f2.Collectie.Naam));
                    break;
                case FilmSorterenOp.Naam:
                default:
                    model.Films.Sort((f1, f2) => f1.Naam.CompareTo(f2.Naam));
                    break;
            }

            
            model.Films = model.Films.Where(f => count++ < (int)model.MaxFilms).ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return View(FilmMng.ReadFilm(id));
        }

        public ActionResult Lijst(int id)
        {
            var films = FilmMng.ReadFilms().Where(f => f.Acteurs.FirstOrDefault(a => a.ActeurID == id) != null).OrderBy(f => f.Naam).ToList();

            FilmViewModel model = new FilmViewModel()
            {
                Films = films,
                Sorteren = FilmSorterenOp.Naam,
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