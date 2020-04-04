using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Pitanja", Schema = "ForumDB")]
    public class Pitanje
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PitanjeId { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(1)]
        public string Naslov { get; set; }
        [Required]
        [MinLength(1)]
        public string Tekst { get; set; }
        public string Slika { get; set; }
        public DateTime VremePravljenja { get; set; } 
        public DateTime VremeZakljucavanja { get; set; }

        public string ImagePath { get; set; }

        [ForeignKey("Kategorija")]
        public string KategorijaId { get; set; }
        public virtual Kategorije Kategorija { get; set; }

        [ForeignKey("Autor")]
        public int AutorId { get; set; }
        public virtual Korisnik Autor { get; set; }
    }
}