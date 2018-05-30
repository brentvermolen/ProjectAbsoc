using BL.Domain;
using BL.Domain.ActeurKlassen;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TraktContext : DbContext
    {
        public TraktContext() : base("DefaultConnection")
        {
            Database.SetInitializer(new TraktInitializer());

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

            modelBuilder.Entity<Gebruiker>()
                .HasMany(g => g.Archief)
                .WithMany(a => a.Gebruiker);

            modelBuilder.Entity<ActeurFilm>()
                .HasRequired(a => a.Film)
                .WithMany(f => f.Acteurs)
                .HasForeignKey(a => a.FilmID);

            modelBuilder.Entity<ActeurFilm>()
                .HasRequired(a => a.Acteur)
                .WithMany(a => a.Films)
                .HasForeignKey(a => a.ActeurID);

            modelBuilder.Entity<Film>()
                .HasMany(f => f.Archieven)
                .WithMany(b => b.Films);

            modelBuilder.Entity<Film>()
                .HasMany(f => f.Tags)
                .WithMany(t => t.Films);

            modelBuilder.Entity<Film>()
                .HasRequired(f => f.Collectie)
                .WithMany(c => c.Films)
                .HasForeignKey(f => f.CollectieID);

            modelBuilder.Entity<Aflevering>()
                .HasRequired(a => a.Serie)
                .WithMany(s => s.Afleveringen)
                .HasForeignKey(a => a.SerieID);

            modelBuilder.Entity<Aflevering>()
                .HasMany(a => a.Archieven)
                .WithMany(a => a.Afleveringen);

            modelBuilder.Entity<ActeurSerie>()
                .HasRequired(a => a.Serie)
                .WithMany(s => s.Acteurs)
                .HasForeignKey(a => a.SerieID);

            modelBuilder.Entity<ActeurSerie>()
                .HasRequired(a => a.Acteur)
                .WithMany(a => a.Series)
                .HasForeignKey(a => a.ActeurID);
        }

        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Gemeente> Gemeentes { get; set; }

        public DbSet<Film> Films { get; set; }
        public DbSet<Serie> Series { get; set; }
        public DbSet<Aflevering> Afleveringen { get; set; }
        public DbSet<Acteur> Acteurs { get; set; }
        public DbSet<Archief> Archieven { get; set; }
        public DbSet<Collectie> Collecties { get; set; }
    }
}
