using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.Analytic
{
    public class GeneralInfo
    {
        public int count { get; set; } = 0;
        public int expected { get; set; } = 0;
        public int finished { get; set; } = 0;
        public int notHeld { get; set; } = 0;
    }
}