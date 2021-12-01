using System;
using System.Configuration;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    /// <summary>
    /// Instance level configuration to ReelCheck AC plugin 
    /// </summary>
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// Visszadja a megadott funkciójú kamera logikai nevét 
        /// </summary>
        /// <param name="cameraFunction">Kamera funkciója</param>
        /// <returns></returns>
        public string GetCamera(CameraType cameraFunction)
        {
            string result = string.Empty;

            foreach (var cameraElement in
                 GetAllXElements(CameraDefinitionsElement.NAME,CameraDefinitionsElement.CameraElement.NAME))
            {
                if (GetAttribute(cameraElement, CameraDefinitionsElement.CameraElement.Attributes.FUNCTION, String.Empty) ==
                    cameraFunction.ToString().ToUpper())
                {
                    result = GetAttribute(cameraElement,
                        CameraDefinitionsElement.CameraElement.Attributes.NAME, String.Empty);
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class CameraDefinitionsElement
        {
            internal const string NAME = "CameraDefinitions";

            /// <summary>
            /// Camera XML element leírása
            /// </summary>
            internal static class CameraElement
            {
                /// <summary>
                /// Camera XML element neve
                /// </summary>
                internal const string NAME = "Camera";
                /// <summary>
                /// A Camera XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string FUNCTION = "Function";
                    internal const string NAME = "Name";
                }
            }
        }
    }
}
