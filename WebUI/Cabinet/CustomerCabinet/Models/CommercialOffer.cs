using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CustomerCabinet.Models
{
    public class CommercialOffer
    {
        [HiddenInput(DisplayValue = false)]
        public int id {get;set;}

        [Display(Name = "Описание")]
        public string description { get; set; }

        [Display(Name = "Коммерческое предложение")]
        public string file { get; set; }
    }
}