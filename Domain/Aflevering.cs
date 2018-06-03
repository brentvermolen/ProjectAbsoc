using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Aflevering
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Naam { get; set; }
        public int Nummer { get; set; }
        public int Seizoen { get; set; }
        public string AirDate { get; set; }
        public string Omschrijving { get; set; }
        public int AfleveringNr { get; set; }
        public DateTime? Toegevoegd { get; set; }
        public string Path { get; set; }
        public string ImagePath { get; set; }

        public virtual Serie Serie { get; set; }
        public int SerieID { get; set; }

        [NotMapped]
        public string DisplayMember
        {
            get
            {
                if (Seizoen == 0 || Nummer == 0)
                {
                    return Path;
                }
                else
                {
                    if (Naam == null)
                    {
                        return "Seizoen " + Seizoen + " - Aflevering " + Nummer;
                    }
                    else
                    {
                        if (Naam.Equals(""))
                        {
                            return "Seizoen " + Seizoen + " - Aflevering " + Nummer;
                        }
                        else
                        {
                            return Naam + " S" + Seizoen.ToString("D2") + "E" + Nummer.ToString("D2");
                        }
                    }
                }
            }
        }

        public virtual List<Archief> Archieven { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Aflevering))
            {
                return false;
            }

            Aflevering aflevering = (Aflevering)obj;

            if (aflevering.SerieID != SerieID)
            {
                return false;
            }

            if (aflevering.Nummer == Nummer && aflevering.Seizoen == Seizoen)
            {
                return true;
            }

            return false;
        }

        public bool EqualEpisodeEnSeizoen(Aflevering aflevering)
        {
            if (aflevering.Nummer == Nummer && aflevering.Seizoen == Seizoen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return ID + "_" + Naam;
        }
    }
}
