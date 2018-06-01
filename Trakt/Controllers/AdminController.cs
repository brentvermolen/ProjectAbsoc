using BL;
using BL.Domain;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        private readonly FilmManager FilmMng = new FilmManager();

        [HttpPost]
        public ActionResult FilmToevoegen(AdminViewModel model, string id)
        {
            int intId;
            TempData["msg"] = "<script>alert('Er ging iets mis bij het toevoegen van de film...')</script>";

            if (int.TryParse(id, out intId) == true)
            {
                if (FilmMng.ReadFilm(intId) != null)
                {
                    TempData["msg"] = "<script>alert('Film staat al in de databank...')</script>";
                }
                else
                {
                    string request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=en-EN&append_to_response=videos",
                            intId, "2719fd17f1c54d219dedc3aa9309a1e2");

                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        var json = client.DownloadString(request);
                        JObject obj = JObject.Parse(json);
                        Film film = obj.ToObject<Film>();
                        try
                        {
                            film.CollectieID = (int)obj.SelectToken("belongs_to_collection.id");
                        }
                        catch (Exception e) { film.CollectieID = 0; }

                        try
                        {
                            film.TrailerId = (string)obj.SelectToken("videos.results[0].key");
                        }
                        catch (Exception e) { }
                        film.Toegevoegd = DateTime.Today;

                        request = string.Format("https://api.themoviedb.org/3/movie/{0}?api_key={1}&language=nl-BE&append_to_response=videos",
                           intId, "2719fd17f1c54d219dedc3aa9309a1e2");

                        json = client.DownloadString(request);
                        obj = JObject.Parse(json);

                        string nlOmsch = (string)obj.SelectToken("overview");
                        string nlTagline = (string)obj.SelectToken("tagline");
                        string nlTrailer = (string)obj.SelectToken("videos.results[0].key");

                        if (!nlOmsch.Equals(""))
                        {
                            film.Omschrijving = nlOmsch;
                        }
                        if (!nlTagline.Equals(""))
                        {
                            film.Tagline = nlTagline;
                        }
                        if (!nlTrailer.Equals(""))
                        {
                            film.TrailerId = nlTrailer;
                        }

                        FilmMng.AddFilm(film);

                        TempData["msg"] = "";
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}