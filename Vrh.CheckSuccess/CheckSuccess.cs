namespace Vrh.CheckSuccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Az ellenörző olvasás kiértékelés
    /// </summary>
    public static class CheckSuccess
    {
        /// <summary>
        /// Kondíciók közti művelet típus
        /// </summary>
        public enum CheckOperations
        {
            /// <summary>
            /// És
            /// </summary>
            AND = 0,
            // OR
        }

        /// <summary>
        /// Ellenörzés végrehajtása
        /// </summary>
        /// <param name="checkSuccess">Konfigurációból szármozó kondíció beállítások</param>
        /// <param name="dataElements">Kulcs érték párok az adatbehelyettesítéshez</param>
        /// <param name="checkOperations">Művelet típusa</param>
        /// <returns>Ellenörzés sikeressége igen/nem</returns>
        public static bool DoCheck(CheckSuccessType checkSuccess, Dictionary<string, string> dataElementsForTest, Dictionary<string, string> dataElementsForWith, CheckOperations checkOperations)
        {
            bool result = false;
            try
            {
                foreach (CheckSuccessType.Condition condition in checkSuccess.Conditions)
                {
                    bool isCheckSuccess = false;
                    string test = condition.Test;
                    string with = condition.With;
                    string nameStart = "{";
                    string nameEnd = "}";
                    foreach (KeyValuePair<string, string> dataElement in dataElementsForTest)
                    {
                        test = test.Replace($"{nameStart}{dataElement.Key}{nameEnd}", dataElement.Value);
                    }
                    foreach (KeyValuePair<string, string> dataElement in dataElementsForWith)
                    {
                        with = with.Replace($"{nameStart}{dataElement.Key}{nameEnd}", dataElement.Value);
                    }
                    switch (condition.Type.ToUpper())
                    {
                        case "EQUAL":
                            isCheckSuccess = Equal(test, with);
                            break;
                        case "NOTEQUAL":
                            isCheckSuccess = NotEqual(test, with);
                            break;
                        case "MATCH":
                            isCheckSuccess = Match(test, with);
                            break;
                    }
                    bool exitLoop = false;
                    switch (checkOperations.ToString())
                    {
                        case "AND":
                            OperationAND(isCheckSuccess, ref result, out exitLoop);
                            break;
                        case "OR":
                            // Operation_OR(isCheckSuccess, ref result, out exitLoop);
                            break;
                    }
                    if (exitLoop)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// Test és With értéke ugyanaz
        /// </summary>
        /// <param name="test">Kiértékelendő karakter sorozat</param>
        /// <param name="with">
        /// Kiértékelendő karakter sorozat</param>
        /// <returns>Vizsgálat eredménye</returns>
        private static bool Equal(string test, string with)
        {
            bool result = true;
            try
            {
                result = test.Equals(with);
            }
            catch
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// Test és With értéke nemugyanaz
        /// </summary>
        /// <param name="test">Kiértékelendő karakter sorozat</param>
        /// <param name="with">Kiértékelendő karakter sorozat</param>
        /// <returns>Vizsgálat eredménye</returns>
        private static bool NotEqual(string test, string with)
        {
            bool result = true;
            try
            {
                result = !test.Equals(with);
            }
            catch
            {
                return false;
            }

            return result;
        }

        private static bool Match(string test, string with)
        {
            bool result = true;
            try
            {
                Regex regex = new Regex(with);
                var match = regex.Match(test);
                result = match.Success;
            }
            catch
            {
                return false;
            }

            return result;
        }

        private static void OperationAND(bool conditionCheckSuccess, ref bool sumCheckSuccess, out bool exitLoop)
        {
            exitLoop = false;

            try
            {
                if (!sumCheckSuccess && conditionCheckSuccess)
                {
                    sumCheckSuccess = true;
                }
                else if (sumCheckSuccess && !conditionCheckSuccess)
                {
                    sumCheckSuccess = false;
                    exitLoop = true;
                }
            }
            catch
            {
                sumCheckSuccess = false;
            }
        }
    }
}
