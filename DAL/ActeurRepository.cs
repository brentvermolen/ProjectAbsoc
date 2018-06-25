using BL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ActeurRepository
    {
        private readonly TraktContext ctx = new TraktContext();

        public List<Acteur> GetActeurs()
        {
            return ctx.Acteurs.ToList();
        }

        public Acteur GetActeur(int id)
        {
            return ctx.Acteurs.Find(id);
        }

        public void CreateActeur(Acteur a)
        {
            ctx.Acteurs.Add(a);
            ctx.SaveChanges();
        }

        public List<Acteur> GetActeurs(ActeurSortEnum sorteren, int top)
        {
            switch (sorteren)
            {
                case ActeurSortEnum.Aantal_Vermeldingen:
                    return ctx.Acteurs.OrderBy(a => a.Films.Count + a.Series.Count).Take(top).ToList();
                case ActeurSortEnum.Aantal_Vermeldingen_Desc:
                    return ctx.Acteurs.OrderByDescending(a => a.Films.Count + a.Series.Count).Take(top).ToList();
                case ActeurSortEnum.Naam:
                default:
                    return ctx.Acteurs.OrderBy(a => a.Naam).Take(top).ToList();
            }
        }

        public void DeleteActeur(int iD)
        {
            ctx.Acteurs.Remove(GetActeur(iD));
            ctx.SaveChanges();
        }

        public List<Acteur> GetActeurs(Func<Acteur, bool> predicate, ActeurSortEnum sorteren, int top)
        {
            switch (sorteren)
            {
                case ActeurSortEnum.Aantal_Vermeldingen:
                    return ctx.Acteurs.Where(predicate).OrderBy(a => a.Films.Count + a.Series.Count).Take(top).ToList();
                case ActeurSortEnum.Aantal_Vermeldingen_Desc:
                    return ctx.Acteurs.Where(predicate).OrderByDescending(a => a.Films.Count + a.Series.Count).Take(top).ToList();
                case ActeurSortEnum.Naam:
                default:
                    return ctx.Acteurs.Where(predicate).OrderBy(a => a.Naam).Take(top).ToList();
            }
        }
    }
}
