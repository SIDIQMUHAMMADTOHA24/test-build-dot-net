using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace App.WebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin");
            Environment.SetEnvironmentVariable("PATH", path + ";" + binDir);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcHandler.DisableMvcResponseHeader = true;
            PreSendRequestHeaders += Application_PreSendRequestHeaders;
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            var server = HttpContext.Current;
            if (server != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
            }
        }
    }
}
