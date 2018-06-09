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

        public ActionResult Details(int id)
        {
            return View(ActeurMng.ReadActeur(id));
        }
    }
}