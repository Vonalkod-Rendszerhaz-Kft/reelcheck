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
        internal static class LabelMessagesElement
        {
            internal const string NAME = "LabelMessages";

            /// <summary>
            /// LabelMessage XML element leírása
            /// </summary>
            internal static class LabelMessageElement
            {
                /// <summary>
                /// LabelMessage XML element neve
                /// </summary>
                internal const string NAME = "LabelMessage";
                /// <summary>
                /// A LabelMessage XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string SOURCE = "Source";
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
                    /// <summary>
                    /// A MessageMask XML element lehetséges attributumai
                    /// </summary>
                    internal static class Attributes
                    {
                        internal const string LISTNAME = "ListName";
                        internal const string LISTSEPARATOR = "ListSeparator";
                    }
                }

                /// <summary>
                /// Labels XML element leírása
                /// </summary>
                internal static class LabelsElement
                {
                    /// <summary>
                    /// Labels XML element neve
                    /// </summary>
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
                            internal const string ID = "LabelId";
                        }
                    }
                }
            }
        }

        public List<LabelMessage> LabelMessages
        {
            get
            {
                List<LabelMessage> result = new List<LabelMessage>();
                //LabelMessages feltöltése

                foreach (var labelMessageElement in
                                GetAllXElements(LabelMessagesElement.NAME, LabelMessagesElement.LabelMessageElement.NAME))
                {
                    XElement messageMaskElement = labelMessageElement.Element(LabelMessagesElement.LabelMessageElement.MessageMaskElement.NAME);
                   
                    List<string> labels = new List<string>();
                    //Add Labels
                    var labelsXElement = labelMessageElement.Element(LabelMessagesElement.LabelMessageElement.LabelsElement.NAME);
                    if (labelsXElement != null)
                    {
                        foreach (var labelElement in
                                    labelsXElement.Elements(LabelMessagesElement.LabelMessageElement.LabelsElement.LabelElement.NAME))
                        {

                            labels.Add(GetAttribute(labelElement, LabelMessagesElement.LabelMessageElement.LabelsElement.LabelElement.Attributes.ID, String.Empty));
                        }
                    }

                    string cfString = GetAttribute(labelMessageElement, LabelMessagesElement.LabelMessageElement.Attributes.SOURCE, String.Empty);
                    CameraType cameraFunctions = (CameraType)Enum.Parse(typeof(CameraType), cfString, true);
                    LabelMessage labelMessage = new LabelMessage(cameraFunctions,
                                                                 GetAttribute(labelMessageElement,                      LabelMessagesElement.LabelMessageElement.Attributes.ID, String.Empty),
                                                                 GetAttribute(messageMaskElement, LabelMessagesElement.LabelMessageElement.MessageMaskElement.Attributes.LISTNAME, String.Empty),
                                                                 GetAttribute(messageMaskElement, LabelMessagesElement.LabelMessageElement.MessageMaskElement.Attributes.LISTSEPARATOR, String.Empty),
                                                                 GetElementValue(messageMaskElement, string.Empty),
                                                                 labels);

                    result.Add(labelMessage);
                }

                IncomingMessagesAddForLabelMessages(result);

                return result;
            }
        }

        public class LabelMessage
        {
            CameraType _source;
            string _id;
            string _listName;
            string _listSeparator;
            string _messageMask;
            List<string> _labels;

            public LabelMessage(CameraType cameraFunctions, string id, string listName, string listSeparator, string messageMask, List<string> labels)
            {
                _source = cameraFunctions;
                _id = id;
                _listName = listName;
                _listSeparator = listSeparator;
                _messageMask = messageMask;
                _labels = labels;
            }

            public CameraType Source
            {
                get
                {
                    return _source;
                }
            }

            public string Id
            {
                get
                {
                    return _id;
                }
            }

            public string ListName
            {
                get
                {
                    return _listName;
                }
            }

            public string ListSeparator
            {
                get
                {
                    return _listSeparator;
                }
            }

            public string MessageMask
            {
                get
                {
                    return _messageMask;
                }
            }

            public List<string> Labels
            {
                get
                {
                    return _labels;
                }
            }
        }

    }
}
