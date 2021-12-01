namespace ReelCheck.EventHub.Contracts
{
    /// <summary>
    /// ReelCheck beavatkozás EventHub szerződés
    /// </summary>
    public class InterventionContract
    {
        /// <summary>
        /// Gép aktiválása
        /// </summary>
        public class Activate
        {
            /// <summary>
            /// Gépre bejelentkezett felhasználó
            /// </summary>
            public string UserName { get; set; }
        }

        /// <summary>
        /// Gép inaktiválása
        /// </summary>
        public class InActivate
        {
            /// <summary>
            /// Gépre bejelentkezett felhasználó, aki kijelentkezik
            /// </summary>
            public string UserName { get; set; }
        }

        /// <summary>
        /// Azonosítás ismétlése
        /// </summary>
        public class ReIdentify
        {

        }

        /// <summary>
        /// Újra nyomtatás
        /// </summary>
        public class RePrint
        {
        }

        /// <summary>
        /// Nyomtatási művelet figyelmenkívülhagyása
        /// </summary>
        public class SkipPrint
        {
        }

        /// <summary>
        /// Újra ellenőrzés
        /// </summary>
        public class ReCheck
        {
        }

        /// <summary>
        /// Alaphelyzetbe állítás
        /// </summary>
        public class UnloadMode
        {
            /// <summary>
            /// Belépés
            /// </summary>
            public class Enter
            {

            }

            /// <summary>
            /// Kilépés
            /// </summary>
            public class Exit
            {

            }
        }

        /// <summary>
        /// Aranyminta teszt
        /// </summary>
        public class GoldenSampleMode
        {
            /// <summary>
            /// Belépés aramnyminta tesztbe
            /// </summary>
            public class Enter
            {
            }

            /// <summary>
            /// Kilépés aramnyminta tesztből
            /// </summary>
            public class Exit
            {
            }

            /// <summary>
            /// Figyelmeztetés törlés (aramnyminta tesz esedékességére)
            /// </summary>
            public class DeleteDue
            {
            }

            /// <summary>
            /// Aranyminta teszt végzése azonnal legyen esedékes
            /// </summary>
            public class SetDue
            {
            }
        }

        /// <summary>
        /// Kézi leolvasás bevitele
        /// </summary>
        public static class ManualMode
        {
            /// <summary>
            /// Manuális adat megadás
            /// </summary>
            public class On
            {
                /// <summary>
                /// FVS kód
                /// </summary>
                public string FVSCode { get; set; }

                /// <summary>
                /// Cikkszám
                /// </summary>
                public string PartNumber { get; set; }

                /// <summary>
                /// Lotszám
                /// </summary>
                public string LotNumber { get; set; }

                /// <summary>
                /// Mennyiség
                /// </summary>
                public string Qty { get; set; }
            }

            /// <summary>
            /// Manuálsi adat törlése
            /// </summary>
            public class Off
            {
            }
        }

        /// <summary>
        /// Blokkolással kapcsolatos beavatkozások
        /// </summary>
        public class Blocking
        {
            /// <summary>
            /// Blokkolás
            /// </summary>
            public class Block
            {
            }

            /// <summary>
            /// Blokkolás feloldása
            /// </summary>
            public class UnBlock
            {
            }
        }

        /// <summary>
        /// Eredmény
        /// </summary>
        public class Result
        {
        }
    }
}
