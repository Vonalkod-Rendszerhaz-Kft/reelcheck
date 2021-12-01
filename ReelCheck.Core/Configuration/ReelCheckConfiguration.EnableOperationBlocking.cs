using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// Manuális blokkolás engedélyezve van-e?
        /// </summary>
        public bool ManualOperationBlockingEnabled
        {
            get
            {
                return GetElementValue<bool>(
                    GetXElement(EnableOperationBlockingElement.NAME, EnableOperationBlockingElement.ManualElement.NAME), false);
            }
        }

        /// <summary>
        /// Blokkolás engedélyezve, ha elbukik a goldensample teszt
        /// </summary>
        public bool GoldenSampleTestFailedOperationBlockingEnabled
        {
            get
            {
                return GetElementValue<bool>(
                    GetXElement(EnableOperationBlockingElement.NAME, EnableOperationBlockingElement.GoldenSampleTestFailed.NAME), false);
            }
        }

        /// <summary>
        /// Blokkolás engedélyezve, ha az aranyminta teszt esedékessé válik
        /// </summary>
        public bool GoldenSampleTestDueOperationBlockingEnabled
        {
            get
            {
                return GetElementValue<bool>(
                    GetXElement(EnableOperationBlockingElement.NAME, EnableOperationBlockingElement.GoldenSampleTestDue.NAME), false);
            }
        }

        /// <summary>
        /// Blokkolás engedélyezve, ha hibás tekercs forog ki
        /// </summary>
        public bool ReelProcessingFailedOperationBlockingEnabled
        {
            get
            {
                return GetElementValue<bool>(
                    GetXElement(EnableOperationBlockingElement.NAME, EnableOperationBlockingElement.ReelProcessingFailed.NAME), false);
            }
        }
        
        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása (EnableOperationBlocking element)
        /// </summary>
        internal static class EnableOperationBlockingElement
        {
            internal const string NAME = "EnableOperationBlocking";

            /// <summary>
            /// Manual XML element leírása
            /// </summary>
            internal static class ManualElement
            {
                /// <summary>
                /// Manual XML element neve
                /// </summary>
                internal const string NAME = "Manual";
            }

            /// <summary>
            /// GoldenSampleTestFailed XML element leírása
            /// </summary>
            internal static class GoldenSampleTestFailed
            {
                /// <summary>
                /// GoldenSampleTestFailed XML element neve
                /// </summary>
                internal const string NAME = "GoldenSampleTestFailed";
            }

            /// <summary>
            /// GoldenSampleTestDue XML element leírása
            /// </summary>
            internal static class GoldenSampleTestDue
            {
                /// <summary>
                /// GoldenSampleTestDue XML element neve
                /// </summary>
                internal const string NAME = "GoldenSampleTestDue";
            }

            /// <summary>
            /// ReelProcessingFailed XML element leírása
            /// </summary>
            internal static class ReelProcessingFailed
            {
                /// <summary>
                /// ReelProcessingFailed XML element neve
                /// </summary>
                internal const string NAME = "ReelProcessingFailed";
            }
        }
    }
}
