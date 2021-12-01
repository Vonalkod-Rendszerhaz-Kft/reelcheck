using System;
using System.Collections.Generic;
using Vrh.Redis.DataPoolHandler;

namespace ReelCheck.Core
{
    /// <summary>
    /// Tekercs adatok
    /// </summary>
    internal class ReelData : IDisposable
    {
        /// <summary>
        /// Construktor
        /// </summary>
        /// <param name="instanceWriter">Redis dpwriter referencia</param>
        /// <param name="reelPosition">tekercs poziciója</param>
        /// <param name="userName">ellenőrzést végző felhasználó</param>
        /// <param name="workstationType">munkaállomás</param>
        /// <param name="processId">folyamat</param>
        public ReelData(InstanceWriter instanceWriter, int reelPosition,
            string userName, WorkStationType workstationType, string workstationId, ProcessType processId)
        {
            _instanceWriter = instanceWriter;
            _reelPosition = reelPosition;
            _userName = userName;
            _workstationType = workstationType;
            _processId = processId;
            _workstationId = workstationId;
            // Reset redis DP
            ResetData();
        }

        /// <summary>
        /// Tekercs poziciója
        /// </summary>
        public int Position
        {
            get
            {
                lock (_locker)
                {
                    return _reelPosition;
                }
            }
            set
            {
                lock(_locker)
                {
                    //ResetPosition();
                    _reelPosition = value;
                    RefreshPosition();
                }
            }
        }

        /// <summary>
        /// Jelzi, hogy a tekercs pozició goldensample tesztben került feltöltésre
        /// </summary>
        public bool GoldenSample
        {
            get { lock (_locker) { return _goldenSample; } }
            set
            {
                lock (_locker)
                {
                    _goldenSample = value;
                    WriteToCache(DataPoolDefinition.Reel.GSMode, value);
                }
            }
        }
        private bool _goldenSample = false;

        /// <summary>
        /// Jelzi, hogy ez a tekercs még nem kint lévő Goldensample figyelmeztetés miatti blokkolás miatt forgott be
        /// </summary>
        public bool NotGoldenSampleDueYet
        {
            get { lock (_locker) { return _notGoldenSampleDueYet; } }
            set { lock (_locker) { _notGoldenSampleDueYet = value; } }
        }
        private bool _notGoldenSampleDueYet = false;

        /// <summary>
        /// ID kamera 
        /// </summary>
        public string IdCamera
        {
            get { lock (_locker) { return _idCamera; } }
            set { lock (_locker) { _idCamera = value; } }
        }
        private string _idCamera;

        /// <summary>
        /// CHECK kamera
        /// </summary>
        public string CheckCamera
        {
            get { lock (_locker) { return _checkCamera; } }
            set { lock (_locker) { _checkCamera = value; } }
        }
        private string _checkCamera;

        /// <summary>
        /// Nyomtató
        /// </summary>
        public string Printer
        {
            get { lock (_locker) { return _printer; } }
            set { lock (_locker) { _printer = value; } }
        }
        private string _printer;

        /// <summary>
        /// A nyomtatás művelet eredménye
        /// </summary>
        public PrintResult PrintResult
        {
            get { lock (_locker) { return _printResult; } }
            set
            {
                lock (_locker)
                {
                    _printResult = value;
                    WriteToCache(DataPoolDefinition.Reel.PrintResult, value);
                }
            }
        }
        private PrintResult _printResult;

        /// <summary>
        /// Ragasztás eredménye
        /// </summary>
        public PrintResult StickingResult
        {
            get { lock (_locker) { return _stickingResult; } }
            set
            {
                lock (_locker)
                {
                    _stickingResult = value;
                    WriteToCache(DataPoolDefinition.Reel.StickingResult, value);
                }
            }
        }
        private PrintResult _stickingResult;

        /// <summary>
        /// Ellenőrzési folymat indításának időbélyege
        /// </summary>
        public DateTime StartTimeStamp
        {
            get { lock (_locker) { return _startTimeStamp; } }
            set
            {
                lock (_locker)
                {
                    _startTimeStamp = value;
                    WriteToCache(DataPoolDefinition.Reel.CycleStartTimeStamp, _startTimeStamp);
                }
            }
        }
        private DateTime _startTimeStamp = DateTime.MinValue;

        /// <summary>
        /// Ellenőrzési folymat befelyezésének időbélyege
        /// </summary>
        public DateTime? EndTimeStamp
        {
            get { lock (_locker) { return _endTimeStamp; } }
            set { lock (_locker) { _endTimeStamp = value; } }
        }
        private DateTime? _endTimeStamp;

        /// <summary>
        /// Az ellenőrzés eredménye
        /// </summary>
        public Result Result
        {
            get { lock (_locker) { return _result; } }
            set
            {
                lock (_locker)
                {
                    _result = value;
                    WriteToCache(DataPoolDefinition.Reel.ReelResult, value);
                }
            }
        }
        private Result _result;

