using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trakt.Controllers
{
    public class AfleveringController : Controller
    {
        private readonly SerieManager SerieMng = new SerieManager();

        public ActionResult Details(int id)
        {
            return View(SerieMng.ReadAflevering(id));
        }
    }
}