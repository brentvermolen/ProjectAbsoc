using Absoc.Models;
using BL;
using BL.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Absoc.Controllers
{
    public class BoekController : Controller
    {
        private readonly string BoekApi = ConfigurationManager.AppSettings["boekApi"];
        private readonly BoekManager BoekMng = new BoekManager();

        // GET: Boek
        public ActionResult Toevoegen()
        {
            return View(new BoekModel());
        }

        [HttpPost]
        public async Task<ActionResult> Toevoegen(BoekModel model)
        {
            if (model.ISBN == null || model.ISBN.Equals(""))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "ISBN mag niet leeg zijn");
                return View(model);
            }

            if (model.Titel == null || model.Titel.Equals(""))
            {
                model = await BoekOphalenAsync(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (WebClient client = new WebClient())
                    {
                        string destination = System.Web.HttpContext.Current.Server.MapPath("~/Content/BoekCovers/" + model.ISBN + ".png");
                        try
                        {
                            client.DownloadFile(new Uri(model.ImageLink), destination);
                            model.ImageLink = destination;
                        }catch(Exception e) { }
                    }

                    Boek boek = new Boek() { Auteur = model.Auteur, FlapTekst = model.FlapTekst, ImageLink = model.ImageLink, ISBN = model.ISBN, Omslag = model.Omslag, Titel = model.Titel };

                    BoekMng.AddBoek(boek);

                    return View(new BoekModel());
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(BoekModel model)
        {
            if (model.ISBN == null || model.ISBN.Equals(""))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "ISBN mag niet leeg zijn");
                return View(model);
            }

            Boek boek = BoekMng.GetBoek(model.ISBN);

            if (boek != null)
            {
                BoekMng.DeleteBoek(boek);
            }

            ModelState.AddModelError("", "Boek niet gevonden");
            return View(model);
        }

        public async Task<BoekModel> BoekOphalenAsync(BoekModel model)
        {
            Boek boek = BoekMng.GetBoek(model.ISBN);
            if (boek != null)
            {
                model.FlapTekst = boek.FlapTekst;
                model.Auteur = boek.Auteur;
                model.ImageLink = boek.ImageLink;
                model.ISBN = boek.ISBN;
                model.Omslag = boek.Omslag;
                model.Titel = boek.Titel;
                model.alInDb = true;
                ModelState.Clear();
                ModelState.AddModelError("db", "Boek staat al in databank");
                return model;
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetAsync("https://boekenliefde.nl/edition_info_get.api?key=" + BoekApi + "&isbn=" + model.ISBN).Result.Content.ReadAsStringAsync();
                    json.ToString();

                    model = JsonConvert.DeserializeObject<BoekModel>(json);
                    model.ImageLink = model.ImageLink.Replace("small", "original");
                }

                ModelState.Clear();

                return model;
            }
        }
    }
}