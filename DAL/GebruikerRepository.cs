using BL.Domain;
using BL.Domain.IdentityKlassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GebruikerRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public Gebruiker ReadGebruiker(int id)
        {
            return ctx.Gebruikers.Find(id);
        }

        public void UpdateGebruiker(Gebruiker gebruiker)
        {
            ctx.Entry(gebruiker).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }

        public bool ValidPostcode(string postcode)
        {
            return ctx.Gemeentes.Find(postcode) != null;
        }

        public List<Gebruiker> ReadGebruikers()
        {
            return ctx.Gebruikers.ToList();
        }

        public void RemoveGebruiker(Gebruiker gebruiker)
        {
            ctx.Gebruikers.Remove(gebruiker);
            ctx.SaveChanges();
        }

        public Archief ReadArchief(int iD)
        {
            return ctx.Archieven.Find(iD);
        }

        public void CreateLogin(int user)
        {
            var gebruiker = ReadGebruiker(user);

            GebruikerLogin login = new GebruikerLogin()
            {
                LoginProvider = "Website",
                ProviderKey = DateTime.Now.ToString("dd-MM-yyyy hh:mm"),
                Gebruiker = gebruiker,
                UserId = user
            };

            gebruiker.GebruikerLogins.Add(login);
            UpdateGebruiker(gebruiker);
        }
    }
}
