using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trakt.Controllers
{
    [Authorize]
    public class AfleveringController : Controller
    {
        private readonly SerieManager SerieMng = new SerieManager();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Account");
                return;
            }
        }

        public ActionResult Details(int id)
        {
            return View(SerieMng.ReadAflevering(id));
        }
    }
}