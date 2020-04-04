using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("ForumPitanja", Schema = "ForumDB")]
    public class ForumPitanja
    {
        [Key]
        [ForeignKey("Pitanje")]
        public int PitanjeId { get; set; }
        public virtual Pitanje Pitanje { get; set; }
    }
}