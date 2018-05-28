using BL.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AbsocContext : DbContext
    {
        public AbsocContext() : base("DefaultConnection")
        {
            Database.SetInitializer(new AbsocInitializer());

            try
            {
                Database.Initialize(false);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Gebruiker>()
                .HasMany(e => e.GebruikersClaims)
                .WithRequired(e => e.Gebruiker)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Gebruiker>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Gebruikers)
                .Map(m =>
                {
                    m.ToTable("GebruikerRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });

            modelBuilder.Entity<Gebruiker>()
                .HasRequired(g => g.Gemeente)
                .WithMany(p => p.Gebruikers)
                .HasForeignKey(g => g.Postcode);
        }

        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Boek> Boeken { get; set; }
        public DbSet<Gemeente> Gemeentes { get; set; }
    }
}
