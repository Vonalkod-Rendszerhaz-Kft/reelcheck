using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Core.DAL
{
    /// <summary>
    /// Egy ellenőrzést reprezentál
    /// </summary>
    public class Reel
    {
        /// <summary>
        /// PK, autoincrement Id
        /// </summary>
        [Key]
        [Required]
        [Editable(false)] 
        public int Id { get; set; }

        /// <summary>
        /// Az ellenőrzést végző felhasználó
        /// </summary>
        [MaxLength(50)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.UserName))]
        [Column("USERNAME")]
        public string UserName { get; set; }

        /// <summary>
        /// Munkahely azonosító
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.WorkStationId))]
        [Column("STATION_ID")]
        public string WorkstationId { get; set; }

        /// <summary>
        /// Folyamat azonosító
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.ProcessId))]
        [Column("STATION_PROCESS")]
        public string ProcessId { get; set; }

        /// <summary>
        /// ID kamera 
        /// </summary>
        [MaxLength(25)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.IdCamera))]
        [Column("IPPORT_ID")]
        public string IdCamera { get; set; }

        /// <summary>
        /// CHECK kamera
        /// </summary>
        [MaxLength(25)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.CheckCamera))]
        [Column("IPPORT_CHECK")]
        public string CheckCamera { get; set; }

        /// <summary>
        /// Nyomtató
        /// </summary>
        [MaxLength(25)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.Printer))]
        [Column("IPPORT_PRN")]
        public string Printer { get; set; }

        /// <summary>
        /// Ellenőrzési folymat indításának időbélyege
        /// </summary>
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.StartTimeStamp))]
        [Column("TIMESTAMP_START")]
        public DateTime StartTimeStamp { get; set; }

        /// <summary>
        /// Ellenőrzési folymat befelyezésének időbélyege
        /// </summary>
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.EndTimeStamp))]
        [Column("TIMESTAMP_END")]
        public DateTime? EndTimeStamp { get; set; }

        /// <summary>
        /// Az ellenőrzés eredménye
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.Result))]
        [Column("RESULT")]
        public string Result { get; set; }

        /// <summary>
        /// Azonosítási folyamat eredménye
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.IdentificationResult))]
        [Column("RESULT_ID")]
        public string IdentificationResult { get; set; }

        /// <summary>
        /// Háttérrendszer válasza
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.BackgroundSystemResult))]
        [Column("RESULT_IDCONF")]
        public string BackgroundSystemResult { get; set; }

        /// <summary>
        /// Kinyomtatott címke ellenőrzésének eredménye
        /// </summary>
        [MaxLength(10)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.CheckResult))]
        [Column("RESULT_CHECK")]
        public string CheckResult { get; set; }

        /// <summary>
        /// Azonosítási olvasások száma
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.IdentificationReadAttempts))]
        [Column("ATTEMPTS_READID")]
        public int IdentificationReadAttempts { get; set; }

        /// <summary>
        /// Azonosításban a kézi olvasásának száma
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.IdentificationHandheldReadAttempts))]
        [Column("ATTEMPTS_ READIDHH")]
        public int IdentificationHandheldReadAttempts { get; set; }

        /// <summary>
        /// Címke visszaellnőrzési olvasások száma
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.CheckReadAttempts))]
        [Column("ATTEMPTS_READCHECK")]
        public int CheckReadAttempts { get; set; }

        /// <summary>
        /// Kinyomtatott cimke ellenőrzése során a kézi olvasások száma
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.CheckHandheldReadAttempts))]
        [Column("ATTEMPTS_READCHECKHH")]
        public int CheckHandheldReadAttempts { get; set; }

        /// <summary>
        /// Nyomtatásai kisérletek száma
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.PrintAttempts))]
        [Column("ATTEMPTS_PRINT")]
        public int PrintAttempts { get; set; }

        /// <summary>
        /// ID kameraáltal olvasott adatok
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.IdentificationReadsData))]
        [Column("READERDATA_ID")]
        public string IdentificationReadsData { get; set; }

        /// <summary>
        /// Check kamera általolvasott adatok
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.CheckReadsData))]
        [Column("READERDATA_CHECK")]
        public string CheckReadsData { get; set; }

        /// <summary>
        /// A háttérrendszernek küldött adat
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SendToBackgroundSystem))]
        [Column("MSGDATA_IDDATA")]
        public string SendToBackgroundSystem { get; set; }

        /// <summary>
        /// A háttérrendszer válasza
        /// </summary>
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.BackgroundSystemResponse))]
        [Column("MSGDATA_IDDATAACK")]
        public string BackgroundSystemResponse { get; set; }

        /// <summary>
        /// Az azonosított címke azonosítója
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.LabelId))]
        [Column("DE_LABELID")]
        public string LabelId { get; set; }

        /// <summary>
        /// Beszállító kód
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.Vendor))]
        [Column("DE_SVENDOR")]
        public string Vendor { get; set; }

        /// <summary>
        /// Beszállítói cikkszám
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SupplierPartNumber))]
        [Column("DE_SPN")]
        public string SupplierPartNumber { get; set; }

        /// <summary>
        /// Beszállítói Lot
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SupplierLot))]
        [Column("DE_SLOT")]
        public string SupplierLot { get; set; }

        /// <summary>
        /// Beszállítói FVS
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SupplierFVS))]
        [Column("DE_SFVS")]
        public string SupplierFVS { get; set; }

        /// <summary>
        /// Beszállítói SN
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SupplierReelSerialNumber))]
        [Column("DE_SRSN")]
        public string SupplierReelSerialNumber { get; set; }

        /// <summary>
        /// BEszállítói mennyiség
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.SupplierQty))]
        [Column("DE_SQTY")]
        public string SupplierQty { get; set; }

        /// <summary>
        /// Belső MTS
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalMTS))]
        [Column("DE_IMTSID")]
        public string InternalMTS { get; set; }

        /// <summary>
        /// Belső FVS
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalFVS))]
        [Column("DE_IFVS")]
        public string InternalFVS { get; set; }

        /// <summary>
        /// Belső mennyiség
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalQty))]
        [Column("DE_IQTY")]
        public string InternalQty { get; set; }

        /// <summary>
        /// Belső LOT
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalLot))]
        [Column("DE_ILOT")]
        public string InternalLot { get; set; }

        /// <summary>
        /// Belső cikkszám
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalPartNumber))]
        [Column("DE_IPN")]
        public string InternalPartNumber { get; set; }

        /// <summary>
        /// Beszállító belső kódja
        /// </summary>
        [MaxLength(100)]
        [Index(IsClustered = false, IsUnique = false)]
        [DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.DAL.Reel.DisplayName.InternalVendor))]
        [Column("DE_IVENDOR")]
        public string InternalVendor { get; set; }        
    }
}
