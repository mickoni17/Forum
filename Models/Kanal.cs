using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Kanali", Schema = "ForumDB")]
    public class Kanal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int KanalId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Naziv { get; set; }
        public DateTime VremeOtvaranja { get; set; }
        public bool Status { get; set; } // false - neaktivan ; true - aktivan

        [ForeignKey("Otvarac")]
        public int OtvaracId { get; set; }
        public virtual Korisnik Otvarac { get; set; }
    }
}