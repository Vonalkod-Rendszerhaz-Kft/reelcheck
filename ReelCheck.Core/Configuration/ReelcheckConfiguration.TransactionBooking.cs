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
        internal static class TransactionBookingElement
        {
            internal const string NAME = "TransactionBooking";
            /// <summary>
            /// A TransactionBooking XML element lehetséges attributumai
            /// </summary>
            internal static class Attributes
            {
                internal const string ENABLED = "Enabled";
            }

            /// <summary>
            /// BookingRecord XML element leírása
            /// </summary>
            internal static class BookingRecordElement
            {
                /// <summary>
                /// BookingRecord XML element neve
                /// </summary>
                internal const string NAME = "BookingRecord";
                /// <summary>
                /// A BookingRecord XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string NAME = "Name";
                    internal const string ENABLED = "Enabled";
                }
            }
        }

        public TransactionBookingType TransactionBooking
        {
            get
            {
                TransactionBookingType result;

                //BookingRecords feltöltése

                List<TransactionBookingType.BookingRecord> bookingRecords = new List<TransactionBookingType.BookingRecord>();

                foreach (var bookingRecordElement in
                                GetAllXElements(TransactionBookingElement.NAME, TransactionBookingElement.BookingRecordElement.NAME))
                {
                    TransactionBookingType.BookingRecord bookingRecord = new TransactionBookingType.BookingRecord(GetAttribute(bookingRecordElement, TransactionBookingElement.BookingRecordElement.Attributes.NAME, string.Empty),
                        GetAttribute(bookingRecordElement, TransactionBookingElement.BookingRecordElement.Attributes.ENABLED, false));
                    bookingRecords.Add(bookingRecord);
                }
                
                result = new TransactionBookingType(GetAttribute(GetXElement(TransactionBookingElement.NAME), TransactionBookingElement.Attributes.ENABLED, false),
                    bookingRecords);

                return result;
            }
        }

        public class TransactionBookingType
        {
            bool _enabled;
            List<BookingRecord> _bookingRecords;

            public TransactionBookingType(bool enabled, List<BookingRecord> bookingRecords)
            {
                _enabled = enabled;
                _bookingRecords = bookingRecords;
            }

            public bool Enabled
            {
                get
                {
                    return _enabled;
                }
            }

            public List<BookingRecord> BookingRecords
            {
                get
                {
                    return _bookingRecords;
                }
            }

            public class BookingRecord
            {
                string _name;
                bool _enabled;

                public BookingRecord(string name, bool enabled)
                {
                    _name = name;
                    _enabled = enabled;
                }

                public string Name
                {
                    get
                    {
                        return _name;
                    }
                }

                public bool Enabled
                {
                    get
                    {
                        return _enabled;
                    }
                }
            }
        }

    }
}
