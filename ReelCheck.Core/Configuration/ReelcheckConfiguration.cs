using System;
using System.Configuration;
using Vrh.LinqXMLProcessor.Base;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// A konfiguráció mögöti XML file 
        /// </summary>
        public string XmlFileDefinition => _xmlFileDefinition;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterFile">Az XMl fájl amit kezel</param>
        public ReelcheckConfiguration(string parameterFile)
        {
            _xmlFileDefinition = parameterFile;
        }
    }
}
