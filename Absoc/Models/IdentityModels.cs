using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Absoc.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class MyUser : IdentityUser<int, MyLogin, MyUserRole, MyClaim>, IUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<MyUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            Postcode = "0000";

            return userIdentity;
        }

        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Geboortedatum { get; set; }
        public string Adres { get; set; }
        public string Postcode { get; set; }

        string IUser<string>.Id => this.Id.ToString();
    }

    public class MyUserRole : IdentityUserRole<int> { }

    public class MyRole : IdentityRole<int, MyUserRole> { }

    public class MyClaim : IdentityUserClaim<int>
    {

    }
    public class MyLogin : IdentityUserLogin<int> { }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<MyUser, MyRole, int, MyLogin, MyUserRole, MyClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //map entities to their tables
            modelBuilder.Entity<MyUser>().ToTable("Gebruikers");
            modelBuilder.Entity<MyRole>().ToTable("Roles");
            modelBuilder.Entity<MyUserRole>().ToTable("GebruikerRoles");
            modelBuilder.Entity<MyClaim>().ToTable("GebruikersClaims");
            modelBuilder.Entity<MyLogin>().ToTable("GebruikerLogins");
            //set autoincrement props
            modelBuilder.Entity<MyUser>().Property(r => r.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyRole>().Property(r => r.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class CustomUserValidator<TUser> : IIdentityValidator<TUser> where TUser : class, Microsoft.AspNet.Identity.IUser
    {
        private static readonly Regex EmailRegex = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly UserManager<MyUser, int> _manager;

        public CustomUserValidator() { }
        public CustomUserValidator(UserManager<MyUser, int> manager) { _manager = manager; }

        public async Task<IdentityResult> ValidateAsync(TUser item)
        {
            var errors = new List<string>();

            if (!EmailRegex.IsMatch(item.UserName))
                errors.Add("Gelieve een geldig e-mail adres op te geven.");

            if (_manager != null)
            {
                var otherAccount = await _manager.FindByNameAsync(item.UserName);
                if (otherAccount != null && otherAccount.Id != int.Parse(item.Id))
                    errors.Add("Dit e-mail adres is al in gebruik.");
            }

            return errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success;
        }
    }
}