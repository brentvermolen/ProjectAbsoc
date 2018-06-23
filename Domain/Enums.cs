using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class ApiKey
    {
        public const string TvDB = "U5YZ7ESIBVSADAZ0";
        public const string MovieDB = "2719fd17f1c54d219dedc3aa9309a1e2";
    }

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
