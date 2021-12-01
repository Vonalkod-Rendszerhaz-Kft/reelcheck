using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReelCheck.Web.Areas.MasterData
{
    public static class DataTablesConstants
    {
        /// <summary>
        /// növekvő rendezés
        /// </summary>
        public const string ORDER_ASC = "asc";

        /// <summary>
        /// csökkenő rendezés
        /// </summary>
        public const string ORDER_DESC = "desc";
    }

    #region DataTablesIn class (Json class sent from dataTables)
    /// <summary>
    /// Egy osztály, amely a dataTables plugin által küldött
    /// paramétereket tartalmazza.
    /// </summary>
    public class DataTablesIn
    {
        /// <summary>
        /// Rajz számláló. 
        /// Annak biztosítására, hogy az ajax aszinkronitása ellenére is
        /// a kirajzolás megflelő sorrendben történjen.
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// A kirajzolt első rekord indexe.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// A rekordok száma, amit a tábla megjelenít.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Az oszlopok listája.
        /// </summary>
        public List<DTColumn> Columns { get; set; }

        /// <summary>
        /// A teljes táblázatra érvényes kereső objektum.
        /// </summary>
        public DTSearch Search { get; set; }

        /// <summary>
        /// A rendezettség listája. (Ha több oszlopot is tartalmaz a rendezés.)
        /// </summary>
        public List<DTOrder> Order { get; set; }

        #region DTColumn class
        /// <summary>
        /// Egy oszlopot leíró osztály.
        /// </summary>
        public class DTColumn
        {
            /// <summary>
            /// Az oszlop tartalma.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Az oszlop neve (azonosítója).
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Az oszlop kereshető-e.
            /// </summary>
            public bool Searchable { get; set; }

            /// <summary>
            /// Az oszlop rendezése engedélyezve van-e.
            /// </summary>
            public bool Orderable { get; set; }

            /// <summary>
            /// Az oszlopra vonatkozó kereső objektum.
            /// </summary>
            public DTSearch Search { get; set; }
        }
        #endregion DTColumn class

        #region DTSearch class
        /// <summary>
        /// Egy keresést leíró osztály.
        /// </summary>
        public class DTSearch
        {
            /// <summary>
            /// Egy string kereső érték.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Igaz, ha a kereső érték egy reguláris kifejezés.
            /// </summary>
            public bool Regex { get; set; }
        }
        #endregion DTSearch class

        #region DTOrder class
        /// <summary>
        /// Egy rendezést leíró osztály.
        /// </summary>
        public class DTOrder
        {
            /// <summary>
            /// Melyik indexű oszlop tartozik a rendezésbe. 
            /// </summary>
            public int Column { get; set; }

            /// <summary>
            /// A rendezettség iránya. 
            /// </summary>
            public string Dir { get; set; }
        }
        #endregion DTOrder class
    }
    #endregion DataTablesIn class (Json class sent from dataTables)
}