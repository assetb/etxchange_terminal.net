using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Karazhira.Models
{
    public class Good
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Display(Name = "Наименование")]
        public string name { get; set; }
    }
}