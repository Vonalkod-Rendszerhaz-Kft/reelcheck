using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using ReelCheck.Core.DAL;
using Vrh.Web.Common.Lib;

namespace ReelCheck.Web.Areas.MasterData
{
    public class ReelController : BaseController
    {
        private const int MAX_VIEWINDEX = 2;

        // GET: MasterData/Reel
        public ActionResult Index(int initview = 0)
        {
            ViewBag.InitView = Math.Max(0, Math.Min(2, initview));
            return View();
        }

        #region Actions and methods for DataTables 

        #region GetData action (JsonResult)

        /// <summary>
        /// A DataTables plugin számára az adatok előállítása.
        /// </summary>
        /// <param name="dataTable">A DataTables által küldött adatok, paraméterek.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData(DataTablesIn dataTable)
        {
            try
            {
                using (DataBaseContext dbc = new DataBaseContext())
                {
                    IQueryable<Reel> all = dbc.Reels;

                    #region Szűrés (filter)

                    IQueryable<Reel> filtered = all;

                    #region Tábla szerinti szűrés

                    if (!string.IsNullOrEmpty(dataTable.Search.Value))
                    {
                        string srch = base.SearchFormat(dataTable.Search.Value);
                        filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.UserName) > 0 ||
                                                       SqlFunctions.PatIndex(srch, x.WorkstationId) > 0 ||
                                                       SqlFunctions.PatIndex(srch, x.ProcessId) > 0 ||
                                                       SqlFunctions.PatIndex(srch, x.IdCamera) > 0
                                                 );
                    }

                    #endregion Tábla szerinti szűrés

                    #region Oszlopok szerinti szűrés
                    foreach (DataTablesIn.DTColumn col in dataTable.Columns.Where(x => x.Searchable && !String.IsNullOrEmpty(x.Search?.Value)))
                    {
                        //string srch = SearchFormat(col.Search.Value);
                        string srch = col.Search.Value;

                        IntegerFilter intfltr;
                        DateTimeFilter dtfltr;
                        switch (col.Data)
                        {
                            case nameof(Reel.Id):
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.Id < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.Id > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.Id > intfltr.Int_1 && x.Id < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.Id).Trim()) > 0);
                                break;
                            case nameof(Reel.UserName):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.UserName) > 0);
                                break;
                            case nameof(Reel.WorkstationId):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.WorkstationId) > 0);
                                break;
                            case nameof(Reel.ProcessId):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.ProcessId) > 0);
                                break;
                            case nameof(Reel.IdCamera):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.IdCamera) > 0);
                                break;
                            case nameof(Reel.CheckCamera):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.CheckCamera) > 0);
                                break;
                            case nameof(Reel.Printer):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.Printer) > 0);
                                break;
                            case nameof(Reel.StartTimeStamp):   //DateTime
                                dtfltr = new DateTimeFilter(srch, BaseDateFormat);
                                if (dtfltr.Mode == DateTimeFilterEnum.Before) filtered = filtered.Where(x => x.StartTimeStamp < dtfltr.DateTime_1);
                                else if (dtfltr.Mode == DateTimeFilterEnum.After) filtered = filtered.Where(x => x.StartTimeStamp > dtfltr.DateTime_1);
                                else if (dtfltr.Mode == DateTimeFilterEnum.Between) filtered = filtered.Where(x => x.StartTimeStamp > dtfltr.DateTime_1 && x.StartTimeStamp < dtfltr.DateTime_2);
                                else if (dtfltr.Mode == DateTimeFilterEnum.Equal)
                                {
                                    filtered = filtered.Where(x => (dtfltr.Year ? x.StartTimeStamp.Year == dtfltr.DateTime_1.Year : true) &&
                                                                   (dtfltr.Month ? x.StartTimeStamp.Month == dtfltr.DateTime_1.Month : true) &&
                                                                   (dtfltr.Day ? x.StartTimeStamp.Day == dtfltr.DateTime_1.Day : true) &&
                                                                   (dtfltr.Hour ? x.StartTimeStamp.Hour == dtfltr.DateTime_1.Hour : true) &&
                                                                   (dtfltr.Minute ? x.StartTimeStamp.Minute == dtfltr.DateTime_1.Minute : true) &&
                                                                   (dtfltr.Second ? x.StartTimeStamp.Second == dtfltr.DateTime_1.Second : true) &&
                                                                   (dtfltr.Millisecond ? x.StartTimeStamp.Millisecond == dtfltr.DateTime_1.Millisecond : true));
                                }
                                break;
                            case nameof(Reel.EndTimeStamp):   //DateTime?
                                dtfltr = new DateTimeFilter(srch, BaseDateFormat);
                                if (dtfltr.Mode == DateTimeFilterEnum.Before) filtered = filtered.Where(x => x.EndTimeStamp < dtfltr.DateTime_1);
                                else if (dtfltr.Mode == DateTimeFilterEnum.After) filtered = filtered.Where(x => x.EndTimeStamp > dtfltr.DateTime_1);
                                else if (dtfltr.Mode == DateTimeFilterEnum.Between) filtered = filtered.Where(x => x.EndTimeStamp > dtfltr.DateTime_1 && x.EndTimeStamp < dtfltr.DateTime_2);
                                else if (dtfltr.Mode == DateTimeFilterEnum.Equal)
                                {
                                    filtered = filtered.Where(x => (dtfltr.Year ? x.EndTimeStamp.Value.Year == dtfltr.DateTime_1.Year : true) &&
                                                                   (dtfltr.Month ? x.EndTimeStamp.Value.Month == dtfltr.DateTime_1.Month : true) &&
                                                                   (dtfltr.Day ? x.EndTimeStamp.Value.Day == dtfltr.DateTime_1.Day : true) &&
                                                                   (dtfltr.Hour ? x.EndTimeStamp.Value.Hour == dtfltr.DateTime_1.Hour : true) &&
                                                                   (dtfltr.Minute ? x.EndTimeStamp.Value.Minute == dtfltr.DateTime_1.Minute : true) &&
                                                                   (dtfltr.Second ? x.EndTimeStamp.Value.Second == dtfltr.DateTime_1.Second : true) &&
                                                                   (dtfltr.Millisecond ? x.EndTimeStamp.Value.Millisecond == dtfltr.DateTime_1.Millisecond : true));
                                }
                                break;
                            case nameof(Reel.Result):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.Result) > 0);
                                break;
                            case nameof(Reel.IdentificationResult):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.IdentificationResult) > 0);
                                break;
                            case nameof(Reel.BackgroundSystemResult):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.BackgroundSystemResult) > 0);
                                break;
                            case nameof(Reel.CheckResult):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.CheckResult) > 0);
                                break;
                            case nameof(Reel.IdentificationReadAttempts):   //int
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.IdentificationReadAttempts < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.IdentificationReadAttempts > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.IdentificationReadAttempts > intfltr.Int_1 && x.IdentificationReadAttempts < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.IdentificationReadAttempts).Trim()) > 0);
                                break;
                            case nameof(Reel.IdentificationHandheldReadAttempts): //int
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.IdentificationHandheldReadAttempts < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.IdentificationHandheldReadAttempts > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.IdentificationHandheldReadAttempts > intfltr.Int_1 && x.IdentificationHandheldReadAttempts < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.IdentificationHandheldReadAttempts).Trim()) > 0);
                                break;
                            case nameof(Reel.CheckReadAttempts):    //int
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.CheckReadAttempts < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.CheckReadAttempts > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.CheckReadAttempts > intfltr.Int_1 && x.CheckReadAttempts < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.CheckReadAttempts).Trim()) > 0);
                                break;
                            case nameof(Reel.CheckHandheldReadAttempts): //int
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.CheckHandheldReadAttempts < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.CheckHandheldReadAttempts > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.CheckHandheldReadAttempts > intfltr.Int_1 && x.CheckHandheldReadAttempts < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.CheckHandheldReadAttempts).Trim()) > 0);
                                break;
                            case nameof(Reel.PrintAttempts):    //int
                                intfltr = new IntegerFilter(srch);
                                if (intfltr.Mode == IntegerFilterEnum.Before) filtered = filtered.Where(x => x.PrintAttempts < intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.After) filtered = filtered.Where(x => x.PrintAttempts > intfltr.Int_1);
                                else if (intfltr.Mode == IntegerFilterEnum.Between) filtered = filtered.Where(x => x.PrintAttempts > intfltr.Int_1 && x.Id < intfltr.Int_2);
                                else filtered = filtered.Where(x => SqlFunctions.PatIndex(intfltr.Search, SqlFunctions.StringConvert((decimal)x.PrintAttempts).Trim()) > 0);
                                break;
                            case nameof(Reel.IdentificationReadsData):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.IdentificationReadsData) > 0);
                                break;
                            case nameof(Reel.CheckReadsData):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.CheckReadsData) > 0);
                                break;
                            case nameof(Reel.SendToBackgroundSystem):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SendToBackgroundSystem) > 0);
                                break;
                            case nameof(Reel.BackgroundSystemResponse):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.BackgroundSystemResponse) > 0);
                                break;
                            case nameof(Reel.LabelId):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.LabelId) > 0);
                                break;
                            case nameof(Reel.Vendor):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.Vendor) > 0);
                                break;
                            case nameof(Reel.SupplierPartNumber):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SupplierPartNumber) > 0);
                                break;
                            case nameof(Reel.SupplierLot):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SupplierLot) > 0);
                                break;
                            case nameof(Reel.SupplierFVS):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SupplierFVS) > 0);
                                break;
                            case nameof(Reel.SupplierReelSerialNumber):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SupplierReelSerialNumber) > 0);
                                break;
                            case nameof(Reel.SupplierQty):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.SupplierQty) > 0);
                                break;
                            case nameof(Reel.InternalMTS):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalMTS) > 0);
                                break;
                            case nameof(Reel.InternalFVS):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalFVS) > 0);
                                break;
                            case nameof(Reel.InternalQty):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalQty) > 0);
                                break;
                            case nameof(Reel.InternalLot):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalLot) > 0);
                                break;
                            case nameof(Reel.InternalPartNumber):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalPartNumber) > 0);
                                break;
                            case nameof(Reel.InternalVendor):
                                filtered = filtered.Where(x => SqlFunctions.PatIndex(srch, x.InternalVendor) > 0);
                                break;
                            default:
                                break;
                        }

                    }
                    #endregion Oszlopok szerinti szűrés

                    #endregion Szűrés (filter)

                    #region Rendezés
                    IOrderedQueryable<Reel> filteredAndOrdered;
                    if (dataTable.Order != null && dataTable.Order.Any())
                    {
                        DataTablesIn.DTOrder ord = dataTable.Order[0];
                        Expression<Func<Reel, object>> sortExpression = GetSortExpression(dataTable.Columns[ord.Column].Data);
                        Expression<Func<Reel, int>> intExpression = (x => x.Id);
                        //sortExpression = (x => x.Id);
                        filteredAndOrdered = ord.Dir == DataTablesConstants.ORDER_DESC 
                                           ? filtered.OrderByDescending(sortExpression) 
                                           : filtered.OrderBy(sortExpression);

                        for (int i = 1; i < dataTable.Order.Count(); i++)
                        {
                            ord = dataTable.Order[i];
                            sortExpression = GetSortExpression(dataTable.Columns[ord.Column].Data);
                            filteredAndOrdered = (ord.Dir == DataTablesConstants.ORDER_DESC 
                                               ? filteredAndOrdered.ThenByDescending(sortExpression)
                                               : filteredAndOrdered.ThenBy(sortExpression));
                        }
                    }
                    else
                    {   // a Take() miatt mindenképp kell rendezni
                        filteredAndOrdered = filtered.OrderBy(o => o.Id);
                    }
                    #endregion Rendezés

                    #region Megjelenítendő adatok
                    //List<object[,]> result = new List<object[,]>();
                    //
                    //foreach (Reel item in filteredAndOrdered.Skip(dataTable.Start).Take(take))
                    //{
                    //    result.Add(BuildOneRow(item));
                    //}
                    #endregion Megjelenítendő adatok

                    int filteredCount = filteredAndOrdered.Count();
                    int take = dataTable.Length > 0 ? dataTable.Length : filteredCount;
                    return Json(new
                    {
                        draw = dataTable.Draw,
                        recordsTotal = all.Count(),
                        recordsFiltered = filteredCount,
                        data = filteredAndOrdered.Skip(dataTable.Start).Take(take).ToList(),
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = String.Join("\n",WebCommon.ErrorListBuilder(ex)) }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion GetData action (JsonResult)

        #region GetSortExpression private method
        /// <summary>
        /// Az oszlopnévnek megfelelő rendező kifejezéssel tér vissza.
        /// </summary>
        /// <param name="colName">Az oszlop neve</param>
        /// <returns></returns>
        private Expression<Func<Reel, object>> GetSortExpression(string colName)
        {
            switch (colName)
            {
                case nameof(Reel.Id):
                    return (x => x.Id.ToString());
                case nameof(Reel.UserName):
                    return (x => x.UserName);
                case nameof(Reel.WorkstationId):
                    return (x => x.WorkstationId);
                case nameof(Reel.ProcessId):
                    return (x => x.ProcessId);
                case nameof(Reel.IdCamera):
                    return (x => x.IdCamera);
                case nameof(Reel.CheckCamera):
                    return (x => x.CheckCamera);
                case nameof(Reel.Printer):
                    return (x => x.Printer);
                case nameof(Reel.StartTimeStamp):   //DateTime
                    return (x => x.StartTimeStamp);
                case nameof(Reel.EndTimeStamp):   //DateTime?
                    return (x => x.EndTimeStamp);
                case nameof(Reel.Result):
                    return (x => x.Result);
                case nameof(Reel.IdentificationResult):
                    return (x => x.IdentificationResult);
                case nameof(Reel.BackgroundSystemResult):
                    return (x => x.BackgroundSystemResult);
                case nameof(Reel.CheckResult):
                    return (x => x.CheckResult);
                case nameof(Reel.IdentificationReadAttempts):   //int
                    return (x => x.IdentificationReadAttempts.ToString());
                case nameof(Reel.IdentificationHandheldReadAttempts): //int
                    return (x => x.IdentificationHandheldReadAttempts.ToString());
                case nameof(Reel.CheckReadAttempts):    //int
                    return (x => x.CheckReadAttempts.ToString());
                case nameof(Reel.CheckHandheldReadAttempts): //int
                    return (x => x.CheckHandheldReadAttempts.ToString());
                case nameof(Reel.PrintAttempts):    //int
                    return (x => x.PrintAttempts.ToString());
                case nameof(Reel.IdentificationReadsData):
                    return (x => x.IdentificationReadsData);
                case nameof(Reel.CheckReadsData):
                    return (x => x.CheckReadsData);
                case nameof(Reel.SendToBackgroundSystem):
                    return (x => x.SendToBackgroundSystem);
                case nameof(Reel.BackgroundSystemResponse):
                    return (x => x.BackgroundSystemResponse);
                case nameof(Reel.LabelId):
                    return (x => x.LabelId);
                case nameof(Reel.Vendor):
                    return (x => x.Vendor);
                case nameof(Reel.SupplierPartNumber):
                    return (x => x.SupplierPartNumber);
                case nameof(Reel.SupplierLot):
                    return (x => x.SupplierLot);
                case nameof(Reel.SupplierFVS):
                    return (x => x.SupplierFVS);
                case nameof(Reel.SupplierReelSerialNumber):
                    return (x => x.SupplierReelSerialNumber);
                case nameof(Reel.SupplierQty):
                    return (x => x.SupplierQty);
                case nameof(Reel.InternalMTS):
                    return (x => x.InternalMTS);
                case nameof(Reel.InternalFVS):
                    return (x => x.InternalFVS);
                case nameof(Reel.InternalQty):
                    return (x => x.InternalQty);
                case nameof(Reel.InternalLot):
                    return (x => x.InternalLot);
                case nameof(Reel.InternalPartNumber):
                    return (x => x.InternalPartNumber);
                case nameof(Reel.InternalVendor):
                    return (x => x.InternalVendor);
                default:
                    throw new ApplicationException($"There are unknown column name in the ordering! Name={colName}");
            }
        }
        #endregion GetSortExpression private method

        #region Build.... private methods (full comment)
        //WA20190514: Ez valójában nem is kell!
        //private object[,] BuildOneRow(Reel item)
        //{
        //    return (new object[,]
        //    {
        //        { nameof(Reel.Id), item.Id.ToString() },
        //        { nameof(Reel.UserName), item.UserName },
        //        { nameof(Reel.WorkstationId), item.WorkstationId },
        //        { nameof(Reel.ProcessId), item.ProcessId },
        //        { nameof(Reel.IdCamera), item.IdCamera },
        //        { nameof(Reel.CheckCamera), item.CheckCamera }
        //    }
        //    );
        //}
        //private string BuildRefreshRowScript(Reel row)
        //{
        //    string sarrayrow = "";
        //    string svesszo = "";
        //    foreach (var item in BuildOneRow(row))
        //    {
        //        sarrayrow += svesszo + " '" + item + "'";
        //        svesszo = ",";
        //    }
        //    sarrayrow = "[" + sarrayrow + "]";
        //    string sselector = "tr#" + row.Id.ToString();
        //    return ("dataTableRefreshRow( '" + base.DataTableSelector + "', " + sarrayrow + ", '" + sselector + "' )");

        //}

        #endregion Build.... private methods

        #endregion Actions and methods for DataTables 
    }
}