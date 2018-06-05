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

            ActeurViewModel model = new ActeurViewModel()
            {
                Acteurs = acteurs,
                Sorteren = ActeurSorterenOp.Naam,
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

            var count = 0;
            List<Acteur> series = new List<Acteur>();

            switch (model.FilterOp)
            {
                case ActeurFilterOp.Naam:
                default:
                    series = ActeurMng.ReadActeurs().Where(f => f.Naam.ToLower().Contains(filter.ToLower())).ToList();
                    break;
            }

            model.Acteurs = series;

            switch (model.Sorteren)
            {
                case ActeurSorterenOp.Naam:
                default:
                    model.Acteurs.Sort((f1, f2) => f1.Naam.CompareTo(f2.Naam));
                    break;
                case ActeurSorterenOp.Aantal_Vermeldingen_Desc:
                    model.Acteurs.Sort((f1, f2) =>
                    {
                        int count1 = f1.Films.Count + f1.Series.Count;
                        int count2 = f2.Films.Count + f2.Series.Count;

                        return count2.CompareTo(count1);
                    });
                    break;
                case ActeurSorterenOp.Aantal_Vermeldingen:
                    model.Acteurs.Sort((f1, f2) =>
                    {
                        int count1 = f1.Films.Count + f1.Series.Count;
                        int count2 = f2.Films.Count + f2.Series.Count;

                        return count1.CompareTo(count2);
                    });
                    break;
            }
            
            model.Acteurs = model.Acteurs.Where(f => count++ < (int)model.MaxFilms).ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return View(ActeurMng.ReadActeur(id));
        }
    }
}