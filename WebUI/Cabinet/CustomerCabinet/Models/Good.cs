﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CustomerCabinet.Models
{
    public class Good
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Display(Name = "Наименование")]
        public string name { get; set; }
    }
}