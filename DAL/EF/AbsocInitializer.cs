using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Domain;

namespace DAL
{
    public class AbsocInitializer : CreateDatabaseIfNotExists<AbsocContext>
    {
        protected override void Seed(AbsocContext context)
        {
            Boek boek = new Boek()
            {
                Naam = "Dit is een test voor te zien of de DB werkt"
            };

            context.Boeken.Add(boek);

            context.SaveChanges();
        }
    }
}