        /// <summary>
        /// Azonosítási folyamat eredménye összesített eredménye
        /// </summary>
        public IdentificationResult IdentificationResult
        {
            get { lock (_locker) { return _identificationResult; } }
            set {
                lock (_locker)
                {
                    _identificationResult = value;
                }
            }
        }
        private IdentificationResult _identificationResult;

        /// <summary>
        /// Azonosítás: kamera olvasás eredménye
        /// </summary>
        public IdentificationResult IdentificationReadResult
        {
            get { lock (_locker) { return _identificationReadResult; } }
            set
            {
                lock (_locker)
                {
                    _identificationReadResult = value;
                    WriteToCache(DataPoolDefinition.Reel.IdentificationReadResult, value);
                }
            }
        }
        private IdentificationResult _identificationReadResult;

        /// <summary>
        /// Azonosítás: regex kiértékelés eredménye
        /// </summary>
        public IdentificationResult IdentificationRegexCheckResult
        {
            get { lock (_locker) { return _identificationRegexCheckResult; } }
            set
            {
                lock (_locker)
                {
                    _identificationRegexCheckResult = value;
                    WriteToCache(DataPoolDefinition.Reel.IdentificationRegexCheckResult, value);
                }
            }
        }
        private IdentificationResult _identificationRegexCheckResult;

        /// <summary>
        /// Külső rendszer ellenőrzésének eredménye
        /// </summary>
        public IdentificationResult IdentificationExternalCheckResult
        {
            get { lock (_locker) { return _identificationExternalCheckResult; } }
            set
            {
                lock (_locker)
                {
                    _identificationExternalCheckResult = value;
                    WriteToCache(DataPoolDefinition.Reel.IdentificationExternalCheckResult, value);
                }
            }
        }
        private IdentificationResult _identificationExternalCheckResult;

        /// <summary>
        /// Háttérrendszer válasza
        /// </summary>
        public BackgroundSystemResult BackgroundSystemResult
        {
            get { lock (_locker) { return _backgroundSystemResult; } }
            set { lock (_locker) { _backgroundSystemResult = value; } }
        }
        private BackgroundSystemResult _backgroundSystemResult;

        /// <summary>
        /// Azonosítás összköltsége
        /// </summary>
        public TimeSpan IdentificationFullCost
        {
            get { lock (_locker) { return _identificationFullCost; } }
            set
            {
                lock (_locker)
                {
                    _identificationFullCost = value;
                    //WriteToCache(DataPoolDefinition.Reel.CheckResult, value);
                }
            }
        }
        private TimeSpan _identificationFullCost = new TimeSpan(0);

        /// <summary>
        /// Azonosítás címke olvasási költsége
        /// </summary>
        public TimeSpan IdentificationReadCost
        {
            get { lock (_locker) { return _identificationReadCost; } }
            set
            {
                lock (_locker)
                {
                    _identificationReadCost = value;
                    //WriteToCache(DataPoolDefinition.Reel.CheckResult, value);
                }
            }
        }
        private TimeSpan _identificationReadCost = new TimeSpan(0);

        /// <summary>
        /// Azonosítás adat ellenőrzési költsége (RegexCheck)
        /// </summary>
        public TimeSpan IdentificationRegexCheckCost
        {
            get { lock (_locker) { return _identificationRegexCheckCost; } }
            set
            {
                lock (_locker)
                {
                    _identificationRegexCheckCost = value;
                    //WriteToCache(DataPoolDefinition.Reel.CheckResult, value);
                }
            }
        }
        private TimeSpan _identificationRegexCheckCost = new TimeSpan(0);

        /// <summary>
        /// Azonosítás külső rendszerben való ellenőrzési költsége (CustomFVS)
        /// </summary>
        public TimeSpan IdentificationExternalSystemCheckCost
        {
            get { lock (_locker) { return _identificationExternalSystemCheckCost; } }
            set
            {
                lock (_locker)
                {
                    _identificationExternalSystemCheckCost = value;
                    //WriteToCache(DataPoolDefinition.Reel.CheckResult, value);
                }
            }
        }
        private TimeSpan _identificationExternalSystemCheckCost = new TimeSpan(0);

        /// <summary>
        /// Azonosítás külső rendszerben való ellenőrzési költsége (CustomFVS)
        /// </summary>
        public TimeSpan IdentificationWaitForTriggerToLowCost
        {
            get { lock (_locker) { return _identificationWaitForTriggerToLowCost; } }
            set
            {
                lock (_locker)
                {
                    _identificationWaitForTriggerToLowCost = value;
                }
            }
        }
        private TimeSpan _identificationWaitForTriggerToLowCost = new TimeSpan(0);

        /// <summary>
        /// Nyomtatás költsége
        /// </summary>
        public TimeSpan PrintCost
        {
            get { lock (_locker) { return _printCost; } }
            set
            {
                lock (_locker)
                {
                    _printCost = value;
                }
            }
        }
        private TimeSpan _printCost = new TimeSpan(0);

