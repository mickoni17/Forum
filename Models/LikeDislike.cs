using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    //Pitanje sta za key? Ako uopste i mora.
    [Table("Lajkovi", Schema = "ForumDB")]
    public class LikeDislike
    {
        public int Ocena { get; set; } //-1 dislike, 1 like

        [Key]
        [Column(Order = 0)]
        [ForeignKey("Korisnik")]
        public int KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }

        [Key]
        [Column(Order = 2)]
        //Moze da bude null ukoliko je pitanje lajkovano
        [ForeignKey("Odgovor")]
        public int OdgovorId { get; set; }
        public virtual Odgovor Odgovor { get; set; }

    }
}