using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace CustomFVS.Areas.Reelcheck.Controllers
{
    public class CORSTestController : Controller
    {
        public ActionResult Success(string userName, string password)
        {
            string temp = String.Empty;
            if (Request.RequestType == "POST")
            {
                // ID=99|USER=ddd|PSW=ddd
                byte[] buffer = new byte[Request.ContentLength];
                Request.InputStream.Read(buffer, 0, Request.ContentLength);
                temp = $"SUCCESS POST: {Request.ContentEncoding.GetString(buffer)}";
            }
            else
            {
                temp = $"SUCCESS GET: username: {userName}, password: {password}";
            }
            var origin = Request.Headers["Origin"];
            if (!String.IsNullOrEmpty(origin))
            {
                Response.Headers.Add("Access-Control-Allow-Origin", origin);
            }
            try
            {
                using (var stream = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/Statistic.txt")))
                {
                    stream.WriteLine($"{DateTime.Now}: {temp}");
                    stream.Close();
                }
                string responseBody = "status=PASS|szint=1";
                return Content(responseBody);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Error(string userName, string password)
        {
            string temp = String.Empty;
            if (Request.RequestType == "POST")
            {
                // ID=99|USER=ddd|PSW=ddd
                byte[] buffer = new byte[Request.ContentLength];
                Request.InputStream.Read(buffer, 0, Request.ContentLength);
                temp = $"ERROR POST: {Request.ContentEncoding.GetString(buffer)}";
            }
            else
            {
                temp = $"ERROR GET: username: {userName}, password: {password}";
            }
            try
            {
                using (var stream = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/Statistic.txt")))
                {
                    stream.WriteLine($"{DateTime.Now}: {temp}");
                    stream.Close();
                }
                string responseBody = "status=PASS|szint=1";
                return Content(responseBody);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}