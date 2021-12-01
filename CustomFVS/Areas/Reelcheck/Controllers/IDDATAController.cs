using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomFVS.Areas.Reelcheck.Models;

namespace CustomFVS.Areas.Reelcheck.Controllers
{
    public class IDDATAController : Controller
    {
        // GET: Reelcheck/IDDATAS
        public ActionResult Index(string iddata)
        {
            string info = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/README.txt"));
            return Content(info, "text");
        }

        // POST: Reelcheck/IDDATAS/ReelCheck
        public ActionResult ReelCheck(string iddata)
        {
            if (Request.RequestType == "POST")
            {
                byte[] buffer = new byte[Request.ContentLength];
                Request.InputStream.Read(buffer, 0, Request.ContentLength);
                iddata = Request.ContentEncoding.GetString(buffer);
            }
            string[] fields = iddata.Split('|');
            string typeId = String.Empty;
            string fvs = String.Empty;
            foreach (var field in fields)
            {
                if (field.ToUpper().Contains("ID"))
                {
                    typeId = field.Remove(0, 3);
                }
                if (field.ToUpper().Contains("FVS="))
                {
                    fvs = field.Remove(0, 4);
                }
            }
            try
            {
                using (var stream = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/Statistic.txt")))
                {
                    stream.WriteLine($"{DateTime.Now}: {iddata}");
                    stream.Close();
                }
            }
            catch { }
            if (typeId == "1")
            {
                // első 14 karakter
                // 6 kar datecode
                // 4 kar futósorszám
                // 5 kar mennyiség
                string fvsStart = fvs.Substring(0, 14);
                DateTime now = DateTime.Now;
                int year = now.Year;
                string y = year.ToString().Substring(year.ToString().Length - 2);
                string m = now.Month.ToString().PadLeft(2, '0');
                string d = now.Day.ToString().PadLeft(2, '0');
                string dateCode = $"{y}{m}{d}";
                Random r = new Random();
                int rInt = r.Next(1, 9999);
                var runningId = rInt.ToString().PadLeft(4, '0');
                string qty = fvs.Substring(fvs.Length - 5);
                string mtsid = Guid.NewGuid().ToString().Substring(0, 8).ToUpper() + Guid.NewGuid().ToString().Substring(0, 3).ToUpper();
                string partnumber = fvs.Substring(0, 8);
                if (fvs.StartsWith("939558421412JMA498425.33500"))
                {                    
                    return Content($"status=PASS|mtsid=mtsid|address=A1|c9023=565243303030303030303031|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=EZT A SZÖVEGET KELL MEGJELENÍTENI!!!|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16208150P3CG741710315RDD10000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303032|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16167476P3CG741710315QSO10000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303033|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("28136407J4HK1VBK5883543 02500"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303034|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("R6SEKE1712121OBU04000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303035|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("09402926MZK5DWVC739204C303000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303036|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("1618844382520A0MQG33498603000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303037|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16058165GWMK6L000179238805000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303038|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("9400063 61649A173925221  4000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303039|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("2811787272045G7D2422302003000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303130|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("28187063J4HK1VSB2802523 03000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303131|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16167390GWMK6L000178583310000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303132|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("X988238541TVE735010B11S65524"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303133|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("28124924QSJFRPAU4540715X02000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303134|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("09401625MZK5DWVC6392332103000"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=A1|c9023=565243303030303030303135|description=TEST 1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16208150"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0705A08|c9023={fvsStart}{dateCode}{runningId}{qty}|description=IC-RDD,murata|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16167476"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0705D68|c9023={fvsStart}{dateCode}{runningId}{qty}|description=CHOKE-200OHM, COM MODE, SM|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("09400083"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0904C07|c9023={fvsStart}{dateCode}{runningId}{qty}|description=RES-SM,1210 3.0N|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("28375256"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0904D09|c9023={fvsStart}{dateCode}{runningId}{qty}|description=RES-SM,1210 24.3K1|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("93955842"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0907C07|c9023={fvsStart}{dateCode}{runningId}{qty}|description=CAP-SM,0.1UF,16V,10|partnumber={partnumber}|quantity={qty}|msg=EZT A SZÖVEGET KELL MEGJELENÍTENI!!!|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16167390"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0504C03|c9023={fvsStart}{dateCode}{runningId}{qty}|description=IC-VR,BUCK BOOST,LM3668-Q1,1A,WSON|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("16058165"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0204E04|c9023={fvsStart}{dateCode}{runningId}{qty}|description=CAP-SM,0603,X7R,0.0|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                if (fvs.StartsWith("09402926"))
                {
                    return Content($"status=PASS|mtsid={mtsid}|address=S0204E04|c9023={fvsStart}{dateCode}{runningId}{qty}|description=CAP-ELS,47UF,35V,20%,-55-105C|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
                }
                return Content($"status=PASS|mtsid={mtsid}|address=S0404D07|c9023={fvsStart}{dateCode}{runningId}{qty}|description=FAKE|partnumber={partnumber}|quantity={qty}|msg=|bookid=12345|msl=MSL");
            }
            try
            {
                string ack = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/ACK{typeId}.txt"));
                return Content(ack);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}