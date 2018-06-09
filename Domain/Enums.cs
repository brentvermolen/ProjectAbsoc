using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public enum FilmSortEnum
    {
        Naam = 0,
        Toegevoegd,
        Release,
        Release_Desc
    }

    public enum AfleveringSortEnum
    {
        Toegevoegd = 0
    }

    public enum SerieSortEnum
    {
        Naam = 0,
        Aantal_Afleveringen = 1,
        Aantal_Afleveringen_Desc = 2
    }

    public enum CollectieSortEnum
    {
        Naam = 0
    }

    public enum ActeurSortEnum
    {
        Naam = 0,
        Aantal_Vermeldingen,
        Aantal_Vermeldingen_Desc
    }
}
