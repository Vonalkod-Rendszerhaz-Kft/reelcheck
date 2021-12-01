using System.Web.Mvc;

namespace ReelCheck.Web.Areas.Intervention
{
    public class InterventionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Intervention";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Intervention_default",
                "Intervention/{controller}/{action}/{interventionName}",
                new { controller = "Intervention", action = "Index", interventionName = UrlParameter.Optional }
            );
        }
    }
}