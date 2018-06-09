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
            return View(SerieMng.ReadSerie(id));
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
    }
}