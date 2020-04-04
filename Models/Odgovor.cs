using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Odgovori", Schema = "ForumDB")]
    public class Odgovor
    {
        [Key]
        public int OdgovorId { get; set; }
        public DateTime Vreme { get; set; }
        [MinLength(1)]
        public string Tekst { get; set; }

        [ForeignKey("Pitanje")]
        public int PitanjeId { get; set; }
        public virtual Pitanje Pitanje {get;set;}

        [ForeignKey("Korisnik")]
        public int KorsinikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }
        
        [ForeignKey("ReplyNa")]
        public int? ReplyNaId { get; set; }
        public virtual Odgovor ReplyNa { get; set; }
    }
}