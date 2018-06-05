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
    public class CollectiesController : Controller
    {
        private readonly CollectieManager CollectieMng = new CollectieManager();

        private const FilterMax DEFAULT_MAX = FilterMax.Twaalf;

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

            var collecties = CollectieMng.ReadCollecties().OrderBy(f => f.Naam).Where(f => count++ < (int)DEFAULT_MAX).ToList();

            CollectieViewModel model = new CollectieViewModel()
            {
                Collecties = collecties,
                Sorteren = CollectieSorterenOp.Naam,
                MaxFilms = DEFAULT_MAX
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CollectieViewModel model)
        {
            string filter = "";
            if (model.Filter != null)
            {
                filter = model.Filter;
            }

            var count = 0;
            List<Collectie> collecties = new List<Collectie>();

            switch (model.FilterOp)
            {
                case CollectieFilterOp.Naam:
                default:
                    collecties = CollectieMng.ReadCollecties().Where(f => f.Naam.ToLower().Contains(filter.ToLower())).ToList();
                    break;
            }

            model.Collecties = collecties;

            switch (model.Sorteren)
            {
                case CollectieSorterenOp.Naam:
                default:
                    model.Collecties.Sort((f1, f2) => f1.Naam.CompareTo(f2.Naam));
                    break;
            }


            model.Collecties = model.Collecties.Where(f => count++ < (int)model.MaxFilms).ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return View(CollectieMng.ReadCollectie(id));
        }
    }
}