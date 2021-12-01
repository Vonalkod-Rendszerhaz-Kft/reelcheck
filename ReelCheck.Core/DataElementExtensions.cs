using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RegexChecker;

namespace ReelCheck.Core
{
    /// <summary>
    /// Adatelem kezeléssekl kapcsolatos bővítő metódusok
    /// </summary>
    public static class DataElementExtensions
    {
        /// <summary>
        /// Egy dictioneryből kiemeli az értékeket és behejetesíti a kapott constructorStringbe, ha {} jelek közt szerepel a kulcs, 
        /// ha a kulcs nem szerepel a dictioneryybe akkor üres érték (String.Empty) helyetesítődik
        /// </summary>
        /// <param name="dataElements">az adatelemek (kulcs érték párok) listája, amiből dolgozik</param>
        /// <param name="constructorString">A konstruktor string</param>
        /// <param name="key">a visszadott kulcs-érétk pár kulcs ez lesz</param>
        /// <returns>kulcsérték pár, ahol a kulcs a kapott kulcs, a value a constructorstring az elvégzett helyetesítésekkel</returns>
        public static KeyValuePair<string, string> ConstructKeyValuePair(this Dictionary<string, string> dataElements, string constructorString, string key)
        {
            var deList = new List<DataElement>();
            foreach (var item in dataElements)
            {                
                var de = new DataElement()
                {
                    Name = item.Key,
                    Value = item.Value,
                };
                deList.Add(de);
            }
            return new KeyValuePair<string, string>(key, deList.ConstructString(constructorString));
        }

        public static string ConstructString(this List<KeyAndValue> dataElements, string constructorString)
        {
            var deList = new List<DataElement>();
            foreach (var item in dataElements)
            {
                var de = new DataElement()
                {
                    Name = item.Key,
                    Value = item.Value,
                };
                deList.Add(de);
            }
            return deList.ConstructString(constructorString);
        }

        /// <summary>
        /// Egy List<DataElement> kiemeli az értékeket és behejetesíti a kapott constructorStringbe, ha {} jelek közt szerepel a kulcs, 
        /// ha a kulcs nem szerepel a dictioneryybe akkor üres érték (String.Empty) helyetesítődik
        /// </summary>
        /// <param name="dataElements"></param>
        /// <param name="constructorString"></param>
        /// <returns></returns>
        public static string ConstructString(this List<DataElement> dataElements, string constructorString)
        {
            List<string> keysInConstructor = new List<string>();
            var allParts = constructorString.Split(new char[] { '{' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in allParts)
            {
                if (part.Contains("}"))
                {
                    keysInConstructor.Add(part.Substring(0, part.IndexOf("}")));
                }
            }
            foreach (var keyitem in keysInConstructor)
            {
                if (dataElements.Any(x => x.Name.ToUpper() == keyitem.ToUpper()))
                {
                    constructorString = constructorString.Replace($"{{{keyitem}}}", dataElements.FirstOrDefault(x => x.Name.ToUpper() == keyitem.ToUpper()).Value);
                }
                else
                {
                    constructorString = constructorString.Replace($"{{{keyitem}}}", "");
                }
            }
            return constructorString;
        }

        /// <summary>
        /// Biztonságosan hozzáad egy elemet egy kulcs érték párokat tartalmazó listához
        ///  Ha nincs még benen ilyen kulccsal --> hozzáadja
        ///  Ha már benne van, akkor  alétező elem Value értékét írja felül
        /// </summary>
        /// <param name="dataElements">Ezen a listán dolgozik</param>
        /// <param name="newElement">A hozzáadandó elem</param>
        public static void SafeAdd(this List<KeyAndValue> dataElements, KeyAndValue newElement)
        {
            var existingElement = dataElements.FirstOrDefault(x => x.Key == newElement.Key);
            if (existingElement != null)
            {
                existingElement.Value = newElement.Value;
            }
            else
            {
                dataElements.Add(newElement);
            }
        }

        /// <summary>
        /// A megadott maximális hoszban adja vissza a stringet (ha hoszabb a végét levágja)
        /// </summary>
        /// <param name="input">a bemenő string</param>
        /// <param name="maxLength">a maximálisan megengedett hossz</param>
        /// <returns></returns>
        public static string GetWithMaxLength(this string input, int maxLength)
        {
            if (maxLength <= 0)
            {
                return string.Empty;
            }
            if (input.Length <= maxLength)
            {
                return input;
            }
            else
            {
                return input.Substring(0, maxLength - 1);
            }
        }

        /// <summary>
        /// Tördeli egy bemenő stringet
        /// </summary>
        /// <param name="errorMessage">input string</param>
        /// <param name="lineLength">egy sor maximális hossza</param>
        /// <param name="lineNumber">ennyi sor van</param>
        /// <returns></returns>
        public static List<string> SplitErrorMessage(this string errorMessage, int lineLength = 25, int lineNumber = 5)
        {
            int line = 1;
            List<string> result = new List<string>();
            while(errorMessage.Length > 0 && line <= lineNumber)
            {                
                result.Add(errorMessage.Substring(0, lineLength < errorMessage.Length ? lineLength : errorMessage.Length));
                errorMessage = errorMessage.Remove(0, lineLength < errorMessage.Length ? lineLength : errorMessage.Length);
                lineNumber++;
            }
            return result;
        }

        /// <summary>
        /// Az ékezetes karaktereket a megfelelő ékezet nélküli párjára cseréli a bejövő stringben
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static string RemoveAccents(this string input)
        {
            string knownAccents = "áÁéÉöÖőŐüÜűŰóÓúÚíÍ";
            string knownSubstitute = "aAeEoOoOuUuUoOuUiI";
            if (knownAccents.Length != knownSubstitute.Length)
            {
                return input;
            }
            for (int i = 0; i < knownAccents.Length; i++)
            {
                input = input.Replace(knownAccents[i], knownSubstitute[i]);
            }
            return input;
        }

        /// <summary>
        /// Megadja a dátum hányadik hét az évben
        /// </summary>
        /// <param name="date">input dátum</param>
        /// <returns>mindig két karakteren</returns>
        public static string WeekNo(this DateTime date)
        {
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dtfi.Calendar;
            int weekNo = cal.GetWeekOfYear(date, dtfi.CalendarWeekRule, dtfi.FirstDayOfWeek);
            return weekNo.ToString("00");
        }

        /// <summary>
        /// Kibontja a felhazsnáló névből a delphi felhasználónevet
        /// </summary>
        /// <param name="fullUserName">A teljes felhazsnálónév, aláhúzások közt tartalmazza a delphi usernevet</param>
        /// <returns></returns>
        public static string GetDelphiUserName(this string fullUserName)
        {
            string delphiUserName = fullUserName;
            int firstUnderlinePosition = fullUserName.IndexOf("_");
            if (firstUnderlinePosition > -1)
            {
                int secondUnderlinePosition = fullUserName.IndexOf("_", firstUnderlinePosition);
                if (secondUnderlinePosition > -1)
                {
                    var splitted = fullUserName.Split('_');
                    if (splitted.Length > 1)
                    {
                        delphiUserName = splitted[1];
                    }
                }
            }
            return delphiUserName;
        }
    }
}
