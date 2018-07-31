using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.Models
{
    public class FormLogin
    {
        public string errorMessage { get; set; }
        public string login { get; set; }
        public string pass { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool remembeMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}