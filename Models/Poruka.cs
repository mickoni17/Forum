using IEP_Projekat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Poruka", Schema = "ForumDB")]
    public class Poruka
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PorukaId { get; set; }

        [ForeignKey("Korisnik")]
        public int KorsinikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }

        [ForeignKey("Kanal")]
        public int KanalId { get; set; }
        public virtual Kanal Kanal { get; set; }

        public string Tekst { get; set; }
        public DateTime DatumKreiranja { get; set; }

    }
}