using BL;
using BL.Domain;
using Microsoft.AspNet.Identity;
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

            foreach(Archief archief in gebruiker.Archief)
            {
                if (archief != null)
                {
                    if (archief.Films.FirstOrDefault(f => f.ID == id) != null)
                    {
                        ViewBag.Archieven.Add(archief.Naam);
                    }
                }
            }

            return View(FilmMng.ReadFilm(id));
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