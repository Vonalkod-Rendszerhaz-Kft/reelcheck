using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core
{
    public static class TrueWordCodes
    {
        private const string HU_LANGCODE = "hu-HU";

        public static class ReelCheckService
        {
            public static class UiMessage
            {
                [InitializeTranslation("konfiguráció", HU_LANGCODE)]
                public static class Configuration { }
                [InitializeTranslation("szekció", HU_LANGCODE)]
                public static class Section { }
                public static class Interventions
                {
                    [InitializeTranslation("Ismeretlen beavatkozás!", HU_LANGCODE)]
                    public static class UnknownIntervention { }
                    public static class SkipPrint
                    {
                        [InitializeTranslation("Nyomtatás átlépése sikeres", HU_LANGCODE)]
                        public static class Success { }
                        [InitializeTranslation("A nyomtatás folyamat jelenleg nem igényel beavatkozást", HU_LANGCODE)]
                        public static class NoNeed { }
                    }
                    public static class RePrint
                    {
                        [InitializeTranslation("Újranyomtatás indítása sikeres", HU_LANGCODE)]
                        public static class Success { }
                        [InitializeTranslation("A nyomtatás folyamat jelenleg nem igényel beavatkozást", HU_LANGCODE)]
                        public static class NoNeed { }
                    }
                    public static class BadStates
                    {
                        [InitializeTranslation("Várjon, amíg a vezérlő szoftver elindul!", HU_LANGCODE)]
                        public static class Starting { }
                        [InitializeTranslation("Nincs bejelentkezve!", HU_LANGCODE)]
                        public static class NotLogin { }
                        [InitializeTranslation("A vezérlő szoftver leállítás alatt!", HU_LANGCODE)]
                        public static class Stopping { }
                        [InitializeTranslation("A vezérlő szoftver leállt!", HU_LANGCODE)]
                        public static class Stopped { }
                    }
                    public static class Activate
                    {
                        [InitializeTranslation("Sikeres bejelentkezés", HU_LANGCODE)]
                        public static class Success { }
                        [InitializeTranslation("Nem válthat felhasználót, mert tekercs azonosítás van folyamatban!", HU_LANGCODE)]
                        public static class InProgress { }
                        [InitializeTranslation("Felhasználónév megadása kötelező, talán nincs bejelentkezve!", HU_LANGCODE)]
                        public static class NoUserName { }
                    }
                    public static class Inactivate
                    {
                        [InitializeTranslation("Sikeres kijelentkezés", HU_LANGCODE)]
                        public static class Success { }
                        [InitializeTranslation("Jelenleg nem jelentkezhet ki, mert tekercs azonosítás van folyamatban!", HU_LANGCODE)]
                        public static class InProgress { }
                        [InitializeTranslation("Másik felhasználó van bejelentkezve! Csak ő jelentkezhet ki!", HU_LANGCODE)]
                        public static class OtherUser { }
                        [InitializeTranslation("Felhasználónév megadása kötelező, talán nincs bejelentkezve!", HU_LANGCODE)]
                        public static class NoUserName { }

                    }
                    public static class ManulaMode
                    {
                        public static class On
                        {
                            [InitializeTranslation("Sikeres adatbeküldés. Manual mód aktív. Helyezze a tekrcset az asztalra!", HU_LANGCODE)]
                            public static class Success { }
                            [InitializeTranslation("Manual mód csak a félautomata gépen kezdeményezhető!", HU_LANGCODE)]
                            public static class LabelEOnly { }
                            [InitializeTranslation("Ha nem ad meg FVS kódot, akkor a cikkszám megadása kötelező!", HU_LANGCODE)]
                            public static class PartNumberRequired { }
                            [InitializeTranslation("Ha nem ad meg FVS kódot, akkor a lotszám megadása kötelező!", HU_LANGCODE)]
                            public static class LotNumberRequired { }
                            [InitializeTranslation("Ha nem ad meg FVS kódot, akkor a mennyiség megadása kötelező!", HU_LANGCODE)]
                            public static class QtyRequired { }
                        }
                        public static class Off
                        {
                            [InitializeTranslation("Manualisan megadott adatok törlése sikeres.", HU_LANGCODE)]
                            public static class Success { }
                            [InitializeTranslation("Jelenkleg nincs érvényben manuálisan megadott adat!", HU_LANGCODE)]
                            public static class NoManualData { }
                        }
                    }
                    public static class GoldenSample
                    {
                        public static class Enter
                        {
                            [InitializeTranslation("Már fut az aranyminta teszt!", HU_LANGCODE)]
                            public static class AlreadyRun { }
                            [InitializeTranslation("Aranyminta teszt üzemmód kikapcsolva a konfigurációban!", HU_LANGCODE)]
                            public static class Off { }
                            [InitializeTranslation("Aranyminta teszt csak üres gépen indítható!", HU_LANGCODE)]
                            public static class NotEmpty { }
                            [InitializeTranslation("Aranyminta teszt indítása sikeres.", HU_LANGCODE)]
                            public static class Success { }
                        }
                        public static class Exit
                        {
                            [InitializeTranslation("Jelenleg nem fut aranyminta teszt!", HU_LANGCODE)]
                            public static class NotRun { }
                            [InitializeTranslation("Aranyminta teszt felfüggesztése sikeres.", HU_LANGCODE)]
                            public static class Success { }
                        }
                        public static class DeleteDue
                        {
                            [InitializeTranslation("Jelenleg nem esedékes aranyminta teszt!", HU_LANGCODE)]
                            public static class NotDue { }

                            [InitializeTranslation("Aranyminta teszt esedékesség törlése sikeres.", HU_LANGCODE)]
                            public static class Success { }
                        }
                        public static class SetDueNow
                        {
                            [InitializeTranslation("Már jelenleg is esedékes aranyminta tesztet végezni!", HU_LANGCODE)]
                            public static class AlreadyDue { }
                            [InitializeTranslation("Az aranyminta teszt már folymamtban van!", HU_LANGCODE)]
                            public static class AlreadyRun { }
                            [InitializeTranslation("Aranyminta teszt esedékesség beállítása sikeres.", HU_LANGCODE)]
                            public static class Success { }
                        }
                    }
                    public static class Blocking
                    {
                        public static class Block
                        {
                            [InitializeTranslation("A gép blokkolva van!", HU_LANGCODE)]
                            public static class Blocked { }
                            [InitializeTranslation("A manuális blokkolás le vanm tiltva a konfigurációban!", HU_LANGCODE)]
                            public static class ManualBlockingDisabled { }
                            [InitializeTranslation("A gép blokkolása megtörtént!", HU_LANGCODE)]
                            public static class Success { }
                        }
                        public static class UnBlock
                        {
                            [InitializeTranslation("A gép blokkolását sikeresen megszüntette!", HU_LANGCODE)]
                            public static class Success { }
                        }
                    }
                }
                public static class PrintOperation
                {
                    [InitializeTranslation("PRINT: Ezen a logikai néven nincs nyomtató definiálva a konfigurációs fájlban, vagy nem aktív státusszal szerepel!", HU_LANGCODE)]
                    public static class PrinterNotDefined { }
                    [InitializeTranslation("PRINT: Sikeres címke nyomtatás, várakozás a ragasztásra...", HU_LANGCODE)]
                    public static class PrintSuccess { }
                    [InitializeTranslation("A beállítások szerint ehhez a státuszhoz nem kell címkét nyomtatni.", HU_LANGCODE)]
                    public static class NoLabelThisStatus { }
                    [InitializeTranslation("A konfigurációban nincs cimke definiálva a PASS státuszhoz! Nyomtatás nem történt!", HU_LANGCODE)]
                    public static class PassPrintEventNotDefined { }
                    [InitializeTranslation("Nyomtatási hiba! Beavatkozás szükséges! (Újranyomtatás/Nyomtatás kihagyása)", HU_LANGCODE)]
                    public static class PrintError { }
                    [InitializeTranslation("A nyomtatási folyamat véget ért", HU_LANGCODE)]
                    public static class PrintEnd { }
                    [InitializeTranslation("Nincs szükség nyomtatásra! (Üres pozició!)", HU_LANGCODE)]
                    public static class PrintNoNeed { }
                    [InitializeTranslation("A nyomtatást a felhasználó átlépte!", HU_LANGCODE)]
                    public static class SkipPrint { }
                }
                public static class IdentificationOperation
                {
                    [InitializeTranslation("Üres tekercs pozició!", HU_LANGCODE)]
                    public static class EptyReelPosition { }
                    [InitializeTranslation("A tekercsen lévő címkét nem ismeri fel a kamera!", HU_LANGCODE)]
                    public static class NoRead { }
                    [InitializeTranslation("A címke azonosítva, de hiba az adatokkal!", HU_LANGCODE)]
                    public static class LabelsSectionError { }
                    [InitializeTranslation("A cimke azonosítva, de hiba az adatokkal!", HU_LANGCODE)]
                    public static class LabelMessagesSectionError { }
                    [InitializeTranslation("Nincs definiálva CustomFVS üzenet!", HU_LANGCODE)]
                    public static class OutgoingSectionError { }
                    [InitializeTranslation("Nincs definiálva CustomFVS-től kapott válaszüzenet!", HU_LANGCODE)]
                    public static class IncommingSectionError { }
                    [InitializeTranslation("Hiba a CustomFVS kommunikációban!", HU_LANGCODE)]
                    public static class CustomFVSNoAck { }
                    [InitializeTranslation("A CustomFVS elutasította a tekercset!", HU_LANGCODE)]
                    public static class CustomFVSFail { }
                }
                public static class CheckOperation
                {
                    [InitializeTranslation("Olvasásái hiba az ellenőrző kamerán!", HU_LANGCODE)]
                    public static class CheckReadError { }
                    [InitializeTranslation("Üres tekercs pozició!", HU_LANGCODE)]
                    public static class EptyReelPosition { }
                    [InitializeTranslation("A tekercsen lévő címkét nem ismeri fel a kamera!", HU_LANGCODE)]
                    public static class NoRead { }
                    [InitializeTranslation("A címke azonosítva, de hiba az adatokkal!", HU_LANGCODE)]
                    public static class LabelsSectionError { }
                    [InitializeTranslation("A cimke azonosítva, de hiba az adatokkal!", HU_LANGCODE)]
                    public static class LabelMessagesSectionError { }
                    [InitializeTranslation("Hiba a CustomFVS kommunikációban!", HU_LANGCODE)]
                    public static class CustomFVSNoAck { }
                    [InitializeTranslation("A CustomFVS hibát jelzett a tekercs státuszának könyvelésére!", HU_LANGCODE)]
                    public static class CustomFVSFail { }
                    [InitializeTranslation("Hibásak a nyomtatott címkén lévő adatok a definiált ellenőrző feltételek szerint!", HU_LANGCODE)]
                    public static class CheckSuccessError { }
                    [InitializeTranslation("Nincs definiálva CustomFVS könyvelő üzenet!", HU_LANGCODE)]
                    public static class OutgoingSectionError { }


                }
                public static class Blocking
                {
                    [InitializeTranslation("A munkahely blokkolva!", HU_LANGCODE)]
                    public static class Blocked { }
                    [InitializeTranslation("A blokkolás oka:", HU_LANGCODE)]
                    public static class BlocReason { }
                    public static class Reasons
                    {
                        [InitializeTranslation("Manuális blokkolás", HU_LANGCODE)]
                        public static class Manual { }
                        [InitializeTranslation("Elbukott aranyminta teszt", HU_LANGCODE)]
                        public static class GoldenSampleTestFailed { }
                        [InitializeTranslation("Aranyminta teszt esedékes", HU_LANGCODE)]
                        public static class GoldenSampleTestDue { }
                        [InitializeTranslation("Hibás tekercsazonosítás", HU_LANGCODE)]
                        public static class ReelProcessingFailed { }
                    }
                }

            }
            public static class PrintedLabelMessage
            {
                [InitializeTranslation("Az azonosító kamera által nem ismert cimke!", HU_LANGCODE)]
                public static class NoRead { }
                [InitializeTranslation("Adathiba a címke felismerés során!", HU_LANGCODE)]
                public static class DataError { }
                [InitializeTranslation("A CustomFVS rendszer nem elérhető!", HU_LANGCODE)]
                public static class NoAck { }
                [InitializeTranslation("A CustomFVS rendszer elutasította a tekercset:", HU_LANGCODE)]
                public static class Refuse { }
            }
        }
        public static class DAL
        {
            public static class Reel
            {
                public static class DisplayName
                {
                    [InitializeTranslation("Ellenőrzést végző felhasználó", HU_LANGCODE)]
                    public static class UserName { }
                    [InitializeTranslation("Munkahely azonosító", HU_LANGCODE)]
                    public static class WorkStationId { }
                    [InitializeTranslation("Munkafolyamat azonosító", HU_LANGCODE)]
                    public static class ProcessId { }
                    [InitializeTranslation("Azonosító kamera IP/PORT", HU_LANGCODE)]
                    public static class IdCamera { }
                    [InitializeTranslation("Ellenőrző kamera IP/PORT", HU_LANGCODE)]
                    public static class CheckCamera { }
                    [InitializeTranslation("Nyomtató IP/PORT", HU_LANGCODE)]
                    public static class Printer { }
                    [InitializeTranslation("Folyamat kezdete", HU_LANGCODE)]
                    public static class StartTimeStamp { }
                    [InitializeTranslation("Folyamat vége", HU_LANGCODE)]
                    public static class EndTimeStamp { }
                    [InitializeTranslation("Kombinált eredmény", HU_LANGCODE)]
                    public static class Result { }
                    [InitializeTranslation("Azonosítás eredménye", HU_LANGCODE)]
                    public static class IdentificationResult { }
                    [InitializeTranslation("Háttér rendszer válasza", HU_LANGCODE)]
                    public static class BackgroundSystemResult { }
                    [InitializeTranslation("Nyomtatott címke ellenőrzésének eredménye", HU_LANGCODE)]
                    public static class CheckResult { }
                    [InitializeTranslation("Azonosítási kisérletek száma", HU_LANGCODE)]
                    public static class IdentificationReadAttempts { }
                    [InitializeTranslation("Kéziolvasások száma", HU_LANGCODE)]
                    public static class IdentificationHandheldReadAttempts { }
                    [InitializeTranslation("Cimkeellenőrzési olvasások száma", HU_LANGCODE)]
                    public static class CheckReadAttempts { }
                    [InitializeTranslation("Cimkeellenőrzési kézi olvasások száma", HU_LANGCODE)]
                    public static class CheckHandheldReadAttempts { }
                    [InitializeTranslation("Nyomtatási kisérletek száma", HU_LANGCODE)]
                    public static class PrintAttempts { }
                    [InitializeTranslation("Azonosításkor olvasott adatok", HU_LANGCODE)]
                    public static class IdentificationReadsData { }
                    [InitializeTranslation("Ellenőrzéskor olvasott adatok", HU_LANGCODE)]
                    public static class CheckReadsData { }
                    [InitializeTranslation("Háttérrendszernek küldött adatok", HU_LANGCODE)]
                    public static class SendToBackgroundSystem { }
                    [InitializeTranslation("Háttérrendszer válasza", HU_LANGCODE)]
                    public static class BackgroundSystemResponse { }
                    [InitializeTranslation("Felismert cimkeazonosító", HU_LANGCODE)]
                    public static class LabelId { }
                    [InitializeTranslation("Felismert beszállító azonosító", HU_LANGCODE)]
                    public static class Vendor { }
                    [InitializeTranslation("Felismert beszállítói cikkszám", HU_LANGCODE)]
                    public static class SupplierPartNumber { }
                    [InitializeTranslation("Felismert beszállítói LOT", HU_LANGCODE)]
                    public static class SupplierLot { }
                    [InitializeTranslation("Felismert beszállítói FVS", HU_LANGCODE)]
                    public static class SupplierFVS { }
                    [InitializeTranslation("Beszállítói egyedi tekercs azonosító", HU_LANGCODE)]
                    public static class SupplierReelSerialNumber { }
                    [InitializeTranslation("Beszállítói mennyiség", HU_LANGCODE)]
                    public static class SupplierQty { }
                    [InitializeTranslation("Belső MTS azonosító", HU_LANGCODE)]
                    public static class InternalMTS { }
                    [InitializeTranslation("Belső FVS azonosító", HU_LANGCODE)]
                    public static class InternalFVS { }
                    [InitializeTranslation("Belső mennyiség", HU_LANGCODE)]
                    public static class InternalQty { }
                    [InitializeTranslation("Belső LOT szám", HU_LANGCODE)]
                    public static class InternalLot { }
                    [InitializeTranslation("Belső cikkszám", HU_LANGCODE)]
                    public static class InternalPartNumber { }
                    [InitializeTranslation("Beszállító azonosítója", HU_LANGCODE)]
                    public static class InternalVendor { }
                }
            }
        }
    }
}
