using System.Web.Mvc;

namespace CustomFVS.Areas.Reelcheck
{
    public class ReelcheckAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Reelcheck";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Reelcheck_default",
                "Reelcheck/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}