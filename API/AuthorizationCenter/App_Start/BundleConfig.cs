using System.Web.Optimization;

namespace AuthorizationCenter.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/jquery").Include("~/scripts/jquery-3.1.1.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/foundation").Include("~/scripts/foundation/foundation.js"));

            bundles.Add(new StyleBundle("~/content/founcation").Include("~/Content/foundation/foundation.css", "~/Content/foundation/normalize.css"));
            bundles.Add(new StyleBundle("~/content/styles").Include("~/Сontent/Site.css"));
        }
    }
}