using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SupplierCabinet.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/main").Include(
                "~/scripts/lib/jquery/dist/jquery.min.js",
                "~/scripts/lib/foundation/js/foundation.min.js",
                "~/scripts/lib/foundation/js/foundation/foundation.dropdown.js",
                "~/scripts/lib/foundation/js/foundation/foundation.topbar.js",
                "~/scripts/lib/foundation/js/foundation/foundation.reveal.js",
                "~/scripts/lib/requirejs/require.js",
                "~/scripts/app/global_settings.js"
                //"~/scripts/app/require-config.js"
            ));
            bundles.Add(new StyleBundle("~/bundles/css/main").Include(
                "~/Content/Site.css",
                "~/scripts/lib/foundation/css/foundation.min.css",
                "~/scripts/lib/foundation-datepicker/css/foundation-datepicker.min.css",
                "~/scripts/lib/foundation/css/normalize.min.css",
                "~/scripts/lib/foundation-icon-fonts/foundation-icons.css",
                "~/Content/GeneralStyle.css"
                ));
        }
    }
}