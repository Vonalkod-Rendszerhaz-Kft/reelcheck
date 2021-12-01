using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using VRH.Common;
using VRH.Log4Pro.MultiLanguageManager;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;

using ReelCheck.EventHub.Contracts;
using ReelCheck.Core;

namespace ReelCheck.Web.Areas.Intervention.Controllers
{
    public class LABELKController : Controller
    {
        private const string PAR_USERNAME = "userName";

        // GET: Intervention/LABELK/Run
        public JsonResult Run(string interventionName)
        {
            // Weben nem work az Assembly.GetEntryAssembly() hívás, de ez átveri...
            WebInterventions.SetEntryAssembly();
            // TODO: később megvalósítandó egy általános intervention kezelő, mely az EventHub message contract alapján automatikusan képes dolgozni!!! Addig ez az implementáció:
            List<KeyAndValue> parameters = new List<KeyAndValue>();
            foreach (string item in Request.QueryString.Keys)
            {
                parameters.Add(
                    new KeyAndValue()
                    {
                        Key = item,
                        Value = Request.QueryString[item]
                    });
            }
            try
            {
                //WA20180810: Laci kérése, ha nincs "userName" nevű paraméter 
                //            akkor adjunk hozzá egyet a helyi user nevével, ha be van jelentkezve
                if (User.Identity.IsAuthenticated)
                {   // ha van bejelentkezett felhasználó
                    if (!parameters.Any(x => x.Key.Equals(PAR_USERNAME, StringComparison.OrdinalIgnoreCase)))
                    {   //ha nem tartalmaz a paraméterlista "userName" nevűt
                        parameters.Add(new KeyAndValue { Key = PAR_USERNAME, Value = User.Identity.Name });
                    }
                }
                return Json(
                    new ReturnInfoJSON()
                    {
                        ReturnMessage = MultiLanguageManager.GetTranslation(
                            WebInterventions.RunIntervention(interventionName, parameters, "LABELK")),
                    }
                    , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(
                    new ReturnInfoJSON()
                    {
                        ReturnValue = -1,
                        ReturnMessage = MultiLanguageManager.GetTranslation(ex.Message),
                    }
                    , JsonRequestBehavior.AllowGet);
            }
        }
    }
}