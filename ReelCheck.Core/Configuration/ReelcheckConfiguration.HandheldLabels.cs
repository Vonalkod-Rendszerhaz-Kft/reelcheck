using System;
using System.Collections.Generic;
using System.Configuration;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {

        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class HandheldLabelsElement
        {
            internal const string NAME = "HandheldLabels";

            /// <summary>
            /// HandheldLabel XML element leírása
            /// </summary>
            internal static class HandheldLabelElement
            {
                /// <summary>
                /// HandheldLabel XML element neve
                /// </summary>
                internal const string NAME = "HandheldLabel";
                /// <summary>
                /// A HandheldLabel XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string SOURCE = "Source";
                }

                /// <summary>
                /// HandheldLabel XML element leírása
                /// </summary>
                internal static class LabelElement
                {
                    /// <summary>
                    /// Label XML element neve
                    /// </summary>
                    internal const string NAME = "Label";
                    /// <summary>
                    /// A Label XML element lehetséges attributumai
                    /// </summary>
                    internal static class Attributes
                    {
                        internal const string ID = "Id";
                    }
                }
            }
        }

        public List<string> GetHandheldLabel(CameraType source)
        {
            List<string> result = new List<string>();

            foreach (var handheldLabelElement in
                                            GetAllXElements(HandheldLabelsElement.NAME, HandheldLabelsElement.HandheldLabelElement.NAME))
            {
                if (GetAttribute(handheldLabelElement, HandheldLabelsElement.HandheldLabelElement.Attributes.SOURCE, String.Empty) ==
                    source.ToString().ToUpper())
                {
                    result.Add(GetAttribute(handheldLabelElement.Element(HandheldLabelsElement.HandheldLabelElement.LabelElement.NAME), HandheldLabelsElement.HandheldLabelElement.LabelElement.Attributes.ID, string.Empty));
                }
            }

            return result;
        }    
    }
}
