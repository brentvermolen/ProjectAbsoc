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
    public class AbsocInitializer : DropCreateDatabaseAlways<AbsocContext>
    {
        protected override void Seed(AbsocContext context)
        {
            ReadGemeenten(context);

            context.SaveChanges();
        }

        private void ReadGemeenten(AbsocContext context)
        {
            using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/gemeente.json")))
            {
                string json = sr.ReadToEnd();

                List<Gemeente> gemeentes = JsonConvert.DeserializeObject<List<Gemeente>>(json);

                gemeentes = gemeentes.Where(g => g.Deelgemeente.Equals("Neen")).ToList();

                gemeentes.Add(new Gemeente() { Deelgemeente = "Neen", Postcode = "0000", Plaatsnaam = "Onkbekend", Provincie = "Onbekend" });

                context.Gemeentes.AddRange(gemeentes);
            }
        }
    }
}
