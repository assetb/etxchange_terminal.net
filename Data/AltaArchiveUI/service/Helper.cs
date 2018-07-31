using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveUI.service {
    public static class Helper {


        private static Dictionary<string, string> extimages = new Dictionary<string, string>();

        public static string GetImageSource(string ext) {
            if(extimages.Count() < 1) fillextimage();
            return extimages.First(a => a.Key.Equals(ext)).Value;
        }


        private static void fillextimage() {
            extimages.Add("doc", "1");
            extimages.Add("docx", "2");
            extimages.Add("html", "3");
            extimages.Add("jpg", "4");
            extimages.Add("pdf", "5");
            extimages.Add("png", "6");
            extimages.Add("xls", "7");
            extimages.Add("xlsx", "8");
            extimages.Add("jpeg", "9");
        }
    }



}
