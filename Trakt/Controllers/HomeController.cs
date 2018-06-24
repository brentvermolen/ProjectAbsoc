using Trakt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using BL.Domain;
using Antlr.Runtime.Misc;

namespace Absoc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly FilmManager FilmMng = new FilmManager();
        private readonly SerieManager SerieMng = new SerieManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                LaatsteFilms = FilmMng.ReadFilms(FilmSortEnum.Toegevoegd, 12),
                NieuwsteFilms = FilmMng.ReadFilms(FilmSortEnum.Release_Desc, 12),
                Afleveringen = SerieMng.ReadAfleveringen(AfleveringSortEnum.Toegevoegd, 6)
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Zoeken(string q)
        {
            ZoekenViewModel model = new ZoekenViewModel();



            return View(model);
        }
    }
}