        /// <summary>
        /// Ragasztás költsége
        /// </summary>
        public TimeSpan StickingCost
        {
            get { lock (_locker) { return _stickingCost; } }
            set
            {
                lock (_locker)
                {
                    _stickingCost = value;
                }
            }
        }
        private TimeSpan _stickingCost = new TimeSpan(0);

        /// <summary>
        /// Nyomtatás beavatkozásra várakozás költsége
        /// </summary>
        public TimeSpan WaitPrintInterventionCost
        {
            get { lock (_locker) { return _waitPrintInterventionCost; } }
            set
            {
                lock (_locker)
                {
                    _waitPrintInterventionCost = value;
                }
            }
        }
        private TimeSpan _waitPrintInterventionCost = new TimeSpan(0);

        /// <summary>
        /// Nyomtatás költsége
        /// </summary>
        public TimeSpan FullPrintCost
        {
            get { lock (_locker) { return _fullPrintCost; } }
            set
            {
                lock (_locker)
                {
                    _fullPrintCost = value;
                }
            }
        }
        private TimeSpan _fullPrintCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: wárakozás az engedélyező  triggerre (csak LABELE)
        /// </summary>
        public TimeSpan CheckWaitForStartTriggerCost
        {
            get { lock (_locker) { return _checkWaitForStartTriggerCost; } }
            set
            {
                lock (_locker)
                {
                    _checkWaitForStartTriggerCost = value;
                }
            }
        }
        private TimeSpan _checkWaitForStartTriggerCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: címke olvasás költsége
        /// </summary>
        public TimeSpan CheckReadCost
        {
            get { lock (_locker) { return _checkReadCost; } }
            set
            {
                lock (_locker)
                {
                    _checkReadCost = value;
                }
            }
        }
        private TimeSpan _checkReadCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: Adatkiértékelés költsége
        /// </summary>
        public TimeSpan CheckRegexAndDataCheckCost
        {
            get { lock (_locker) { return _checkRegexAndDataCheckCost; } }
            set
            {
                lock (_locker)
                {
                    _checkRegexAndDataCheckCost = value;
                }
            }
        }
        private TimeSpan _checkRegexAndDataCheckCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: tekercs adatbátismentés költsége
        /// </summary>
        public TimeSpan SaveDbCost
        {
            get { lock (_locker) { return _saveDbCost; } }
            set
            {
                lock (_locker)
                {
                    _saveDbCost = value;
                }
            }
        }
        private TimeSpan _saveDbCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: tekercs bevételezés eredmény CustomFVS-be könyvelése
        /// </summary>
        public TimeSpan BookingCost
        {
            get { lock (_locker) { return _bookingCost; } }
            set
            {
                lock (_locker)
                {
                    _bookingCost = value;
                }
            }
        }
        private TimeSpan _bookingCost = new TimeSpan(0);

        /// <summary>
        /// Ellenőrzés: tekercs bevételezés eredmény CustomFVS-be könyvelése
        /// </summary>
        public TimeSpan CheckFullCost
        {
            get { lock (_locker) { return _checkFullCost; } }
            set
            {
                lock (_locker)
                {
                    _checkFullCost = value;
                }
            }
        }
        private TimeSpan _checkFullCost = new TimeSpan(0);

        /// <summary>
        /// Kinyomtatott címke ellenőrzésének eredménye
        /// </summary>
        public CheckResult CheckResult
        {
            get { lock (_locker) { return _checkResult; } }
            set
            {
                lock (_locker)
                {
                    _checkResult = value;
                    //WriteToCache(DataPoolDefinition.Reel.CheckResult, value);
                }
            }
        }
        private CheckResult _checkResult;

        /// <summary>
        /// Check kamera olvasási eredménye
        /// </summary>
        public CheckResult CheckReadResult
        {
            get { lock (_locker) { return _checkReadResult; } }
            set
            {
                lock (_locker)
                {
                    _checkReadResult = value;
                    WriteToCache(DataPoolDefinition.Reel.CheckReadResult, value);
                }
            }
        }
        private CheckResult _checkReadResult;

        /// <summary>
        /// Check kamera Reegex ellenőrzés eredménye
        /// </summary>
        public CheckResult CheckRegexCheckResult
        {
            get { lock (_locker) { return _checkRegexCheckResult; } }
            set
            {
                lock (_locker)
                {
                    _checkRegexCheckResult = value;
                    WriteToCache(DataPoolDefinition.Reel.CheckRegexCheckResult, value);
                }
            }
        }
        private CheckResult _checkRegexCheckResult;

        /// <summary>
        /// Sikeres/sikertelen ellenőrzést követő könyvelés (PASS/FAIL) eredménye
        /// </summary>
        public ExternalBookingResult CheckExternalBookingResult
        {
            get { lock (_locker) { return _checkExternalBookingResult; } }
            set
            {
                lock (_locker)
                {
                    _checkExternalBookingResult = value;
                    WriteToCache(DataPoolDefinition.Reel.CheckExternalBookingResult, value);
                }
            }
        }
        private ExternalBookingResult _checkExternalBookingResult;

