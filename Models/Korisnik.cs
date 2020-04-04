using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Korisnici", Schema = "ForumDB")]
    public class Korisnik
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int KorisnikId { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string Ime { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string Prezime { get; set; }
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Mail { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        public string Lozinka { get; set; }
        public int Tokeni { get; set; }
        public int Uloga { get; set; } //0 - korisnik ; 1 - agent ; 2 - admin
        public int Status { get; set; } // 0 - neaktivan ; 1 - aktivan ; 2 - verifikacija
        public bool Private { get; set; } // false - otvoren ; true - zakljucan
        public string Validation { get; set; }
        public bool NewPassword { get; set; }

    }
}