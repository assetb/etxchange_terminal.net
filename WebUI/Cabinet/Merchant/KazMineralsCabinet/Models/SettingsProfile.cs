using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KazMineralsCabinet.Models
{
    public class SettingsProfile
    {
        [Display(Name = "Запоминать последнюю страницу")]
        public bool isSaveLastPage { get; set; }

        [Display(Name = "Страница по умолчанию")]
        public String defaultPage { get; set; }
    }
}