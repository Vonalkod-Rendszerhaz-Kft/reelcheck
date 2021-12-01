using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class IncomingMessagesElement
        {
            /// <summary>
            /// IncomingMessages XML element név
            /// </summary>
            internal const string NAME = "IncomingMessages";

            /// <summary>
            /// IncomingMessage XML element leírása
            /// </summary>
            internal static class IncomingMessageElement
            {
                /// <summary>
                /// IncomingMessage XML element neve
                /// </summary>
                internal const string NAME = "IncomingMessage";
                /// <summary>
                /// A IncomingMessage XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string ID = "Id";
                }

                /// <summary>
                /// MessageMask XML element leírása
                /// </summary>
                internal static class MessageMaskElement
                {
                    /// <summary>
                    /// MessageMask XML element neve
                    /// </summary>
                    internal const string NAME = "MessageMask";
                }

                /// <summary>
                /// Parts XML element leírása
                /// </summary>
                internal static class PartsElement
                {
                    /// <summary>
                    /// Parts XML element neve
                    /// </summary>
                    internal const string NAME = "Parts";

                    /// <summary>
                    /// Part XML element leírása
                    /// </summary>
                    internal static class PartElement
                    {
                        /// <summary>
                        /// Part XML element neve
                        /// </summary>
                        internal const string NAME = "Part";
                        internal static class Attributes
                        {
                            internal const string NAME = "Name";
                        }
                    }
                }

                /// <summary>
                /// DataElements XML element leírása
                /// </summary>
                internal static class DataElementsElement
                {
                    /// <summary>
                    /// DataElements XML element neve
                    /// </summary>
                    internal const string NAME = "DataElements";

                    /// <summary>
                    /// DataElement XML element leírása
                    /// </summary>
                    internal static class DataElementElement
                    {
                        /// <summary>
                        /// DataElement XML element neve
                        /// </summary>
                        internal const string NAME = "DataElement";
                        /// <summary>
                        /// A DataElement XML element lehetséges attributumai
                        /// </summary>
                        internal static class Attributes
                        {
                            internal const string NAME = "Name";
                        }
                    }
                }
            }
        }

        public List<LabelMessage> IncomingMessagesAddForLabelMessages(List<LabelMessage> labelMessages)
        {
            //LabelMessages feltöltése
            foreach (var incomingMessageElement in
                            GetAllXElements(IncomingMessagesElement.NAME, IncomingMessagesElement.IncomingMessageElement.NAME))
            {
                XElement messageMaskElement = incomingMessageElement.Element(IncomingMessagesElement.IncomingMessageElement.MessageMaskElement.NAME);
                List<string> labels = new List<string>();
                //Add one Label
                string id = GetAttribute(incomingMessageElement, IncomingMessagesElement.IncomingMessageElement.Attributes.ID, String.Empty);
                labels.Add(id);
                LabelMessage labelMessage = new LabelMessage(0,
                                                             id,
                                                             id,
                                                             "|",
                                                             GetElementValue(messageMaskElement, string.Empty),
                                                             labels);
                labelMessages.Add(labelMessage);
            }
            return labelMessages;
        }

        public List<Label> IncomingMessagesAddForLabels(List<Label> labels)
        {
            //Labels feltöltése
            foreach (var incomingMessageElement in
                            GetAllXElements(IncomingMessagesElement.NAME, IncomingMessagesElement.IncomingMessageElement.NAME))
            {
                string id = GetAttribute(incomingMessageElement, IncomingMessagesElement.IncomingMessageElement.Attributes.ID, String.Empty);
                List<Label.Barcode> barcodes = new List<Label.Barcode>();
                //Add Barcodes
                var partsElement = incomingMessageElement.Element(IncomingMessagesElement.IncomingMessageElement.PartsElement.NAME);
                if (partsElement != null)
                {
                    foreach (var partElement in partsElement.Elements(IncomingMessagesElement.IncomingMessageElement.PartsElement.PartElement.NAME))
                    {
                        Label.Barcode barcode = new Label.Barcode(GetAttribute(partElement, IncomingMessagesElement.IncomingMessageElement.PartsElement.PartElement.Attributes.NAME, String.Empty),
                                                                  GetElementValue(partElement, string.Empty));
                        barcodes.Add(barcode);
                    }
                }
                List<Label.DataElement> dataElements = new List<Label.DataElement>();
                //Add DataElements
                var dataElementsElement = incomingMessageElement.Element(IncomingMessagesElement.IncomingMessageElement.DataElementsElement.NAME);
                if (dataElementsElement != null)
                {
                    foreach (var dataElementElement in dataElementsElement.Elements(IncomingMessagesElement.IncomingMessageElement.DataElementsElement.DataElementElement.NAME))
                    {
                        Label.DataElement dataElement = new Label.DataElement(GetAttribute(dataElementElement, IncomingMessagesElement.IncomingMessageElement.DataElementsElement.DataElementElement.Attributes.NAME, String.Empty),
                                                                  GetElementValue(dataElementElement, string.Empty));
                        dataElements.Add(dataElement);
                    }
                }
                Label label = new Label(id, barcodes, dataElements);
                labels.Add(label);
            }
            return labels;
        }
    }
}
