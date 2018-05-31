using BL;
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
    public class AdminController : Controller
    {
        private readonly GebruikerManager GebruikerMng = new GebruikerManager();
        private readonly ArchiefManager ArchiefMng = new ArchiefManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Account");
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            var gebruiker = GebruikerMng.GetGebruiker(int.Parse(User.Identity.GetUserId()));

            if (gebruiker == null)
            {
                return RedirectToAction("Index", "Account");
            }

            if (gebruiker.IsAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }

            var gebruikers = GebruikerMng.GetGebruikers().Where(g => g.IsAdmin == false).ToList();
            gebruikers.Sort((g1, g2) => g1.Voornaam.CompareTo(g2.Voornaam));
            var archieven = ArchiefMng.GetArchieven();
            archieven.Sort((a1, a2) => a1.Naam.CompareTo(a2.Naam));


            AdminViewModel model = new AdminViewModel()
            {
                Gebruikers = gebruikers,
                Archieven = archieven
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeGebruikerState(AdminViewModel model, string id)
        {
            int intId = 0;
            if (int.TryParse(id, out intId))
            {
                var gebruiker = GebruikerMng.GetGebruiker(intId);

                if (gebruiker != null)
                {
                    gebruiker.EmailConfirmed = !gebruiker.EmailConfirmed;

                    GebruikerMng.ChangeGebruiker(gebruiker);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetGebruiker(string id)
        {
            return View(GebruikerMng.GetGebruiker(int.Parse(id)));
        }
    }
}