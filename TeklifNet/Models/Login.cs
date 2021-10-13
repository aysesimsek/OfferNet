using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeklifNet.Models
{
    public class Login
    {

        [Display(Name = "MAIL")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Geçersiz Mail Adresi")]
        public string MAIL { get; set; }


        [Display(Name = "SIFRE")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string SIFRE { get; set; }


        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}