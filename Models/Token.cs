using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Tokeni", Schema = "ForumDB")]
    public class Token
    {
        [Key]
        [Required]
        [StringLength(1)]
        public string Sorta { get; set; }
        public double Vrednost { get; set; }
    }
}