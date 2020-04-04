using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Porudzbine", Schema = "ForumDB")]
    public class Porudzbine
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Korisnik")]
        public int KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }
        [Key]
        [Column(Order = 1)]
        public DateTime Vreme { get; set; }
        public int Tokeni { get; set; }
        public int Cena { get; set; }

    }
}