        public string ErrorMessage
        {
            get { lock (_locker) { return _errorMessage; } }
            set
            {
                lock (_locker)
                {
                    _errorMessage = value;
                    WriteToCache(DataPoolDefinition.Reel.ErrorMessage, value);
                }
            }
        }
        private string _errorMessage;

        /// <summary>
        /// Azonosítási olvasások száma
        /// </summary>
        public int IdentificationReadAttempts
        {
            get { lock (_locker) { return _identificationReadAttempts; } }
            set { lock (_locker) { _identificationReadAttempts = value; } }
        }
        private int _identificationReadAttempts;

        /// <summary>
        /// Azonosításban a kézi olvasásának száma
        /// </summary>
        public int IdentificationHandheldReadAttempts
        {
            get { lock (_locker) { return _identificationHandheldReadAttempts; } }
            set { lock (_locker) { _identificationHandheldReadAttempts = value; } }
        }
        private int _identificationHandheldReadAttempts;

        /// <summary>
        /// Címke visszaellnőrzési olvasások száma
        /// </summary>
        public int CheckReadAttempts
        {
            get { lock (_locker) { return _checkReadAttempts; } }
            set { lock (_locker) { _checkReadAttempts = value; } }
        }
        private int _checkReadAttempts;

        /// <summary>
        /// Kinyomtatott cimke ellenőrzése során a kézi olvasások száma
        /// </summary>
        public int CheckHandheldReadAttempts
        {
            get { lock (_locker) { return _checkHandheldReadAttempts; } }
            set { lock (_locker) { _checkHandheldReadAttempts = value; } }
        }
        private int _checkHandheldReadAttempts;

        /// <summary>
        /// Nyomtatásai kisérletek száma
        /// </summary>
        public int PrintAttempts
        {
            get { lock (_locker) { return _printAttempts; } }
            set { lock (_locker) { _printAttempts = value; } }
        }
        private int _printAttempts;

        /// <summary>
        /// ID kameraáltal olvasott adatok
        /// </summary>
        public string IdentificationReadsData
        {
            get { lock (_locker) { return _identificationReadsData; } }
            set
            {
                lock (_locker)
                {
                    if (value != null)
                    {
                        _identificationReadsData = value;
                    }
                    else
                    {
                        _identificationReadsData = String.Empty;
                    }
                    WriteToCache<string>(DataPoolDefinition.Reel.BCListId, value);
                }
            }
        }
        private string _identificationReadsData;

        /// <summary>
        /// Címke vonalkód adatok
        /// </summary>
        public string BCList
        {
            get { lock (_locker) { return _bclist; } }
            set
            {
                lock (_locker)
                {
                    if (value != null)
                    {
                        _bclist = value;
                    }
                    else
                    {
                        _bclist = String.Empty;
                    }
                    WriteToCache<string>(DataPoolDefinition.Reel.BCList, value);
                }
            }
        }
        private string _bclist;

        /// <summary>
        /// CustomFVS-től kapott státusz
        /// </summary>
        public string Status
        {
            get { lock (_locker) { return _status; } }
            set
            {
                lock (_locker)
                {
                    if (value != null)
                    {
                        _status = value;
                    }
                    else
                    {
                        _status = String.Empty;
                    }
                }
            }
        }
        private string _status;

        /// <summary>
        /// Check kamera általolvasott adatok
        /// </summary>
        public string CheckReadsData
        {
            get { lock (_locker) { return _checkReadsData; } }
            set
            {
                lock (_locker)
                {
                    if (value != null)
                    {
                        _checkReadsData = value;
                    }
                    else
                    {
                        _checkReadsData = String.Empty;
                    }
                    WriteToCache<string>(DataPoolDefinition.Reel.BCListCheck, value);
                }
            }
        }
        private string _checkReadsData;

        /// <summary>
        /// Check kamera általolvasott MTS id
        /// </summary>
        public string CheckMTSId
        {
            get { lock (_locker) { return _checkMTSId; } }
            set
            {
                lock (_locker)
                {
                    _checkMTSId = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.CMTSID, value);
                }
            }
        }
        private string _checkMTSId;

