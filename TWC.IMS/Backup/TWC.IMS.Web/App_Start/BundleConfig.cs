using System.Web;
using System.Web.Optimization;

namespace TWC.IMS.Web
{
    public class BundleConfig
    {
        private BundleConfig() { }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles != null)
            {
#if DEBUG
                bundles.IgnoreList.Clear();
#endif
                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate*"));

                bundles.Add(new ScriptBundle("~/bundles/jsplumb").Include(
                            "~/Scripts/jsplumb/jsplumb.js"));

                // Use the development version of Modernizr to develop with and learn from. Then, when you're
                // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

                bundles.Add(new ScriptBundle("~/bundles/application").Include(
                          "~/Scripts/application/application.js",
                          "~/Scripts/application/form-change-listener.js",
                          "~/Scripts/application/keep-session-alive.js"));

                bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                          "~/Content/fontawesome.css", new CssRewriteUrlTransform()));

                bundles.Add(new StyleBundle("~/Content/css").Include(
                          "~/Content/webkit-scrollbar.css",
                          "~/Content/bootstrap.css",
                          "~/Content/site.css",
                          "~/Content/dynamicworkflow.css"));
            }
        }
    }
}
