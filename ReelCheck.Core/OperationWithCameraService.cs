namespace ReelCheck.Core
{
    /// <summary>
    /// Olyan entitás ős osztálya, amelyik használ valamit a Vrh.CameraService szolgáltatásaiból 
    /// </summary>
    internal class OperationWithCameraService : Operation
    {
        /// <summary>
        /// Konstruktor amivel megosztott EventHub calllockerrel lehet létrehozni
        /// </summary>
        /// <param name="sharedEventHubCallLocker">referencia a közös callLockerre</param>
        public OperationWithCameraService(object sharedEventHubCallLocker)
            : base(sharedEventHubCallLocker)
        {            
        }

        /// <summary>
        /// Az azonosítást végző AutoID kamera logikai neve
        /// </summary>
        protected string _camera;

        /// <summary>
        /// A Camera szolgáltatás EventHub csatornája
        /// </summary>
        protected string _cameraEventHubChannel;
    }
}
