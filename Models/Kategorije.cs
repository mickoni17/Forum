using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    [Table("Kategorije", Schema = "ForumDB")]
    public class Kategorije
    {
        [Key]
        [Required]
        [MaxLength(30)]
        [MinLength(1)]
        public string Naziv { get; set; }
    }
}