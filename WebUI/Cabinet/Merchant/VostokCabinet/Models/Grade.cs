using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VostokCabinet.Models
{
    public class Grade
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int sourceId { get; set; }

        [Display(Name = "Источник")]
        public string source { get; set; }

        [Display(Name = "Отзыв")]
        public string description { get; set; }
    }
}