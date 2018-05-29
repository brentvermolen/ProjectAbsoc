using BL.Domain.IdentityKlassen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Domain
{
    public class Gebruiker
    {
        [Key]
        public int ID { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Geboortedatum { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        public string Adres { get; set; }
        public string Postcode { get; set; }
        public virtual Gemeente Gemeente { get; set; }

        public bool EmailConfirmed { get; set; }
        public DateTime? LastConfirmationMail { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        [StringLength(256)]
        public string UserName { get; set; }



        public virtual ICollection<GebruikerLogin> GebruikerLogins { get; set; }
        public virtual ICollection<GebruikersClaim> GebruikersClaims { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
