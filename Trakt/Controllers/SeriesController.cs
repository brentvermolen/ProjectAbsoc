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
    public class SeriesController : Controller
    {
        private readonly SerieManager SerieMng = new SerieManager();

        private const FilterMaxFilms DEFAULT_MAX = FilterMaxFilms.Vijftig;

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

            var series = SerieMng.ReadSeries().OrderBy(f => f.Naam).Where(f => count++ < (int)DEFAULT_MAX).ToList();

            SerieViewModel model = new SerieViewModel()
            {
                Series = series,
                Sorteren = SerieSorterenOp.Naam,
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

            var count = 0;
            List<Serie> series = new List<Serie>();

            switch (model.FilterOp)
            {
                case SerieFilterOp.Naam:
                default:
                    series = SerieMng.ReadSeries().Where(f => f.Naam.ToLower().Contains(filter.ToLower())).ToList();
                    break;
            }

            model.Series = series;

            switch (model.Sorteren)
            {
                case SerieSorterenOp.Naam:
                default:
                    model.Series.Sort((f1, f2) => f1.Naam.CompareTo(f2.Naam));
                    break;
                case SerieSorterenOp.AantalAfleveringen:
                    model.Series.Sort((f1, f2) => f1.Afleveringen.Count.CompareTo(f2.Afleveringen.Count));
                    break;
            }
            
            model.Series = model.Series.Where(f => count++ < (int)model.MaxFilms).ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return View(SerieMng.ReadSerie(id));
        }

        public ActionResult Lijst(int id)
        {
            var series = SerieMng.ReadSeries().Where(f => f.Acteurs.FirstOrDefault(a => a.ActeurID == id) != null).OrderBy(f => f.Naam).ToList();

            SerieViewModel model = new SerieViewModel()
            {
                Series = series,
                Sorteren = SerieSorterenOp.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View("Index", model);
        }
    }
}