using System;
using Vrh.Redis.DataPoolHandler;

namespace ReelCheck.Core
{
    /// <summary>
    /// Redis pool adatainak definiciója
    /// </summary>
    internal static class DataPoolDefinition
    {
        /// <summary>
        /// Pool verziója
        /// </summary>
        internal const string POOLVERSION = "3.13.0";

        /// <summary>
        /// Ellenőrzi, és ha szükséges létrehozza a Redis pool-t
        /// </summary>
        /// <param name="ph">poolhandler</param>
        /// <param name="iw">instancewriter</param>
        public static void CheckPool(PoolHandler ph, InstanceWriter iw)
        {
            // POOL
            if (ph.IsPoolExists())
            {
                if (ph.GetPoolVersion() != POOLVERSION)
                {
                    ph.RemovePool();
                }
            }
            if (!ph.IsPoolExists())
            {
                ph.RegisterPool(POOLVERSION);
                iw.RemoveInstance();
            }
            else
            {
                return;
            }
            //
            // DEFINED KEYS

            // UIMESSAGE INTERFACE
            for (int i = 1; i < 6; i++)
            {
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = $"WS.UIMESSAGE{i}" });
            }
            // WS - Munkahely szintű adatok
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.StationType.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.UserName.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.Station.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.Process.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.DateTime, Name = DataPoolDefinition.WS.MainStatus.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.IdStatus.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.PrintStatus.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.CheckStatus.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.ReelCheckMode.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.ManualData.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.OperationBlocked.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.WS.BlockReasonCode.GetRedisKey() });
            // GS - aranyminta teszthez kapcsolódó adatok
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.DueMode.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.DueTime.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.GSCycles.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.StartTime.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.Due.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.LastStart.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.LastResult.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.CyclesFromLast.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.Active.GetRedisKey() });
            ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.GS.LastSuccessTest.GetRedisKey() });
            // REEL - tekercs szintű adatok
            for (int i = 1; i < 5; i++)
            {
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.BCList.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.BCListId.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.BCListCheck.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IdData.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IdDataAck.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.LabelId.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SVendor.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SPN.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SLot.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SFVS.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SRSRN.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.SQty.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IMTSId.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IFVS.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IQty.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.ILot.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.ILotDate.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.ILotSN.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IPN.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IVendor.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.DateTime, Name = DataPoolDefinition.Reel.CycleStartTimeStamp.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.CMTSID.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.CFVS.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.Boolean, Name = DataPoolDefinition.Reel.Empty.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IdentificationReadResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IdentificationRegexCheckResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IdentificationExternalCheckResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.PrintResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.StickingResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.CheckReadResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.CheckRegexCheckResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.CheckExternalBookingResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.ReelResult.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.ErrorMessage.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.IMSL.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.Boolean, Name = DataPoolDefinition.Reel.GSMode.GetRedisKey(i) });
                ph.RegisterKey(new RedisPoolKeyDefination() { DataType = DataType.String, Name = DataPoolDefinition.Reel.BackGroundSystemMessage.GetRedisKey(i) });
            }
            // INSTANCE
            if (!iw.IsPoolInstanceExists())
            {
                iw.RegisterInstance();
            }
        }

        /// <summary>
        /// Workstation szintű adatok
        /// </summary>
        internal enum WS
        {
            /// <summary>
            /// Gépazonosító
            /// </summary>
            StationType,
            /// <summary>
            /// Bejelentkezett felhazsnáló
            /// </summary>
            UserName,
            /// <summary>
            /// Munkaállomás 
            /// </summary>
            Station,
            /// <summary>
            /// Folyamat típusa
            /// </summary>
            Process,
            /// <summary>
            /// Státusz a munkaállomáson (felhazsnálónak szánt információs szöveg)
            /// </summary>
            MainStatus,
            /// <summary>
            /// UI-ra szánt üszenetek a működés részeleteiről
            /// </summary>
            UIMessage,
            /// <summary>
            /// Azonosítás állapota
            /// </summary>
            IdStatus,
            /// <summary>
            /// A nyomtatás állapota
            /// </summary>
            PrintStatus,
            /// <summary>
            /// Az nyomtatott címke ellenőrzésének állapota
            /// </summary>
            CheckStatus,
            /// <summary>
            /// Az állomás állapotai
            /// </summary>
            ReelCheckMode,
            /// <summary>
            /// Manuális adatbeviteltől étrkezett adatok
            /// </summary>
            ManualData,
            /// <summary>
            /// Jelzi, hogy blokkolva van-e a munkahely
            /// </summary>
            OperationBlocked,
            /// <summary>
            /// Ablokolásd oka
            /// </summary>
            BlockReasonCode,
        }

        /// <summary>
        /// Golden sample változók
        /// </summary>
        public enum GS
        {
            Due,
            DueMode,
            DueTime,
            StartTime,
            GSCycles,
            LastStart,
            LastSuccessTest,
            LastResult,            
            CyclesFromLast,
            Active,
        }

        /// <summary>
        /// Redis kulcs visszaadása a workstation szintű adatok esetén
        /// </summary>
        /// <param name="ws">ws mező enum, amelynek a kulcsát kérjük</param>
        /// <param name="msgpos">UIMESSAGE esetén az üzenet poziciója</param>
        /// <returns>redis kulcs</returns>
        internal static string GetRedisKey(this WS ws, int msgpos = 1)
        {
            if (ws != WS.UIMessage)
            {
                return $"{ws.GetType().Name.ToUpper()}.{ws.ToString().ToUpper()}";
            }
            else
            {
                return $"{ws.GetType().Name.ToUpper()}.{ws.ToString().ToUpper()}{msgpos}";
            }
        }

        internal static string GetRedisKey(this GS gs)
        {
            return $"{gs.GetType().Name.ToUpper()}.{gs.ToString().ToUpper()}";
        }

        /// <summary>
        /// Tekercs szintű adatok 
        /// </summary>
        internal enum Reel
        {
            /// <summary>
            /// az olvasótól az azonosítási fázisban beérkezett adatstringek
            /// </summary>
            BCListId,
            /// <summary>
            /// BCList
            /// </summary>
            BCList,
            /// <summary>
            /// az olvasótól az ellenőrző fázisban beérkezett adatstringek
            /// </summary>
            BCListCheck,
            /// <summary>
            /// a háttér rendszernek elküldött tranzakciós üzenet
            /// </summary>
            IdData,
            /// <summary>
            /// a háttér rendszer válasza a tranzakciós üzenetre
            /// </summary>
            IdDataAck,
            /// <summary>
            /// a ciklusban felismert címke azonosítója
            /// </summary>
            LabelId,
            /// <summary>
            /// a ciklusban felismert beszállítói azonosító (beszállító kód)
            /// </summary>
            SVendor,
            /// <summary>
            /// a ciklusban felismert beszállítói cikkszám
            /// </summary>
            SPN,
            /// <summary>
            /// a ciklusban felismert beszállítói LOT szám
            /// </summary>
            SLot,
            /// <summary>
            /// a ciklusban felismert beszállítói FVS/c9023 kód
            /// </summary>
            SFVS,
            /// <summary>
            /// a ciklusban felismert beszállítói egyedi tekercs azonosító
            /// </summary>
            SRSRN,
            /// <summary>
            /// a ciklusban felismert beszállítói mennyiség
            /// </summary>
            SQty,
            /// <summary>
            /// a ciklusban felismert belső MTS azonosító
            /// </summary>
            IMTSId,
            /// <summary>
            /// a ciklusban felismert belső FVS kód
            /// </summary>
            IFVS,
            /// <summary>
            /// a ciklusban felismert mennyiség
            /// </summary>
            IQty,
            /// <summary>
            /// a ciklusban felismert belső LOT szám
            /// </summary>
            ILot,
            /// <summary>
            /// belső Lot-ban kódolt dátum
            /// </summary>
            ILotDate,
            /// <summary>
            /// Belső lotban kódolt serialnumber
            /// </summary>
            ILotSN,
            /// <summary>
            /// A ciklusban felismert MSL kód
            /// </summary>
            IMSL,
            /// <summary>
            /// a ciklusban felismert belső cikkszám
            /// </summary>
            IPN,
            /// <summary>
            /// a ciklusban felismert beszállítói azonosító (Delphi kód)
            /// </summary>
            IVendor,
            /// <summary>
            /// indítás időbélyege
            /// </summary>
            CycleStartTimeStamp,
            /// <summary>
            /// Check camera által olvasott MTSID adat
            /// </summary>
            CMTSID,
            /// <summary>
            /// Check kamera által olvasott FVS adat
            /// </summary>
            CFVS,
            /// <summary>
            /// Üres tekercs pozició
            /// </summary>
            Empty,
            /// <summary>
            /// Azonosító kamera olvasásának az eredménye
            /// </summary>
            IdentificationReadResult,
            /// <summary>
            /// Azonosítás során olvasott adatok regex kiértékelésének eredemnénye
            /// </summary>
            IdentificationRegexCheckResult,
            /// <summary>
            /// Külső rendeszerben val elelnőrzés eredménye
            /// </summary>
            IdentificationExternalCheckResult,
            /// <summary>
            /// Nyomtatás eredeménye
            /// </summary>
            PrintResult,
            /// <summary>
            /// Ragasztás eredménye
            /// </summary>
            StickingResult,
            /// <summary>
            /// ellenőrző olvasás eredeménye
            /// </summary>
            CheckReadResult,
            /// <summary>
            /// ellenőrző olvasás által olvasott adatok regex ellenőrzése
            /// </summary>
            CheckRegexCheckResult,
            /// <summary>
            /// Külső rendeszerbe való végső könyvelés eredménye
            /// </summary>
            CheckExternalBookingResult,
            /// <summary>
            /// A tekercs kapott státusza
            /// </summary>
            ReelResult,
            /// <summary>
            /// Tekercs hibás státuszához tartozó hibaszöveg
            /// </summary>
            ErrorMessage,
            /// <summary>
            /// Jelzi, hogy a tekercs aranyminta ellenőrzésben vesz részt
            /// </summary>
            GSMode,
            /// <summary>
            /// A háttér rendszer üzenete az azonosításnál
            /// </summary>
            BackGroundSystemMessage,
        };

        /// <summary>
        /// Redis kulcs visszaadása a tekercs szintű adatok esetén
        /// </summary>
        /// <param name="reel">reel mező enum, amelynek a kulcsát kérjük</param>
        /// <param name="workstationType">munkehely típusa</param>
        /// <param name="position">Ha automata munkahely, akkor a tekercs pozició (egyéb)</param>
        /// <returns></returns>
        internal static string GetRedisKey(this Reel reel, int position = 1)
        {
            string pos = position.ToString();
            return $"SECT{pos}.{reel.GetMyUglyName()}";
        }

        /// <summary>
        /// A tekercs szintú adatok randa datapool kulcs nevét adja (ahol végképp enem lehet az ENUM elnevezést kulcsra váltani)
        /// </summary>
        /// <param name="reel">Tekercs adat azonosító enum</param>
        /// <returns>a használt redis datapool kulcs</returns>
        internal static string GetMyUglyName(this Reel reel)
        {
            switch (reel)
            {
                case Reel.IdentificationReadResult:
                    return "STATUS_ID_READ";
                case Reel.IdentificationRegexCheckResult:
                    return "STATUS_ID_RCCHECK";
                case Reel.IdentificationExternalCheckResult:
                    return "STATUS_ID_EXTCHECK";
                case Reel.PrintResult:
                    return "STATUS_PRN_PRINT";
                case Reel.StickingResult:
                    return "STATUS_PRN_APPLY";
                case Reel.ReelResult:
                    return "STATUS_EXIT";
                case Reel.CheckReadResult:
                    return "STATUS_CHK_READ";
                case Reel.CheckRegexCheckResult:
                    return "STATUS_CHK_RCCHECK";
                case Reel.CheckExternalBookingResult:
                    return "STATUS_CHK_BOOKING";
                default:
                    return reel.ToString().ToUpper();
            }
        }
    }
}
