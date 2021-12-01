namespace ReelCheck.Core
{
    /// <summary>
    /// Olyan entitás ősosztálya, amelyik a Vrh.PrintingService-t használja
    /// </summary>
    internal class OperationWithPrintingService : Operation
    {
        /// <summary>
        /// Konstruktor amivel megosztott EventHub calllockerrel lehet létrehozni
        /// </summary>
        /// <param name="sharedEventHubCallLocker">referencia a közös callLockerre</param>
        public OperationWithPrintingService(object sharedEventHubCallLocker)
            : base(sharedEventHubCallLocker)
        {
        }

        /// <summary>
        /// A használt nyomtató
        /// </summary>
        protected Configuration.ReelcheckConfiguration.Printer _printer;

        /// <summary>
        /// A Printer szolgáltatás EventHub csatornája
        /// </summary>
        protected string _printerEventHubChannel;
    }
}