        /// <summary>
        /// Check kamera által olvasott FVS
        /// </summary>
        public string CheckFVS
        {
            get { lock (_locker) { return _checkFVS; } }
            set
            {
                lock (_locker)
                {
                    _checkFVS = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.CFVS, value);
                }
            }
        }
        private string _checkFVS;

        /// <summary>
        /// A háttérrendszernek küldött adat
        /// </summary>
        public string SendToBackgroundSystem
        {
            get { lock (_locker) { return _sendToBackgroundSystem; } }
            set
            {
                lock (_locker)
                {
                    _sendToBackgroundSystem = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IdData, value);
                }
            }
        }
        private string _sendToBackgroundSystem;

        /// <summary>
        /// A háttérrendszernek küldött adat
        /// </summary>
        public string SendToBackgroundSystemForBook
        {
            get { lock (_locker) { return _sendToBackgroundSystemForBook; } }
            set
            {
                lock (_locker)
                {
                    _sendToBackgroundSystemForBook = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IdData, value);
                }
            }
        }
        private string _sendToBackgroundSystemForBook;

        /// <summary>
        /// A háttérrendszer válasza
        /// </summary>
        public string BackgroundSystemResponse
        {
            get { lock (_locker) { return _backgroundSystemResponse; } }
            set
            {
                lock (_locker)
                {
                    _backgroundSystemResponse = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IdDataAck, value);
                }
            }
        }
        private string _backgroundSystemResponse;

        /// <summary>
        /// Háttérrendszer felhasználónak szánt üzenete
        /// </summary>
        public string BackgroundSystemMessage
        {
            get { lock (_locker) { return _backgroundSystemMessage; } }
            set
            {
                lock (_locker)
                {
                    _backgroundSystemMessage = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.BackGroundSystemMessage, value);
                }
            }
        }
        private string _backgroundSystemMessage;

        /// <summary>
        /// Az azonosított címke azonosítója
        /// </summary>
        public string LabelId
        {
            get { lock (_locker) { return _labelId; } }
            set
            {
                lock (_locker)
                {
                    _labelId = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.LabelId, value);
                }
            }
        }
        private string _labelId;

        /// <summary>
        /// Beszállító kód
        /// </summary>
        public string Vendor
        {
            get { lock (_locker) { return _vendor; } }
            set
            {
                lock (_locker)
                {
                    _vendor = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SVendor, value);
                }
            }
        }
        private string _vendor;

        /// <summary>
        /// Beszállítói cikkszám
        /// </summary>
        public string SupplierPartNumber
        {
            get { lock (_locker) { return _supplierPartNumber; } }
            set
            {
                lock (_locker)
                {
                    _supplierPartNumber = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SPN, value);
                }
            }
        }
        private string _supplierPartNumber;

        /// <summary>
        /// Beszállítói Lot
        /// </summary>
        public string SupplierLot
        {
            get { lock (_locker) { return _suplierLot; } }
            set
            {
                lock (_locker)
                {
                    _suplierLot = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SLot, value);
                }
            }
        }
        private string _suplierLot;

        /// <summary>
        /// Beszállítói FVS
        /// </summary>
        public string SupplierFVS
        {
            get { lock (_locker) { return _suplierFVS; } }
            set
            {
                lock (_locker)
                {
                    _suplierFVS = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SFVS, value);
                }
            }
        }
        private string _suplierFVS;

        /// <summary>
        /// Beszállítói SN
        /// </summary>
        public string SupplierReelSerialNumber
        {
            get { lock (_locker) { return _suplierReelSerialNumber; } }
            set
            {
                lock (_locker)
                {
                    _suplierReelSerialNumber = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SRSRN, value);
                }
            }
        }
        private string _suplierReelSerialNumber;

        /// <summary>
        /// BEszállítói mennyiség
        /// </summary>
        public string SupplierQty
        {
            get { lock (_locker) { return _suplierQty; } }
            set
            {
                lock (_locker)
                {
                    _suplierQty = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.SQty, value);
                }
            }
        }
        private string _suplierQty;

        /// <summary>
        /// Belső MTS
        /// </summary>
        public string InternalMTS
        {
            get { lock (_locker) { return _internalMTS; } }
            set
            {
                lock (_locker)
                {
                    _internalMTS = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IMTSId, value);
                }
            }
        }
        private string _internalMTS;

        /// <summary>
        /// Belső FVS
        /// </summary>
        public string InternalFVS
        {
            get { lock (_locker) { return _internalFVS; } }
            set
            {
                lock (_locker)
                {
                    _internalFVS = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IFVS, value);
                }
            }
        }
        private string _internalFVS;

        /// <summary>
        /// Belső mennyiség
        /// </summary>
        public string InternalQty
        {
            get { lock (_locker) { return _internalQty; } }
            set
            {
                lock (_locker)
                {
                    _internalQty = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IQty, value);
                }
            }
        }
        private string _internalQty;

        public string InternalMslCode
        {
            get { lock (_locker) { return _internalMSLCode; } }
            set
            {
                lock (_locker)
                {
                    _internalMSLCode = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IMSL, value);
                }
            }
        }
        private string _internalMSLCode;

        /// <summary>
        /// Belső LOT
        /// </summary>
        public string InternalLot
        {
            get { lock (_locker) { return _internalLot; } }
            set
            {
                lock (_locker)
                {
                    _internalLot = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.ILot, value);
                }
            }
        }
        private string _internalLot;

        /// <summary>
        /// Belső LOT-ban kódolt dátum
        /// </summary>
        public string InternalLotDate
        {
            get { lock (_locker) { return _internalLotDate; } }
            set
            {
                lock (_locker)
                {
                    _internalLotDate = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.ILotDate, value);
                }
            }
        }
        private string _internalLotDate;

        /// <summary>
        /// Belső LOT-ban kódolt SN
        /// </summary>
        public string InternalLotSN
        {
            get { lock (_locker) { return _internalLotSN; } }
            set
            {
                lock (_locker)
                {
                    _internalLotSN = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.ILotSN, value);
                }
            }
        }
        private string _internalLotSN;

        /// <summary>
        /// Belső cikkszám
        /// </summary>
        public string InternalPartNumber
        {
            get { lock (_locker) { return _internalPartNumber; } }
            set
            {
                lock (_locker)
                {
                    _internalPartNumber = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IPN, value);
                }
            }
        }
        private string _internalPartNumber;

        /// <summary>
        /// Beszállító belső kódja
        /// </summary>
        public string InternalVendor
        {
            get { lock (_locker) { return _internalVendor; } }
            set
            {
                lock (_locker)
                {
                    _internalVendor = value;
                    WriteToCache<string>(DataPoolDefinition.Reel.IVendor, value);
                }
            }
        }
        private string _internalVendor;

        /// <summary>
        /// Jelzi, hogy üres a tekercspozició
        /// </summary>
        public bool Empty
        {
            get { lock (_locker) { return _empty; } }
            set
            {
                lock (_locker)
                {
                    _empty = value;
                    WriteToCache<Boolean>(DataPoolDefinition.Reel.Empty, value);
                }
            }
        }
        private bool _empty = true;

        /// <summary>
        /// Jelzi, hogy könyvelés szükséges
        /// </summary>
        public bool BookingNeed
        {
            get { lock (_locker) { return _bookingNeed; } }
            set
            {
                lock (_locker)
                {
                    _bookingNeed = value;
                }
            }
        }
        private bool _bookingNeed = false;

        /// <summary>
        /// Az ellenőrzést végző felhasználó
        /// </summary>
        public string UserName
        {
            get { lock (_locker) { return _userName; } }
            set
            {
                lock (_locker)
                {
                    _userName = value;
                }
            }
        }
        private string _userName;

        /// <summary>
        /// Minden az azonosítás műveletnél keletkezett adat
        /// </summary>
        public List<KeyAndValue> AllIdentificationData
        {
            get
            {
                lock(_locker)
                {
                    return _allIdentificationData;
                }
            }
        }
        private List<KeyAndValue> _allIdentificationData = new List<KeyAndValue>();

        /// <summary>
        /// Minden a nyomtatott címkéről visszaolvasott adat
        /// </summary>
        public List<KeyAndValue> AllCheckData
        {
            get
            {
                lock (_locker)
                {
                    return _allCheckData;
                }
            }
        }
        private List<KeyAndValue> _allCheckData = new List<KeyAndValue>();


        /// <summary>
        /// Visszadja az objektumot reprezentáló Reel DAL entitást
        /// </summary>
        public DAL.Reel ReelEntity
        {
            get
            {
                return new DAL.Reel()
                {
                    BackgroundSystemResponse = BackgroundSystemResponse,
                    BackgroundSystemResult = BackgroundSystemResult.ToString().GetWithMaxLength(10).ToUpper(),
                    CheckCamera = CheckCamera.GetWithMaxLength(25),
                    CheckHandheldReadAttempts = CheckHandheldReadAttempts,
                    CheckReadAttempts = CheckReadAttempts,
                    CheckReadsData = CheckReadsData.ToString(),
                    CheckResult = CheckResult.ToString().GetWithMaxLength(10).ToUpper(),
                    EndTimeStamp = EndTimeStamp.HasValue ? EndTimeStamp.Value : DateTime.Now,
                    IdCamera = IdCamera.GetWithMaxLength(25),
                    IdentificationHandheldReadAttempts = IdentificationHandheldReadAttempts,
                    IdentificationReadAttempts = IdentificationReadAttempts,
                    IdentificationReadsData = IdentificationReadsData.ToString(),
                    IdentificationResult = IdentificationResult.ToString().GetWithMaxLength(10).ToUpper(),
                    InternalFVS = InternalFVS.GetWithMaxLength(100),
                    InternalLot = InternalLot.GetWithMaxLength(100),
                    InternalMTS = InternalMTS.GetWithMaxLength(100),
                    InternalPartNumber = InternalPartNumber.GetWithMaxLength(100),
                    InternalQty = InternalQty.GetWithMaxLength(100),
                    InternalVendor = InternalVendor.GetWithMaxLength(100),
                    LabelId = LabelId.GetWithMaxLength(100),
                    PrintAttempts = PrintAttempts,
                    Printer = Printer.GetWithMaxLength(25),
                    ProcessId = _processId.ToString().GetWithMaxLength(10).ToUpper(),
                    Result = Result.ToString().GetWithMaxLength(10).ToUpper(),
                    SendToBackgroundSystem = SendToBackgroundSystem,
                    StartTimeStamp = StartTimeStamp,
                    SupplierFVS = SupplierFVS.GetWithMaxLength(100),
                    SupplierLot = SupplierLot.GetWithMaxLength(100),
                    SupplierPartNumber = SupplierPartNumber.GetWithMaxLength(100),
                    SupplierQty = SupplierQty.GetWithMaxLength(100),
                    SupplierReelSerialNumber = SupplierReelSerialNumber.GetWithMaxLength(100),
                    UserName = _userName.GetWithMaxLength(50),
                    Vendor = Vendor.GetWithMaxLength(100),
                    WorkstationId = _workstationId.GetWithMaxLength(10).ToUpper(),
                };
            }
        }

        /// <summary>
        /// Inicializálja a tekercs adatokat
        /// </summary>
        private void ResetData()
        {
            BCList = String.Empty;
            CheckFVS = String.Empty;
            CheckMTSId = String.Empty;
            IdCamera = String.Empty;
            CheckCamera = String.Empty;
            Printer = String.Empty;
            StartTimeStamp = DateTime.MinValue;
            BackgroundSystemResponse = String.Empty;
            BackgroundSystemResult = BackgroundSystemResult.None;
            CheckCamera = String.Empty;
            CheckHandheldReadAttempts = 0;
            CheckReadAttempts = 0;
            CheckReadsData = null;
            CheckResult = CheckResult.None;
            CheckReadResult = CheckResult.None;
            CheckRegexCheckResult = CheckResult.None;
            CheckExternalBookingResult = ExternalBookingResult.None;
            EndTimeStamp = null;
            IdCamera = String.Empty;
            IdentificationHandheldReadAttempts = 0;
            IdentificationReadAttempts = 0;
            IdentificationReadsData = null;
            IdentificationResult = IdentificationResult.None;
            IdentificationReadResult = IdentificationResult.None;
            IdentificationRegexCheckResult = IdentificationResult.None;
            IdentificationExternalCheckResult = IdentificationResult.None;
            InternalFVS = String.Empty;
            InternalLot = String.Empty;
            InternalMTS = String.Empty;
            InternalPartNumber = String.Empty;
            InternalQty = String.Empty;
            InternalVendor = String.Empty;
            InternalLotDate = String.Empty;
            InternalLotSN = String.Empty;
            SendToBackgroundSystem = String.Empty;
            Status = String.Empty;
            LabelId = String.Empty;
            PrintAttempts = 0;
            Printer = String.Empty;
            PrintResult = PrintResult.None;
            StickingResult = PrintResult.None;
            Result = Result.None;
            SendToBackgroundSystem = String.Empty;
            StartTimeStamp = DateTime.MinValue;
            SupplierFVS = String.Empty;
            SupplierLot = String.Empty;
            SupplierPartNumber = String.Empty;
            SupplierQty = String.Empty;
            SupplierReelSerialNumber = String.Empty;
            Vendor = String.Empty;
            UserName = String.Empty;
            ErrorMessage = String.Empty;
            GoldenSample = false;
            BackgroundSystemMessage = string.Empty;
        }

        /// <summary>
        /// Nulláza a Redis DP-t az aktuális pozición az objektum adtaival
        /// </summary>
        private void ResetPosition()
        {
            WriteToCache(DataPoolDefinition.Reel.BCList, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.CFVS, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.CMTSID, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.BCListId, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.BCListCheck, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IdData, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IdDataAck, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.LabelId, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SVendor, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SPN, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SLot, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SFVS, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SRSRN, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.SQty, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IMTSId, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IFVS, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IQty, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.ILot, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.ILotDate, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.ILotSN, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IPN, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.IVendor, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.Empty, true);
            WriteToCache(DataPoolDefinition.Reel.IdentificationReadResult, IdentificationResult.None);
            WriteToCache(DataPoolDefinition.Reel.IdentificationRegexCheckResult, IdentificationResult.None);
            WriteToCache(DataPoolDefinition.Reel.IdentificationExternalCheckResult, IdentificationResult.None);
            WriteToCache(DataPoolDefinition.Reel.PrintResult, PrintResult.None);
            WriteToCache(DataPoolDefinition.Reel.StickingResult, PrintResult.None);
            WriteToCache(DataPoolDefinition.Reel.CheckReadResult, CheckResult.None);
            WriteToCache(DataPoolDefinition.Reel.CheckRegexCheckResult, CheckResult.None);
            WriteToCache(DataPoolDefinition.Reel.CheckExternalBookingResult, CheckResult.None);
            WriteToCache(DataPoolDefinition.Reel.ReelResult, Result.None);
            WriteToCache<DateTime>(DataPoolDefinition.Reel.CycleStartTimeStamp, DateTime.MinValue);
            WriteToCache(DataPoolDefinition.Reel.ErrorMessage, String.Empty);
            WriteToCache(DataPoolDefinition.Reel.GSMode, false);
            WriteToCache(DataPoolDefinition.Reel.BackGroundSystemMessage, string.Empty);
        }

        /// <summary>
        /// Megfrissíti a Redis DP-t az aktuális pozición az objektum adtaival
        /// </summary>
        private void RefreshPosition()
        {
            WriteToCache(DataPoolDefinition.Reel.BCList, _bclist);
            WriteToCache(DataPoolDefinition.Reel.CFVS, _checkFVS);
            WriteToCache(DataPoolDefinition.Reel.CMTSID, _checkMTSId);
            WriteToCache<string>(DataPoolDefinition.Reel.BCListId, _identificationReadsData);
            WriteToCache<string>(DataPoolDefinition.Reel.BCListCheck, _checkReadsData);
            WriteToCache<string>(DataPoolDefinition.Reel.IdData, _sendToBackgroundSystem);
            WriteToCache<string>(DataPoolDefinition.Reel.IdDataAck, _backgroundSystemResponse);
            WriteToCache<string>(DataPoolDefinition.Reel.LabelId, _labelId);
            WriteToCache<string>(DataPoolDefinition.Reel.SVendor, _vendor);
            WriteToCache<string>(DataPoolDefinition.Reel.SPN, _supplierPartNumber);
            WriteToCache<string>(DataPoolDefinition.Reel.SLot, _suplierLot);
            WriteToCache<string>(DataPoolDefinition.Reel.SFVS, _suplierFVS);
            WriteToCache<string>(DataPoolDefinition.Reel.SRSRN, _suplierReelSerialNumber);
            WriteToCache<string>(DataPoolDefinition.Reel.SQty, _suplierQty);
            WriteToCache<string>(DataPoolDefinition.Reel.IMTSId, _internalMTS);
            WriteToCache<string>(DataPoolDefinition.Reel.IFVS, _internalFVS);
            WriteToCache<string>(DataPoolDefinition.Reel.IQty, _internalQty);
            WriteToCache<string>(DataPoolDefinition.Reel.ILot, _internalLot);
            WriteToCache(DataPoolDefinition.Reel.ILotDate, _internalLotDate);
            WriteToCache(DataPoolDefinition.Reel.ILotSN, _internalLotSN);
            WriteToCache<string>(DataPoolDefinition.Reel.IPN, _internalPartNumber);
            WriteToCache<string>(DataPoolDefinition.Reel.IVendor, _internalVendor);
            WriteToCache<bool>(DataPoolDefinition.Reel.Empty, _empty);
            WriteToCache(DataPoolDefinition.Reel.IdentificationReadResult, _identificationReadResult);
            WriteToCache(DataPoolDefinition.Reel.IdentificationRegexCheckResult, _identificationRegexCheckResult);
            WriteToCache(DataPoolDefinition.Reel.IdentificationExternalCheckResult, _identificationExternalCheckResult);
            WriteToCache(DataPoolDefinition.Reel.PrintResult, _printResult);
            WriteToCache(DataPoolDefinition.Reel.StickingResult, _stickingResult);
            WriteToCache(DataPoolDefinition.Reel.CheckReadResult, _checkReadResult);
            WriteToCache(DataPoolDefinition.Reel.CheckRegexCheckResult, _checkRegexCheckResult);
            WriteToCache(DataPoolDefinition.Reel.CheckExternalBookingResult, _checkExternalBookingResult);
            WriteToCache(DataPoolDefinition.Reel.ReelResult, _result);
            WriteToCache<DateTime>(DataPoolDefinition.Reel.CycleStartTimeStamp, _startTimeStamp);
            WriteToCache<string>(DataPoolDefinition.Reel.ErrorMessage, _errorMessage);
            WriteToCache(DataPoolDefinition.Reel.GSMode, _goldenSample);
            WriteToCache(DataPoolDefinition.Reel.BackGroundSystemMessage, _backgroundSystemMessage);
        }

        /// <summary>
        /// Kiírja az adatot a Redis cash-re
        /// </summary>
        /// <typeparam name="T">kiírandó adat típusa</typeparam>
        /// <param name="key">az irandó kulcs</param>
        /// <param name="value">érték</param>
        private void WriteToCache<T>(DataPoolDefinition.Reel key, T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    _instanceWriter.WriteKeyValue<string>(key.GetRedisKey(_reelPosition), value.ToString().ToUpper());
                }
                else
                {
                    _instanceWriter.WriteKeyValue<T>(key.GetRedisKey(_reelPosition), value);
                }
            }
            catch(Exception)
            {                
            }
        }

        /// <summary>
        /// Munkahely azonosító
        /// </summary>
        private readonly string _workstationId;

        /// <summary>
        /// Munkahely típus
        /// </summary>
        private WorkStationType _workstationType;

        /// <summary>
        /// Folyamat azonosító
        /// </summary>
        private ProcessType _processId;

        /// <summary>
        /// Tekercs sorszáma
        /// </summary>
        private int _reelPosition;

        /// <summary>
        /// Redis DataPool instance writer
        /// </summary>
        private InstanceWriter _instanceWriter;

        /// <summary>
        /// MT Locker
        /// </summary>
        private object _locker = new object();

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    lock (_locker)
                    {
                        // dispose managed state (managed objects).
                        ResetData();
                        _instanceWriter = null;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReelData() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
