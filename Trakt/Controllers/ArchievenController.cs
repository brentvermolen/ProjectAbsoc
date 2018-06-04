using BL;
using BL.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Trakt.Models;

namespace Trakt.Controllers
{
    [Authorize]
    public class ArchievenController : Controller
    {
        private readonly ArchiefManager ArchiefMng = new ArchiefManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Home");
            }
        }

        // GET: Archieven
        public ActionResult Index()
        {
            var model = new ArchiefIndexModel()
            {
                Archieven = ArchiefMng.GetArchievenFromGebruiker(int.Parse(User.Identity.GetUserId()))
            };

            return View(model);
        }
    }
}