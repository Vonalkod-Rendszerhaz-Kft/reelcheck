using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrh.CheckSuccess
{
    public class CheckSuccessType
    {
        public CheckSuccessType(string nameSeparator, List<Condition> conditions)
        {
            NameSeparator = nameSeparator;
            Conditions = conditions;
        }

        public string NameSeparator { get; }

        public List<Condition> Conditions { get; }

        public class Condition
        {
            public Condition(string type, string test, string with)
            {
                Type = type;
                Test = test;
                With = with;
            }

            public string Type { get; }

            public string Test { get; }

            public string With { get; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cond in Conditions)
            {
                sb.AppendLine($"Type: {cond.Type}, Test: {cond.Test}, With: {cond.With}");
            }
            return sb.ToString();
        }
    }
}
