using System;
using System.Web.Mvc;
using System.Web.Optimization;

namespace ReelCheck.Web.Areas.MasterData
{
    public class MasterDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MasterData";
            }
        }

        /// <summary>
        /// Az area regisztrációjának belépési pontja.
        /// </summary>
        /// <remarks>
        /// Ide kell tenni minden olyan dolgot, 
        /// melyet az area felélesztésekor el kell végezni.
        /// Például:
        /// - a szókódok inicializálása
        /// - vagy a komponens működéséhez szükséges adatbázis migrációjának elvégzése
        /// </remarks>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            RegisterRoutes(context);
            RegisterBundles(BundleTable.Bundles);

            //nyelvi fordítások inicializálása
            VRH.Log4Pro.MultiLanguageManager.MultiLanguageManager.InitializeWordCodes(typeof(WordCodes));

            ////itt érdemesebb elvégeztetni a migráció ellenőrzést, mint minden context példányosításakor
            //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<iSchedulerDB, Vrh.iScheduler.Migrations.Configuration>(true));
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            context.MapRoute(
                this.AreaName + "_default",
                this.AreaName + "/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null) { throw new ArgumentNullException("bundles"); }

            // Bookings Bundles
            //bundles.Add(new ScriptBundle("~/bundles/masterdata/scripts").Include(
            //    "~/Scripts/DataTables/jquery.dataTables.min.js"
            //  , "~/Scripts/DataTables/dataTables.bootstrap4.min.js"
            //));

            //bundles.Add(new StyleBundle("~/masterdata/content").Include(
            //    "~/Content/DataTables/css/jquery.dataTables.min.css"
            //  , "~/Content/DataTables/css/dataTables.bootstrap4.min.css"
            //));

            bundles.Add(new ScriptBundle("~/bundles/masterdata/scripts").Include(
                "~/Scripts/DataTables/jquery.dataTables.min.js",
                "~/Scripts/DataTables/dataTables.bootstrap4.min.js",
                "~/Scripts/moment-with-locales.min.js",
                "~/Areas/MasterData/Scripts/MasterData.js"
            ));

            bundles.Add(new StyleBundle("~/masterdata/content").Include(
                //"~/Content/DataTables/css/jquery.dataTables.min.css",
                "~/Content/DataTables/css/dataTables.bootstrap4.min.css"
            ));
        }

    }
}