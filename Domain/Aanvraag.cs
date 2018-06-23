using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Aanvraag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FilmId { get; set; }
        public DateTime? AangevraagOp { get; set; }

        public int GebruikerId { get; set; }
        public virtual Gebruiker AangevraagdDoor { get; set; }
    }
}
