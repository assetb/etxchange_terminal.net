using AltaBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models
{
    public class Table<T>
    {
        public int countShowItems { get; set; } = 0;
        public int countItems { get; set; } = 0;
        public int currentPage { get; set; } = 0;
        public int countPages { get; set; } = 0;
        public List<T> rows { get; set; } = new List<T>();
    }
}