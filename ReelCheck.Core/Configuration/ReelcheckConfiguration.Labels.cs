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
        internal static class LabelsElement
        {
            internal const string NAME = "Labels";

            /// <summary>
            /// Label XML element leírása
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

                /// <summary>
                /// Barcodes XML element leírása
                /// </summary>
                internal static class BarcodesElement
                {
                    /// <summary>
                    /// Barcodes XML element neve
                    /// </summary>
                    internal const string NAME = "Barcodes";

                    /// <summary>
                    /// Barcode XML element leírása
                    /// </summary>
                    internal static class BarcodeElement
                    {
                        /// <summary>
                        /// Barcode XML element neve
                        /// </summary>
                        internal const string NAME = "Barcode";
                        /// <summary>
                        /// A Barcode XML element lehetséges attributumai
                        /// </summary>
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

        public List<Label> Labels
        {
            get
            {
                List<Label> result = new List<Label>();
                //Labels feltöltése

                foreach (var labelElement in
                                GetAllXElements(LabelsElement.NAME, LabelsElement.LabelElement.NAME))
                {
                    List<Label.Barcode> barcodes = new List<Label.Barcode>();

                    //Add Barcodes
                    foreach (var barcodeElement in
                                labelElement.Element(LabelsElement.LabelElement.BarcodesElement.NAME).Elements(LabelsElement.LabelElement.BarcodesElement.BarcodeElement.NAME))
                    {
                        Label.Barcode barcode = new Label.Barcode(GetAttribute(barcodeElement, LabelsElement.LabelElement.BarcodesElement.BarcodeElement.Attributes.NAME, String.Empty),
                                                                  GetElementValue(barcodeElement, string.Empty));
                        barcodes.Add(barcode);
                    }

                    List<Label.DataElement> dataElements = new List<Label.DataElement>();

                    //Add DataElements
                    foreach (var dataElementElement in
                                labelElement.Element(LabelsElement.LabelElement.DataElementsElement.NAME).Elements(LabelsElement.LabelElement.DataElementsElement.DataElementElement.NAME))
                    {
                        Label.DataElement dataElement = new Label.DataElement(GetAttribute(dataElementElement, LabelsElement.LabelElement.DataElementsElement.DataElementElement.Attributes.NAME, String.Empty),
                                                                  GetElementValue(dataElementElement, string.Empty));
                        dataElements.Add(dataElement);
                    }

                    Label label = new Label(GetAttribute(labelElement, LabelsElement.LabelElement.Attributes.ID, String.Empty),
                                                                 barcodes,
                                                                 dataElements);

                    result.Add(label);
                }

                IncomingMessagesAddForLabels(result);

                return result;
            }
        }

        public class Label
        {
            string _id;
            List<Barcode> _barcodes; 
            List<DataElement> _dataElements;

            public Label(string id, List<Barcode> barcodes, List<DataElement> dataElements)
            {
                _id = id;
                _barcodes = barcodes;
                _dataElements = dataElements;
            }

            public string Id
            {
                get
                {
                    return _id;
                }
            }

            public List<Barcode> Barcodes
            {
                get
                {
                    return _barcodes;
                }
            }

            public class Barcode
            {
                string _name;
                string _value;

                public Barcode(string name, string value)
                {
                    _name = name;
                    _value = value;
                }

                public string Name
                {
                    get
                    {
                        return _name;
                    }
                }

                public string Value
                {
                    get
                    {
                        return _value;
                    }
                }
            }

            public List<DataElement> DataElements
            {
                get
                {
                    return _dataElements;
                }
            }

            public class DataElement
            {
                string _name;
                string _value;

                public DataElement(string name, string value)
                {
                    _name = name;
                    _value = value;
                }

                public string Name
                {
                    get
                    {
                        return _name;
                    }
                }

                public string Value
                {
                    get
                    {
                        return _value;
                    }
                }
            }
        }

    }
}
