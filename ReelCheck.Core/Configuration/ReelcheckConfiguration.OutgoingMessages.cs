using System;
using System.Configuration;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {

        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class OutgoingMessagesElement
        {
            internal const string NAME = "OutgoingMessages";

            /// <summary>
            /// OutgoingMessage XML element leírása
            /// </summary>
            internal static class OutgoingMessageElement
            {
                /// <summary>
                /// OutgoingMessage XML element neve
                /// </summary>
                internal const string NAME = "OutgoingMessage";
                /// <summary>
                /// A OutgoingMessage XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string ID = "Id";
                    internal const string NAMESEPARATOR = "NameSeparator";
                }
            }
        }

        public class OutgoingMessage
        {
            string _nameSeparator;
            string _value;

            public OutgoingMessage(string nameSeparator, string value)
            {
                _nameSeparator = nameSeparator;
                _value = value;
            }

            public string NameSeparator
            {
                get
                {
                    return _nameSeparator;
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

        public OutgoingMessage GetOutgoingMessage(string id)
        {
            OutgoingMessage result = null;

            foreach (var outgoingMessageElement in
                                            GetAllXElements(OutgoingMessagesElement.NAME, OutgoingMessagesElement.OutgoingMessageElement.NAME))
            {
                if (GetAttribute(outgoingMessageElement, OutgoingMessagesElement.OutgoingMessageElement.Attributes.ID, String.Empty) ==
                    id)
                {
                    result = new OutgoingMessage(GetAttribute(outgoingMessageElement,
                        OutgoingMessagesElement.OutgoingMessageElement.Attributes.NAMESEPARATOR, String.Empty), 
                                                GetElementValue(outgoingMessageElement, string.Empty));
                    break;
                }
            }

            return result;
        }    
    }
}
