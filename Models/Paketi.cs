using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Paketi", Schema = "ForumDB")]
    public class Paketi
    {
        [Key]
        public int PaketId { get; set; } //1 za kanal, 2 silver, 3 gold, 4 platinum

        public int Kolicina { get; set; }
        public int? Cena { get; set; }

    }
}