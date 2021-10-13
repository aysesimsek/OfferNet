using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeklifNet.Models
{
    public class Registration
    {
        [Display(Name = "KULL_ADI")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string KULL_ADI { get; set; }

        [Display(Name = "MAIL")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        [DataType(DataType.EmailAddress, ErrorMessage ="Geçersiz Email Adresi!")]
        public string MAIL { get; set; }

        //[Display(Name = "TELEFON")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //public string TELEFON { get; set; }

        [Display(Name = "SIRKET_UNVANI")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_UNVANI { get; set; }

        //[Display(Name = "SIRKET_YETKILI")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //public string SIRKET_YETKILI { get; set; }

        [Display(Name = "SIRKET_ADRES")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_ADRES { get; set; }

        [Display(Name = "SIRKET_IL")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_IL { get; set; }

        [Display(Name = "SIRKET_ILCE")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_ILCE { get; set; }

        [Display(Name = "SIRKET_VERGIDAIRESI")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_VERGIDAIRESI { get; set; }

        [Display(Name = "SIRKET_VERGINUMARASI")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_VERGINUMARASI { get; set; }

        //[Display(Name = "SIRKET_TELEFON")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //public string SIRKET_TELEFON { get; set; }

        [Display(Name = "SIRKET_GSM")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_GSM { get; set; }

        [Display(Name = "SIRKET_FAX")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_FAX { get; set; }

        [Display(Name = "SIRKET_MAIL")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        public string SIRKET_MAIL { get; set; }

        //[Display(Name = "SIRKET_POSTAKODU")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //public string SIRKET_POSTAKODU { get; set; }

        [Display(Name = "SIFRE")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bu alan boş bırakılamaz!")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 haneli olmalıdır.")]
        public string SIFRE { get; set; }

        [Display(Name = "SIFREONAY")]
        [DataType(DataType.Password)]
        [Compare("SIFRE", ErrorMessage = "Şifre eşleşmemektedir!")]
        public string SIFREONAY { get; set; }

    }
}