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
        internal static class PrinterDefinitionsElement
        {
            internal const string NAME = "PrinterDefinitions";

            /// <summary>
            /// Printer XML element leírása
            /// </summary>
            internal static class PrinterElement
            {
                /// <summary>
                /// Printer XML element neve
                /// </summary>
                internal const string NAME = "Printer";
                /// <summary>
                /// A Printer XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string LOGICALNAME = "LogicalName";
                    internal const string PHISYCALNAME = "PhisycalName";
                    internal const string DEFAULTPRINTEVENTID = "DefaultPrintEventId";
                    internal const string ACTIVE = "Active";
                }

                /// <summary>
                /// PrintEvents XML element leírása
                /// </summary>
                internal static class PrintEventsElement
                {
                    /// <summary>
                    /// PrintEvents XML element neve
                    /// </summary>
                    internal const string NAME = "PrintEvents";

                    /// <summary>
                    /// PrintEvent XML element leírása
                    /// </summary>
                    internal static class PrintEventElement
                    {
                        /// <summary>
                        /// PrintEvent XML element neve
                        /// </summary>
                        internal const string NAME = "PrintEvent";
                        /// <summary>
                        /// A PrintEvent XML element lehetséges attributumai
                        /// </summary>
                        internal static class Attributes
                        {
                            internal const string EVENTID = "EventId";
                            internal const string LABELID = "LabelId";
                            internal const string NAMESEPARATOR = "NameSeparator";
                            internal const string ACTIVE = "Active";
                        }

                        /// <summary>
                        /// LabelVars XML element leírása
                        /// </summary>
                        internal static class LabelVarsElement
                        {
                            /// <summary>
                            /// LabelVars XML element neve
                            /// </summary>
                            internal const string NAME = "LabelVars";

                            /// <summary>
                            /// LabelVar XML element leírása
                            /// </summary>
                            internal static class LabelVarElement
                            {
                                /// <summary>
                                /// LabelVar XML element neve
                                /// </summary>
                                internal const string NAME = "LabelVar";
                                /// <summary>
                                /// A LabelVar XML element lehetséges attributumai
                                /// </summary>
                                internal static class Attributes
                                {
                                    internal const string NAME = "Name";
                                }
                            }
                        }
                    }
                }
            }
        }

        public Printer GetPrinter(string logicalName, string stationId)
        {
            Printer result = null;

            foreach (var printerElement in
                                            GetAllXElements(PrinterDefinitionsElement.NAME, PrinterDefinitionsElement.PrinterElement.NAME))
            {
                if (GetAttribute(printerElement, PrinterDefinitionsElement.PrinterElement.Attributes.LOGICALNAME, String.Empty) ==
                    logicalName && GetAttribute(printerElement, PrinterDefinitionsElement.PrinterElement.Attributes.ACTIVE, true))
                {
                    List<Printer.PrintEvent> printEvents = new List<Printer.PrintEvent>();

                    foreach (var printEventElement in
                                printerElement.Element(PrinterDefinitionsElement.PrinterElement.PrintEventsElement.NAME).Elements(PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.NAME))
                    {
                        List<Printer.PrintEvent.LabelVar> labelVars = new List<Printer.PrintEvent.LabelVar>();

                        foreach (var labelVarElement in
                                printEventElement.Element(PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.LabelVarsElement.NAME).Elements(PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.LabelVarsElement.LabelVarElement.NAME))
                        {
                            Printer.PrintEvent.LabelVar labelVar = new Printer.PrintEvent.LabelVar(GetAttribute(labelVarElement, PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.LabelVarsElement.LabelVarElement.Attributes.NAME, String.Empty), 
                                GetElementValue(labelVarElement, string.Empty));
                            labelVars.Add(labelVar);
                        }

                        Printer.PrintEvent printEvent = new Printer.PrintEvent(GetAttribute(printEventElement, PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.Attributes.EVENTID, String.Empty),
                                        GetAttribute(printEventElement, PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.Attributes.LABELID, String.Empty),
                                        GetAttribute(printEventElement, PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.Attributes.NAMESEPARATOR, String.Empty),
                                        GetAttribute(printEventElement, PrinterDefinitionsElement.PrinterElement.PrintEventsElement.PrintEventElement.Attributes.ACTIVE, true),
                                        labelVars);
                        printEvents.Add(printEvent);
                    }
                    string namePattern = GetAttribute(printerElement, PrinterDefinitionsElement.PrinterElement.Attributes.PHISYCALNAME, String.Empty);
                    string phisicalName = namePattern.Replace("{0}", stationId);
                    result = new Printer(GetAttribute(printerElement, PrinterDefinitionsElement.PrinterElement.Attributes.LOGICALNAME, String.Empty), 
                                        phisicalName, 
                                        GetAttribute(printerElement, PrinterDefinitionsElement.PrinterElement.Attributes.DEFAULTPRINTEVENTID, String.Empty),
                                        printEvents);
                    break;
                }
            }

            return result;
        }

        public class Printer
        {
            private string _logicalName;
            private string _phisycalName;
            private string _defaultPrintEventId;
            List<PrintEvent> _printEvents;

            public Printer(string logicalName, string phisycalName, string defaultPrintEventId, List<PrintEvent> printEvents)
            {
                _logicalName = logicalName;
                _phisycalName = phisycalName;
                _defaultPrintEventId = defaultPrintEventId;
                _printEvents = printEvents;
            }            

            public string LogicalName
            {
                get { return _logicalName; }
            }

            public string PhisycalName
            {
                get { return _phisycalName; }
            }

            public string DefaultPrintEventId
            {
                get { return _defaultPrintEventId; }
            }

            public List<PrintEvent> PrintEvents
            {
                get { return _printEvents; }
            }

            public class PrintEvent
            {
                private string _eventId;
                private string _labelId;
                private string _nameSeparator;
                private bool _active;
                List<LabelVar> _labelVars;

                public PrintEvent(string eventId, string labelId, string nameSeparator, bool active, List<LabelVar> labelVars)
                {
                    _eventId = eventId;
                    _labelId = labelId;
                    _nameSeparator = nameSeparator;
                    _active = active;
                    _labelVars = labelVars;
                }

                public string EventId
                {
                    get { return _eventId; }
                }

                public string LabelId
                {
                    get { return _labelId; }
                }

                public string NameSeparator
                {
                    get { return _nameSeparator; }
                }

                public bool Active
                {
                    get { return _active; }
                }

                public List<LabelVar> LabelVars
                {
                    get { return _labelVars; }
                }

                public class LabelVar
                {
                    private string _name;
                    private string _value;

                    public LabelVar(string name, string value)
                    {
                        _name = name;
                        _value = value;
                    }

                    public string Name
                    {
                        get { return _name; }
                    }

                    public string Value
                    {
                        get { return _value; }
                    }
                }
            }
        }
    }
}
