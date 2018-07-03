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
            Aflevering aflevering = SerieMng.ReadAflevering(id);
            
            if (aflevering == null)
            {
                return RedirectToAction("DetailsAndere", new { id });
            }
        
            AfleveringDetailsViewModel model = new AfleveringDetailsViewModel()
            {
                Aflevering = aflevering
            };
            
            return View(model);
        }
        
        public ActionResult DetailsAndere(int id)
        {
            Aflevering aflevering = null;
        
            AfleveringDetailsViewModel model = new AfleveringDetailsViewModel()
            {
                Aflevering = aflevering
            };
        
            return View(model);
        }
    }
}
