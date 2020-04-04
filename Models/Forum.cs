using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    public class Forum : DbContext
    {
        public Forum() : base("name=ForumDB")
        {
            Database.SetInitializer<Forum>(new CreateDatabaseIfNotExists<Forum>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<ForumPitanja> _ForumPitanja { get; set; }
        public DbSet<Kanal> _Kanal { get; set; }
        public DbSet<Kategorije> _Kategorije { get; set; }
        public DbSet<Korisnik> _Korisnik { get; set; }
        public DbSet<LikeDislike> _LikeDislike { get; set; }
        public DbSet<Odgovor> _Odgovor { get; set; }
        public DbSet<Pitanje> _Pitanje { get; set; }
        public DbSet<Token> _Token { get; set; }
        public DbSet<Ucesnici> _Ucesnici { get; set; }
        public DbSet<Porudzbine> _Porudzbine { get; set; }
        public DbSet<Paketi> _Paketi { get; set; }
        public DbSet<Poruka> _Poruka { get; set; }
        public DbSet<ChannelConnections> _ChannelConnections { get; set; }
    }
}
