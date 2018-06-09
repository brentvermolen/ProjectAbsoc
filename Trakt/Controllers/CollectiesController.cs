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
            var collecties = CollectieMng.ReadCollecties(CollectieSortEnum.Naam, (int)DEFAULT_MAX);

            CollectieViewModel model = new CollectieViewModel()
            {
                Collecties = collecties,
                Sorteren = CollectieSortEnum.Naam,
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
            
            Func<Collectie, bool> predicate;
            switch (model.FilterOp)
            {
                case CollectieFilterOp.Naam:
                default:
                    predicate = f => f.Naam.ToLower().Contains(filter.ToLower());
                    break;
            }

            model.Collecties = CollectieMng.ReadCollecties(predicate, model.Sorteren, (int)model.MaxFilms);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return View(CollectieMng.ReadCollectie(id));
        }
    }
}