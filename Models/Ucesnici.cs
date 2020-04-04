﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Ucesnici", Schema = "ForumDB")]
    public class Ucesnici
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Kanal")]
        public int KanalId { get; set; }
        public virtual Kanal Kanal { get; set; }
        
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Klijent")]
        public int KlijentId { get; set; }
        public virtual ICollection<Korisnik> Klijent { get; set; } //1 klijent i vise agenata

    }
}