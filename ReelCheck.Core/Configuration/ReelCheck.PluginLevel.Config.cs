using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    /// <summary>
    /// Plugin szintű konfiguráció, a minden pluginra közös paraméterekre
    /// </summary>
    internal class PluginLevelConfig : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterFile">Az XMl fájl amit kezel</param>
        public PluginLevelConfig(string parameterFile)
        {
            _xmlFileDefinition = parameterFile;
        }

        /// <summary>
        /// A használt redis kiszolgáló
        /// </summary>
        public string RedisConnectionString
        {
            get
            {
                return GetElementValue(GetXElement(typeof(Me.Redis).Name), "localhost");
            }
        }

        /// <summary>
        /// Redis datapool neve
        /// </summary>
        public string DataPoolName
        {
            get
            {
                return GetElementValue(GetXElement(typeof(Me.DataPoolName).Name), "ReelCheck");
            }
        }

        /// <summary>
        /// A nyomtató szolgáltatás által használt EventHub csatorna
        /// </summary>
        public string PrintingServiceEventHubChannel
        {
            get
            {
                return GetElementValue(GetXElement(typeof(Me.PrintingServiceEventHubChannel).Name), "DATA");
            }
        }

        /// <summary>
        /// Inicializálja-e a nyelvi adatbázist a kódból?
        /// </summary>
        public bool InitializeMultilanguageDb
        {
            get
            {
                return GetElementValue<bool>(GetXElement(typeof(Me.InitializeMultilanguageDb).Name), false);
            }
        }

        /// <summary>
        /// A custom fvs rendszer szolgáltatás címe
        /// </summary>
        public string CustomFVSUri
        {
            get
            {
                return GetElementValue<string>(GetXElement(typeof(Me.CustomFVSURI).Name), "");
            }
        }

        /// <summary>
        /// A konfiguráció mögöti XML file 
        /// </summary>
        public string XmlFileDefinition => _xmlFileDefinition;

        #region Defination of namming rules in XML

        /// <summary>
        /// A konfigurációs XML leírása
        /// </summary>
        internal static class Me
        {
            /// <summary>
            /// A Redis connection stringet tároló XML element
            /// </summary>
            internal static class Redis { }
            /// <summary>
            /// Redis DataPool name
            /// </summary>
            internal static class DataPoolName { }
            /// <summary>
            /// Inicializálja-e a nyelvi adatbázist
            /// </summary>
            internal static class InitializeMultilanguageDb { }
            /// <summary>
            /// Noymztató szolgáltatás EventHub csatornája
            /// </summary>
            internal static class PrintingServiceEventHubChannel { }
            /// <summary>
            /// A custom fvs rendszer szolgáltatásának címe
            /// </summary>
            internal static class CustomFVSURI { }
        }

        #endregion Defination of namming rules in XML
    }
}
