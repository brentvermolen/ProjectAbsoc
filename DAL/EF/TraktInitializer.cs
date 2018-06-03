using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Domain;
using Newtonsoft.Json;

namespace DAL
{
    public class TraktInitializer : DropCreateDatabaseAlways<TraktContext>
    {
        protected override void Seed(TraktContext context)
        {
            ReadGemeenten(context);

            context.SaveChanges();
        }

        private void ReadGemeenten(TraktContext context)
        {
            using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/gemeente.json")))
            {
                string json = sr.ReadToEnd();

                List<Gemeente> gemeentes = JsonConvert.DeserializeObject<List<Gemeente>>(json);

                gemeentes = gemeentes.Where(g => g.Deelgemeente.Equals("Neen")).ToList();

                gemeentes.Add(new Gemeente() { Deelgemeente = "Neen", Postcode = "0000", Plaatsnaam = "Onbekend", Provincie = "Onbekend" });

                context.Gemeentes.AddRange(gemeentes);
            }
        }
    }
}
