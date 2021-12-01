using System;

namespace ReelCheck.Core
{
    /// <summary>
    /// Munkahely típusa
    /// </summary>
    public enum WorkStationType
    {
        /// <summary>
        /// Automata munkahely
        /// </summary>
        Automatic = 0,
        /// <summary>
        /// Félautomata munkahely
        /// </summary>
        SemiAutomatic = 1,
    }

    /// <summary>
    /// Folyamat típusok
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// Nem folymat elem
        /// </summary>
        None = 0,
        /// <summary>
        /// Cimke
        /// </summary>
        Label = 1,
    }

    /// <summary>
    /// Állomás üzemmódjai
    /// </summary>
    public enum ReelCheckMode
    {
        WaitLogin,
        Normal,
        GoldenSample,
        Reset,
        HandHeld,
    }

    /// <summary>
    /// Műveletek lehetséges státuszai
    /// </summary>
    public enum OperationStatus
    {
        Operating,
        Wait,
        Ready,
    }

    /// <summary>
    /// Állomás lehetséges állapotai
    /// </summary>
    public enum MainStatus
    {
        Auto,
        Manual,
    }

    /// <summary>
    /// Digitális ki/bemenet típusa
    /// </summary>
    public enum DigitalIOType
    {
        /// <summary>
        /// Kamera ki/bemenet
        /// </summary>
        IO,
        /// <summary>
        /// IOExt ki/bemenet
        /// </summary>
        IOExt,
    }

    /// <summary>
    /// Reelcheck műveletek
    /// </summary>
    public enum Operations
    {
        /// <summary>
        ///  A gép ciklusa
        /// </summary>
        Machine,
        /// <summary>
        /// Azonosítás
        /// </summary>
        Identification,
        /// <summary>
        /// Nyomtatás
        /// </summary>
        Print,
        /// <summary>
        /// Ellenőrzés
        /// </summary>
        Check,
    }

    /// <summary>
    /// Ellenőrzés eredménye
    /// </summary>
    public enum BackgroundSystemResult
    {
        /// <summary>
        /// Nincs
        /// </summary>
        None,
        /// <summary>
        /// Engedélyezett
        /// </summary>
        Pass,
        /// <summary>
        /// Tiltott
        /// </summary>
        Fail,
        /// <summary>
        /// Nincs válasz
        /// </summary>
        NoAck,
    }

    /// <summary>
    /// A teljes folymat visszatérési értéke
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Még nincs
        /// </summary>
        None,
        /// <summary>
        /// Hiba/tiltott
        /// </summary>
        Fail,
        /// <summary>
        /// Ok/ngedélyezett
        /// </summary>
        Pass,
        /// <summary>
        /// A tekercs pozició üres
        /// </summary>
        Empty,
        /// <summary>
        /// aranymintateszt tekercs státusza: sikeres 
        /// </summary>
        GsPass,
        /// <summary>
        /// aranymintateszt tekercs státusza: sikertelen 
        /// </summary>
        GsFail,
    }

    /// <summary>
    /// Camera visszatérése
    /// </summary>
    public enum CheckResult
    {
        /// <summary>
        /// Nincs még eredmény
        /// </summary>
        None,
        /// <summary>
        /// Ok (olvasott és a visszaolvasott adatok egyeznek)
        /// </summary>
        Pass,
        /// <summary>
        /// Nem olvasott semmit
        /// </summary>
        NoRead,
        /// <summary>
        /// Üres címkét olcvasott
        /// </summary>
        Empty,
        /// <summary>
        /// Nem sikerült a BarcodeMatch
        /// </summary>
        DataErr,
        /// <summary>
        /// A vissazolvasott adatok nem egyeznek
        /// </summary>
        NOK,
        /// <summary>
        /// Folyamatban
        /// </summary>
        InProgress,
    }

    public enum ExternalBookingResult
    {
        /// <summary>
        /// Nincs még eredmény
        /// </summary>
        None,
        /// <summary>
        /// Folyamatban
        /// </summary>
        InProgress,
        /// <summary>
        /// Üres tekercs pozició
        /// </summary>
        Empty,
        /// <summary>
        /// A CustomFVS hibát jelezett, vagy hiba a config adatokban
        /// </summary>
        DataErr,
        /// <summary>
        /// Sikertelen CustomFVS kommunikáció
        /// </summary>
        NoAck,
        /// <summary>
        /// Sikeresen jelezve a customFVS-nek, hogy a tekercs zöld státuszt kapva forgott ki
        /// </summary>
        PassBookOK,
        /// <summary>
        /// Sikeresen jelezve a customFVS-nek, hogy a tekercs piros státuszt kapva forgott ki
        /// </summary>
        PassBookNOK
    }

    /// <summary>
    /// Azonosítási művelet lehetséges eredményei
    /// </summary>
    public enum IdentificationResult
    {
        /// <summary>
        /// Az azonosításnak még nincs eredménye
        /// </summary>
        None,
        /// <summary>
        /// Pass (olvasott)
        /// </summary>
        Pass,
        /// <summary>
        /// Nem olvasott semmit
        /// </summary>
        NoRead,
        /// <summary>
        /// Üres címkét olcvasott
        /// </summary>
        Empty,
        /// <summary>
        /// Nem sikerült a BarcodeMatch
        /// </summary>
        DataErr,
        /// <summary>
        /// A CUSTOMFVS rendszer nem válaszolt
        /// </summary>
        NoAck,
        /// <summary>
        /// A CUSTOMFVS rendszer elutasította a tekercset
        /// </summary>
        Refuse,
        /// <summary>
        /// Folyamatban
        /// </summary>
        InProgress,
    }

    /// <summary>
    /// Nyomtatás művelet lehetséges eredményei
    /// </summary>
    public enum PrintResult
    {
        /// <summary>
        /// A Nyomtatásnak még nincs eredménye
        /// </summary>
        None,
        /// <summary>
        /// Sikeres (a kinyomtatatndó címke rendben kinyomtatódott)
        /// </summary>
        Pass,
        /// <summary>
        /// Hiba lépett fel, és végül kihagyták a nyomtatást
        /// </summary>
        Skip,
        /// <summary>
        /// Üres tekercs pozició
        /// </summary>
        Empty,
        /// <summary>
        /// Folyamatban
        /// </summary>
        InProgress,
    }

    /// <summary>
    /// Kamera típusa
    /// </summary>
    public enum CameraType
    {
        /// <summary>
        /// Tekercs azonosító kamera
        /// </summary>
        Id = 1,
        /// <summary>
        /// Ellenőrző kamera
        /// </summary>
        Check = 2,
    }

    /// <summary>
    /// A fő folymamat állapotai
    /// </summary>
    public enum MainState
    {
        /// <summary>
        /// Program indulása
        /// </summary>
        Starting = 0,
        /// <summary>
        /// ReelCheck nem áll készen vár az aktiválásra/bejelentkezésre
        /// </summary>
        NotReady = 1,
        /// <summary>
        /// ReelCheck készen áll. Várakozik az Enable jelre, és hatására elindítja majd a folymamatot
        /// </summary>
        Ready = 2,
        /// <summary>
        /// Visszajelzi és menti a 4. tekercs eredményét, majd betölt egy új (üres) tekercset a helyére, amig nem végez, nem indul a folyamat, Enable jeleket elutasítja
        /// </summary>
        FeedBack = 3,
        /// <summary>
        /// A végrehajtás alatt, a Enable jeleket elutasítja
        /// </summary>
        AtWork = 4,
        /// <summary>
        /// Program leállása
        /// </summary>
        Stopping = 5,
        /// <summary>
        /// Program leállt
        /// </summary>
        Stopped = 6,
    }

    /// <summary>
    /// A fő folymamat triggerei
    /// </summary>
    public enum MainTrigers
    {
        /// <summary>
        /// program betöltve, munkára kész
        /// </summary>
        Loaded,
        /// <summary>
        /// Program leállítása
        /// </summary>
        Stop,
        /// <summary>
        /// A program leállt
        /// </summary>
        Stopped,
        /// <summary>
        /// Munkahely aktiválva 
        /// </summary>
        Activate,
        /// <summary>
        /// Munkahely inaktiválva
        /// </summary>
        InActivate,
        /// <summary>
        /// Megkapta a géptől a jelet, aminek hatására indul(nak) az alfolyamat(ok)
        /// </summary>
        ReceiveEnableSignal,
        /// <summary>
        /// Folymamtok indítása
        /// </summary>
        StartWorking,
        /// <summary>
        /// Minden alfolyamat befejeződött
        /// </summary>
        AllReady,
        /// <summary>
        /// Folymamt reste (csak LABELE)
        /// </summary>
        Reset,
        /// <summary>
        /// Csak LABELE: Feedback folyamatra lép
        /// </summary>
        GoFeedback,
    }

    /// <summary>
    /// Azonosítás folyamat állapotai
    /// </summary>
    public enum IdentificationState
    {
        /// <summary>
        /// Nyugalmi helyzet
        /// </summary>
        Rest,
        /// <summary>
        /// Várakozik az IDCAMENABLE jel HIGH állapotára
        /// </summary>
        WaitForEnable,
        /// <summary>
        /// Azonosító címke olvasása
        /// </summary>
        Read,
        /// <summary>
        /// Olvasott címke felismerése
        /// </summary>
        BarcodeMatch,
        /// <summary>
        /// Olvasott adatok átadása a külső rendszernek
        /// </summary>
        IdData,
        /// <summary>
        /// Az id camrea enable (Trigger1) alacsony állapotára várakozik (csak LabelE)
        /// </summary>
        WaitForIdCameraEnableToLow,
        /// <summary>
        /// Művelet zárása, eredmények mentése
        /// </summary>        
        CloseOperation,
    }

    /// <summary>
    /// Azonosítás művelet trigerei
    /// </summary>
    public enum IdentificationTriggers
    {
        /// <summary>
        /// Indítás
        /// </summary>
        Start,
        /// <summary>
        /// Olvasás indítása
        /// </summary>
        StartRead,
        /// <summary>
        /// Címkeolvasás vége
        /// </summary>
        ReadEnd,
        /// <summary>
        /// Nincs tekercs betéve (üres cimke olvasás)
        /// </summary>
        EmptyRead,
        /// <summary>
        /// Sikertelen olvasás a kamera által
        /// </summary>
        NoRead,
        /// <summary>
        /// Nem sikerült felismerni a cimkét/adatot kinyerni róla 
        /// </summary>
        NoMatch,
        /// <summary>
        /// Cimke felismerés ok, Engedély kérése a külső rendszertől  
        /// </summary>
        CallIdData,
        /// <summary>
        /// Aranyminta tesztben ez a trigger visza a BarcodeMatch után a CloseOperation-re
        /// </summary>
        GoldenSample,
        /// <summary>
        /// Tiltás
        /// </summary>        
        Fail,
        /// <summary>
        /// Engedélyezés
        /// </summary>
        Pass,
        /// <summary>
        /// Címke újraolvasása
        /// </summary>
        Reread,
        /// <summary>
        /// Id Camera enable jel (Trigger1) visszamegy alacsonyra
        /// </summary>
        IdCamEnableChangedToLow,
        /// <summary>
        /// Folymmat befelyezése, vissza a start állapotba
        /// </summary>
        End      
    }

    /// <summary>
    /// A nyomtatás művelet állapotai
    /// </summary>
    public enum PrintState
    {
        /// <summary>
        /// Nyugalmi állapot
        /// </summary>
        Rest,
        /// <summary>
        /// Nyomtatás
        /// </summary>
        Print,
        /// <summary>
        /// Beavatkozásra vár
        /// </summary>
        WaitForIntervention,
        /// <summary>
        /// Nem volt sikeres nyomtatás
        /// </summary>
        NoPrint,
        /// <summary>
        /// Sikeres nyomtatás
        /// </summary>
        PrintOk,
        /// <summary>
        /// A cimke felragasztására várakozik
        /// </summary>
        WaitForSticking,
        /// <summary>
        /// Várakozik, hogy a PLC visszavegye a StickingDone jelet
        /// </summary>
        WaitForStickingDoneReset,
        /// <summary>
        /// Print folyamat vége
        /// </summary>
        PrintProcessEnd,
    }

    /// <summary>
    /// A nyomtatás művelet triggerei
    /// </summary>
    public enum PrintTriggers
    {
        /// <summary>
        /// Nyomtatás indítása
        /// </summary>
        Start,
        /// <summary>
        /// Nyomtatás sikertelen
        /// </summary>
        PrintNok,
        /// <summary>
        /// Nyomtatás rendben
        /// </summary>
        PrintOk,
        /// <summary>
        /// Nyomtatás kihagyása
        /// </summary>
        SkipPrint,
        /// <summary>
        /// Újranyomtatás
        /// </summary>
        Reprint,
        /// <summary>
        /// Wárakozás cimnkézésre
        /// </summary>
        WaitForSticking,
        /// <summary>
        /// Cimkézés befejeződött
        /// </summary>
        StickingDone,
        /// <summary>
        /// Print folyamat vége
        /// </summary>
        ProcessEnd,
        /// <summary>
        /// Vége, vissza nyugalmi állapotba
        /// </summary>        
        End,
    }

    /// <summary>
    /// Ellenőrzés művelet állapotai
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// Nyugalmi állapot
        /// </summary>
        Rest,
        /// <summary>
        /// LABELE: Az engedélyező jel megérkezésére vár
        /// </summary>
        WaitForEnable,
        /// <summary>
        /// Ellenőrzés művelet indítása
        /// </summary>
        StartCheck,
        /// <summary>
        /// Cimke olvasás
        /// </summary>
        Read,
        /// <summary>
        /// Olvasott címke azonosítása
        /// </summary>
        BarcodeMatch,
        /// <summary>
        /// Címketartalom kiértékelése
        /// </summary>
        Check,
        /// <summary>
        /// Művelet lezárása
        /// </summary>
        CloseOperation,
    }

    /// <summary>
    /// Ellenőrzés művelet triggerei
    /// </summary>
    public enum CheckTrigger
    {
        /// <summary>
        /// Művelet indítás
        /// </summary>
        Start,
        /// <summary>
        /// LABELE: Várakozéás a PLC engedélyező jelére
        /// </summary>
        StartWaitForEnable,
        /// <summary>
        /// Nem kell ellenőrzést végezni
        /// </summary>
        NotNeed,
        /// <summary>
        /// Címkeolvasás indítása
        /// </summary>
        StartRead,
        /// <summary>
        /// Olvasott címke felismerése/adatok kinyerésénak indítása
        /// </summary>
        StartBarcodeMatch,
        /// <summary>
        /// Kiértékelés indítása
        /// </summary>
        StartCheck,
        /// <summary>
        /// Kiértékelés vége
        /// </summary>
        EndCheck,
        /// <summary>
        /// Üres pozíció
        /// </summary>
        EmptyRead,
        /// <summary>
        /// Sikertelen olvasás
        /// </summary>
        NoRead,
        /// <summary>
        /// A regex checcker nem tudta azonosítani a címkét a konfigurációban lévő adefiniciók alapján
        /// </summary>
        NoMatch,
        /// <summary>
        /// Művelet vége
        /// </summary>
        End,
    }

    /// <summary>
    /// Kétállapotú kapcsoló
    /// </summary>
    public enum OnOff
    {
        /// <summary>
        /// Kikapcsolt
        /// </summary>
        Off = 0,
        /// <summary>
        /// Bekapcsolt
        /// </summary>
        On = 1,
    }

    /// <summary>
    /// Felismert különleges label típusok
    /// </summary>
    public enum SpecialLabelType
    {
        /// <summary>
        /// Ismeretlen (fel nem ismert címke)
        /// </summary>
        Unknown,
        /// <summary>
        /// Üres a tekercspozició
        /// </summary>
        Empty,
        /// <summary>
        /// Kamera nem olvasott cimkét
        /// </summary>
        NoRead,
    }

    /// <summary>
    /// Könyvelési rekord kategóriák
    /// </summary>
    public enum BookingRecord
    {
        /// <summary>
        /// Üres tekercspozició
        /// </summary>
        Empty,
        /// <summary>
        /// Siekertelen azonosítás/ellenőrzés 
        /// </summary>
        NoRead,
        /// <summary>
        /// Adat hiba
        /// </summary>
        DataError,
        /// <summary>
        /// Nincs válasz Custom FVS-től
        /// </summary>
        NoAck,
        /// <summary>
        /// Custom FVS elutasította
        /// </summary>
        Refuse,
        /// <summary>
        /// Tekercs rendben
        /// </summary>
        Passed,
    }

    /// <summary>
    /// Booking rekord enum bővítő metodusait hordozó osztály
    /// </summary>
    public static class BookingRecordExtension
    {
        /// <summary>
        /// Bővítő metódus, hogy visszadja a Booking rekord konfigurációban használt randa nevét
        /// </summary>
        public static string MyUglyName(this BookingRecord bookingRecord)
        {
            switch (bookingRecord)
            {
                case BookingRecord.Empty:
                    return "RESULT_FAIL_EMPTY";
                case BookingRecord.NoRead:
                    return "RESULT_FAIL_NOREAD";
                case BookingRecord.DataError:
                    return "RESULT_FAIL_DATEAERR";
                case BookingRecord.NoAck:
                    return "RESULT_FAIL_NOACK";
                case BookingRecord.Refuse:
                    return "RESULT_FAIL_REFUSE";
                case BookingRecord.Passed:
                    return "RESULT_PASS";
                default:
                    return String.Empty;
            }
        }
    }       

    /// <summary>
    /// Blokkolás okok
    /// </summary>
    public enum BlockReason
    {
        NonBlocked,
        /// <summary>
        /// Beavatkozással blokkolták a működést
        /// </summary>
        Manual,
        /// <summary>
        /// Hibás tekercs forog ki
        /// </summary>
        ReelProcessingFailed,
        /// <summary>
        /// Aranyminta teszt elbukott
        /// </summary>
        GoldenSampleTestFailed,
        /// <summary>
        /// Aranyminta teszt esedékessé vált
        /// </summary>
        GoldenSampleTestDue,
        /// <summary>
        /// Az azonosításkor  aCustom FVS fontos, megjelenítendő üzenetet hagyott, amit  akezelőnek jová kell hgynia
        /// </summary>
        CustomFVSMessage,
    }
}
