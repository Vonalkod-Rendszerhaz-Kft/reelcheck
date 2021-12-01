using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ReelCheck.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Vrh.Web.Menu inicializáló beállításai
            Vrh.Web.Menu.Global.IsUseAuthentication = true;
            Vrh.Web.Menu.Global.CustomerLogo = "~/Images/delphilogo.png";
        }
    }
}
