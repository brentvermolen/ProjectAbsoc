using BL.Domain;
using DAL;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class WeeklyReview : IJob
    {
        private readonly GebruikerRepository gebruikerRepo = new GebruikerRepository();
        private readonly FilmRepository filmRepo = new FilmRepository();
        private readonly SerieRepository serieRepo = new SerieRepository();

        Task IJob.Execute(IJobExecutionContext context)
        {
            List<Film> films = filmRepo.ReadFilms(f => f.Toegevoegd >= DateTime.Today.AddDays(-7));
            List<Aflevering> afleveringen = serieRepo.ReadAfleveringen(f => f.Toegevoegd >= DateTime.Today.AddDays(-7));
                
            if (films.Count > 0 || afleveringen.Count > 0)
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAdres"], ConfigurationManager.AppSettings["mailWw"]);

                foreach (Gebruiker gebruiker in gebruikerRepo.ReadGebruikers())
                {
                    try
                    {
                        MailMessage msg = new MailMessage();
                        msg.IsBodyHtml = true;

                        msg.To.Add(gebruiker.Email);

                        msg.From = new MailAddress(ConfigurationManager.AppSettings["mailAdres"], "Trakt");

                        msg.Subject = "Wekelijkse Update";

                        msg.Body = "<h1>Wekelijkse Update</h1>" +
                            "<p>Er zijn deze week " + films.Count.ToString() + " film(s) toegevoegd</p>" +
                            "<p>Er zijn deze week" + afleveringen.Count.ToString() + " aflevering(en) toegevoegd</p>";

                        client.Send(msg);

                        return Task.FromResult(0);
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }
            }

            return Task.FromResult(0);
        }
    }
}
