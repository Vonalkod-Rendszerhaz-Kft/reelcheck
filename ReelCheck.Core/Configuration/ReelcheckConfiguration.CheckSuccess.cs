using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Vrh.LinqXMLProcessor.Base;
using Vrh.CheckSuccess;

namespace ReelCheck.Core.Configuration
{
    public partial class ReelcheckConfiguration : LinqXMLProcessorBaseClass
    {
        /// <summary>
        /// A configurációs XML releváns szerkezetének leírása
        /// </summary>
        internal static class CheckSuccessElement
        {
            internal const string NAME = "CheckSuccess";
            /// <summary>
            /// A CheckSuccess XML element lehetséges attributumai
            /// </summary>
            internal static class Attributes
            {
                internal const string NAMESEPARATOR = "NameSeparator";
            }

            /// <summary>
            /// Condition XML element leírása
            /// </summary>
            internal static class ConditionElement
            {
                /// <summary>
                /// Condition XML element neve
                /// </summary>
                internal const string NAME = "Condition";
                /// <summary>
                /// A Condition XML element lehetséges attributumai
                /// </summary>
                internal static class Attributes
                {
                    internal const string TYPE = "Type";
                    internal const string TEST = "Test";
                    internal const string WITH = "With";
                }
            }
        }

        public CheckSuccessType CheckSuccess
        {
            get
            {
                CheckSuccessType result;

                //Conditions feltöltése

                List<CheckSuccessType.Condition> conditions = new List<CheckSuccessType.Condition>();

                foreach (var conditionElement in
                                GetAllXElements(CheckSuccessElement.NAME, CheckSuccessElement.ConditionElement.NAME))
                {
                    CheckSuccessType.Condition condition = new CheckSuccessType.Condition(GetAttribute(conditionElement, CheckSuccessElement.ConditionElement.Attributes.TYPE, string.Empty),
                        GetAttribute(conditionElement, CheckSuccessElement.ConditionElement.Attributes.TEST, string.Empty),
                        GetAttribute(conditionElement, CheckSuccessElement.ConditionElement.Attributes.WITH, string.Empty));
                    conditions.Add(condition);
                }
                
                result = new CheckSuccessType(GetAttribute(GetXElement(CheckSuccessElement.NAME), CheckSuccessElement.Attributes.NAMESEPARATOR, string.Empty),
                    conditions);

                return result;
            }
        }
    }
}
