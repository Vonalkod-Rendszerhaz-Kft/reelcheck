using ReelCheck.Core;
using ReelCheck.EventHub.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Web.Areas.Intervention.Controllers
{
    public static class WebInterventions
    {
        public static String RunIntervention(string interventionName, List<KeyAndValue> parameters, string wsId)
        {
            switch (interventionName.ToUpper())
            {
                case "RE-PRINT":
                    return WebInterventions.RePrint(wsId);
                case "NO-PRINT":
                    return WebInterventions.SkipPrint(wsId);
                case "LOGIN":
                    return WebInterventions.Login(parameters, wsId);
                case "LOGOUT":
                    return WebInterventions.Logout(parameters, wsId);
                case "HHMODEON":
                    return WebInterventions.HandHeldModeOn(parameters, wsId);
                case "HHMODEOFF":
                    return WebInterventions.HandHeldModeOff(wsId);
                case "GSSTART":
                    return WebInterventions.GoldenSampleStart(wsId);
                case "GSSTOP":
                    return WebInterventions.GoldenSampleStop(wsId);
                case "GSDUESKIP":
                    return WebInterventions.GoldenSampleDeleteDue(wsId);
                case "GSSETDUE":
                    return WebInterventions.GoldenSampleSetDueNow(wsId);
                case "BLOCK":
                    return WebInterventions.Block(wsId);
                case "UNBLOCK":
                    return WebInterventions.UnBlock(wsId);
                default:
                    throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.UnknownIntervention)));
            }
        }

        public static string GoldenSampleStart(string ws = "LabelD")
        {
            var rq = new InterventionContract.GoldenSampleMode.Enter();            
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.Enter, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Enter.Success));
        }

        public static string GoldenSampleStop(string ws = "LabelD")
        {
            var rq = new InterventionContract.GoldenSampleMode.Exit();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.Exit, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.Exit.Success));
        }

        public static string GoldenSampleDeleteDue(string ws = "LabelD")
        {
            var rq = new InterventionContract.GoldenSampleMode.DeleteDue();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.DeleteDue, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.DeleteDue.Success));
        }

        public static string GoldenSampleSetDueNow(string ws = "LabelD")
        {
            var rq = new InterventionContract.GoldenSampleMode.SetDue();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.SetDue, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.GoldenSample.SetDueNow.Success));
        }

        public static string Block(string ws = "LabelD")
        {
            var rq = new InterventionContract.Blocking.Block();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.Blocking.Block, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.Block.Success));
        }

        public static string UnBlock(string ws = "LabelD")
        {
            var rq = new InterventionContract.Blocking.UnBlock();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.Blocking.UnBlock, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Blocking.UnBlock.Success));
        }

        public static string Logout(List<KeyAndValue> parameters, string ws = "LabelD")
        {
            KeyAndValue username = parameters.FirstOrDefault(x => x.Key.ToUpper() == "USERNAME");
            if (username == null || string.IsNullOrEmpty(username.Value))
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.NoUserName)));
            }
            var rq = new InterventionContract.InActivate()
            {
                UserName = username.Value,
            };
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.InActivate, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Inactivate.Success));
        }

        public static string Login(List<KeyAndValue> parameters, string ws = "LabelD")
        {
            KeyAndValue username = parameters.FirstOrDefault(x => x.Key.ToUpper() == "USERNAME");
            if (username == null || string.IsNullOrEmpty(username.Value))
            {
                throw new Exception(MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Activate.NoUserName)));
            }
            var rq = new InterventionContract.Activate()
            {
                UserName = username.Value,
            };
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.Activate, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.Activate.Success));
        }

        public static string SkipPrint(string ws = "LabelD")
        {
            var rq = new InterventionContract.SkipPrint();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.SkipPrint, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.SkipPrint.Success));
        }

        public static string RePrint(string ws = "LabelD")
        {
            var rq = new InterventionContract.RePrint();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.RePrint, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.RePrint.Success));
        }

        public static string HandHeldModeOn(List<KeyAndValue> parameters, string ws = "LabelD")
        {
            string fvsCode = parameters.FirstOrDefault(x => x.Key.ToUpper() == "FVS")?.Value;
            string pn = parameters.FirstOrDefault(x => x.Key.ToUpper() == "PN")?.Value;
            string lot = parameters.FirstOrDefault(x => x.Key.ToUpper() == "LN")?.Value;
            string qty = parameters.FirstOrDefault(x => x.Key.ToUpper() == "QTY")?.Value;
            var rq = new InterventionContract.ManualMode.On()
            {
                FVSCode = fvsCode,
                PartNumber = pn,
                LotNumber = lot,
                Qty = qty,
            };
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.ManualMode.On, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.On.Success));
        }

        private static string HandHeldModeOff(string ws = "LabelD")
        {
            var rq = new InterventionContract.ManualMode.Off();
            EventHubCore.Call<RedisPubSubChannel, InterventionContract.ManualMode.Off, InterventionContract.Result>(ws, rq, new TimeSpan(0, 0, 5));
            return MultiLanguageManager.GetWordCodeFromClassType(typeof(TrueWordCodes.ReelCheckService.UiMessage.Interventions.ManulaMode.Off.Success));
        }

        public static void SetEntryAssembly()
        {
            if (Assembly.GetEntryAssembly() == null)
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                AppDomainManager manager = new AppDomainManager();
                FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
                entryAssemblyfield.SetValue(manager, assembly);
                AppDomain domain = AppDomain.CurrentDomain;
                FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
                domainManagerField.SetValue(domain, manager);
            }
        }
    }
}