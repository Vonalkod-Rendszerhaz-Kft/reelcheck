using System;
using System.Collections.Generic;
using System.Configuration;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// Munkhely típus (munkahely azonosítója)
        /// </summary>
        public WorkStationType WorkStationType
        {
            get
            {
                return GetEnumValue<WorkStationType>(GetXElement(GeneralParametersElement.NAME, GeneralParametersElement.StationTypeElement.NAME), WorkStationType.Automatic);
            }
        }

        /// <summary>
        /// Folymat típus
        /// </summary>
        public ProcessType ProcessType
        {
            get
            {
                return GetEnumValue<ProcessType>(GetXElement(GeneralParametersElement.NAME, GeneralParametersElement.ProcessElement.NAME), ProcessType.Label);
            }
        }

        /// <summary>
        /// Időköltség logolás engedályezett/tiltott
        /// </summary>
        public bool TimeLog
        {
            get
            {
                return GetElementValue<bool>(GetXElement(GeneralParametersElement.NAME, GeneralParametersElement.TimeLogElement.NAME), false);
            }
        }

        /// <summary>
        /// Enable I/O bemenet
        /// </summary>
        public DigitalIO EnableIOInput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.Enable).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 0) : (byte)0,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A cimke feklhelyezés megtörténtét jelző bemenet
        /// </summary>
        public DigitalIO StickingDoneIOInput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.StickingDone).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 1) : (byte)1,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A PLC státusz resetet küld
        /// </summary>
        public DigitalIO StatusResetIOInput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.StatusReset).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 1) : (byte)1,
                    Type = type,
                };
            }
        }


        /// <summary>
        /// LABELD: A gép teljes resetelése (folymatok eldobása --> alapálapot)
        /// </summary>
        public DigitalIO HardResetIOInput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.HardReset).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 2) : (byte)2,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// LABELE: Az id ckamera olvasásindító triggere
        /// </summary>
        public DigitalIO IdCameraEnable
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.IdCameraEnable).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 0) : (byte)0,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// LABELE: Az id ckamera olvasásindító triggere
        /// </summary>
        public DigitalIO CheckCameraEnable
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOInputs).Name,
                                typeof(GeneralParametersElement.IOInputs.CheckCameraEnable).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 2) : (byte)2,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A szoftver működésre kész állapotát jelző I/O kimenet
        /// </summary>
        public DigitalIO ReadyIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.Ready).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 0) : (byte)0,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A sikeres cimkenyomtatást jelző I/O kimenet
        /// </summary>
        public DigitalIO LabelPrintedIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.LabelPrinted).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 1) : (byte)1,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// Jelzi, hogy minden folymamt befejeződött
        /// </summary>
        public DigitalIO AllStationFinishedIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.AllStationFinished).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 2) : (byte)2,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A kiforgó tekercs OK (PASS) státuszát jelző I/O kimenet
        /// </summary>
        public DigitalIO WorkpieceOKIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.WorkpieceOK).Name);
                DigitalIOType type = DigitalIOType.IO;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IO);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 0) : (byte)0,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A kiforgó tekercs NOK (FAIL) státuszát jelző I/O kimenet
        /// </summary>
        public DigitalIO WorkpieceNOKIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.WorkPieceNOK).Name);
                DigitalIOType type = DigitalIOType.IO;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IO);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 1) : (byte)1,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// A kiforgó pozición nincs tekercs
        /// </summary>
        public DigitalIO EmptyIOOutput
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.Empty).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 3) : (byte)3,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// IO Output: Az ID kamera elksézült az azonosítással
        /// </summary>
        public DigitalIO IdCameraReady
        {
            get
            {
                var element = GetXElement(GeneralParametersElement.NAME,
                                typeof(GeneralParametersElement.IOOutputs).Name,
                                typeof(GeneralParametersElement.IOOutputs.IdCameraReady).Name);
                DigitalIOType type = DigitalIOType.IOExt;
                if (element != null)
                {
                    type = GetEnumAttributeValue<DigitalIOType>(element,
                        typeof(GeneralParametersElement.IOInputs.Attributes.Type).Name,
                        DigitalIOType.IOExt);
                }
                return new DigitalIO()
                {
                    Port = element != null ? GetElementValue<byte>(element, 3) : (byte)3,
                    Type = type,
                };
            }
        }

        /// <summary>
        /// Digitlis ki/bemenet
        /// </summary>
        public class DigitalIO
        {
            /// <summary>
            /// Portszám
            /// </summary>
            public byte Port { get; set; }

            /// <summary>
            /// Típus
            /// </summary>
            public DigitalIOType Type { get; set; }
        }

        /// <summary>
        /// Ezek után a dátumok után GoldenSample tesztre van szükség
        /// </summary>
        public List<TimeSpan> DueTimers
        {
            get
            {
                var result = new List<TimeSpan>();
                try
                {
                    var elements = GetAllXElements(GeneralParametersElement.NAME,
                                                    GeneralParametersElement.GoldenSampleModeElement.NAME,
                                                    GeneralParametersElement.GoldenSampleModeElement.DueTimersElement.NAME,
                                                    GeneralParametersElement.GoldenSampleModeElement.DueTimersElement.DueTimerElement.NAME);
                    foreach (var element in elements)
                    {
                        TimeSpan ts;
                        if (TimeSpan.TryParse(element.Value, out ts))
                        {
                            result.Add(ts);
                        }
                    }
                }
                catch { }
                return result;
            }
        }

        /// <summary>
        /// Ennyi azonosítás után újabb GoldenSample tesuzt kell
        /// </summary>
        public int DueCycleCounterLimit
        {
            get
            {
                return GetElementValue<int>(GetXElement(
                                                        GeneralParametersElement.NAME,
                                                        GeneralParametersElement.GoldenSampleModeElement.NAME,
                                                        GeneralParametersElement.GoldenSampleModeElement.DueCycleCounterLimit.NAME), 0);
            }
        }

        /// <summary>
        /// Ennyi sikeres egymásutáni ellenőrzés kell a sikeres GoldenSample teszthez
        /// </summary>
        public int GoldenSampeCycles
        {
            get
            {
                return GetElementValue<int>(GetXElement(
                                                        GeneralParametersElement.NAME,
                                                        GeneralParametersElement.GoldenSampleModeElement.NAME,
                                                        GeneralParametersElement.GoldenSampleModeElement.GoldenSampleCycles.NAME), 0);
            }
        }

        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class GeneralParametersElement
        {
            internal const string NAME = "GeneralParameters";
            internal static class StationTypeElement
            {
                /// <summary>
                /// StationType XML element neve
                /// </summary>
                internal const string NAME = "StationType";
            }

            internal static class ProcessElement
            {
                /// <summary>
                /// Station XML element neve
                /// </summary>
                internal const string NAME = "Process";
            }

            /// <summary>
            /// Van-e idő logolás
            /// </summary>
            internal static class TimeLogElement
            {
                /// <summary>
                /// Az idő logolást engedélyező elem neve
                /// </summary>
                internal const string NAME = "TimeLog";
            }

            /// <summary>
            /// GoldenSampleMode XML element leírása
            /// </summary>
            internal static class GoldenSampleModeElement
            {
                /// <summary>
                /// GoldenSampleMode XML element neve
                /// </summary>
                internal const string NAME = "GoldenSampleMode";

                internal static class DueTimersElement
                {
                    internal const string NAME = "DueTimers";
                    internal static class DueTimerElement
                    {
                        internal const string NAME = "DueTimer";
                    }
                }
                internal static class DueCycleCounterLimit
                {
                    internal const string NAME = "DueCycleCounterLimit";
                }
                internal static class GoldenSampleCycles
                {
                    internal const string NAME = "GoldenSampleCycles";
                }
            }

            /// <summary>
            /// I/O bemenetek
            /// </summary>
            internal static class IOInputs
            {
                /// <summary>
                /// Enable bemenet bekötése 
                /// </summary>
                internal static class Enable { }
                /// <summary>
                /// StickingDone bemenet 
                /// </summary>
                internal static class StickingDone { }
                /// <summary>
                /// LABELD: Státusz reset bemenet
                /// </summary>
                internal static class StatusReset { }
                /// <summary>
                /// LABELD: Hard Reset
                /// </summary>
                internal static class HardReset { }
                /// <summary>
                /// LABELE: Az Id kamera negedélyező triggere
                /// </summary>
                internal static class IdCameraEnable { }
                /// <summary>
                /// LABELE: A Check kamera negedélyező triggere
                /// </summary>
                internal static class CheckCameraEnable { }
                /// <summary>
                /// Attribútumok
                /// </summary>
                internal static class Attributes
                {
                    /// <summary>
                    /// Típus
                    /// </summary>
                    internal static class Type { }
                }
            }
            /// <summary>
            /// IO kimenetek
            /// </summary>
            internal static class IOOutputs
            {
                /// <summary>
                /// A kész státuszt jelző I/O kimenet
                /// </summary>
                internal static class Ready { }
                /// <summary>
                /// A kimenet jelzi a sikeres cimkenyomtatást
                /// </summary>
                internal static class LabelPrinted { }
                /// <summary>
                /// A kimenet jelzi az összes folymamt befejeztést
                /// </summary>
                internal static class AllStationFinished { }
                /// <summary>
                /// A kimenet jelzi, hogy a kiforgó tekercs PASS (OK) státuszt kapott
                /// </summary>
                internal static class WorkpieceOK { }
                /// <summary>
                /// A kimenet jelzi, hogy a kiforgó tekercs FAIL (NOK) státuszt kapott
                /// </summary>
                internal static class WorkPieceNOK { }
                /// <summary>
                /// Jelzi, hogy a kivevő pozición nincs tekercs
                /// </summary>
                internal static class Empty { }
                /// <summary>
                /// Jelzi, hogy az Id camera elkészült az azonosítás műveklettel
                /// </summary>
                internal static class IdCameraReady { }
                /// <summary>
                /// Attribútumok
                /// </summary>
                internal static class Attributes
                {
                    /// <summary>
                    /// Típus
                    /// </summary>
                    internal static class Type { }
                }
            }
        }
    }
}
