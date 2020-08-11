using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class StringExtractor
    {

        public string getValueFromSpaceSeparetedString(List<string> stringsOnPage, string searchedString, int stringIndex)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x.StartsWith(searchedString));
            string stringValue = "";
            if (foundIndex != -1)
            {
                string fullString = stringsOnPage[foundIndex];
                int subStringLength = fullString.Length - searchedString.Length;
                stringValue = fullString.Substring(searchedString.Length, subStringLength);
            }

            if (stringValue.Split(' ').Length == 2)
            {
                stringValue = stringValue.Split(' ')[stringIndex];
            }
            return stringValue;
        }

        public string getValueBetweenSearchStrings(List<string> stringsOnPage, string startString, string endString)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x.StartsWith(startString));
            string stringValue = "";
            if (foundIndex != -1)
            {
                string fullString = stringsOnPage[foundIndex];
                int endIndex = fullString.IndexOf(endString);
                if (endIndex != -1)
                {
                    stringValue = fullString.Substring(startString.Length, (endIndex - startString.Length)).Trim();
                }
                else
                {
                    stringValue = fullString.Substring(startString.Length, (fullString.Length - startString.Length)).Trim();
                }
            }
            return stringValue;
        }


        public string getValueAfterSearchString(List<string> stringsOnPage, string searchedString, int stringIndex)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                stringValue = stringsOnPage[foundIndex + 1];
            }

            if (stringValue.Split(',').Length == 2)
            {
                stringValue = stringValue.Split(',')[stringIndex];
            }
            return stringValue.Trim();
        }

        public string getValueNStepAfterSearchString(List<string> stringsOnPage, string searchedString, int steps)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                stringValue = stringsOnPage[foundIndex + steps];
            }
            return stringValue.Trim();
        }

        public string getValueNStepAfterSearchStringTwoTimes(List<string> stringsOnPage, string searchedString, int steps)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                foundIndex = stringsOnPage.FindIndex(foundIndex + 1, x => x == searchedString);
                if (foundIndex != -1)
                {
                    stringValue = stringsOnPage[foundIndex + steps];
                    if (stringValue.Split(' ').Length == 2)
                    {
                        stringValue = stringValue.Split(' ')[0];
                    }
                }
            }
            return stringValue.Trim();
        }

        public string getValueAtIndex(List<string> stringsOnPage, int lineIndex, int stringIndex)
        {
            string stringValue = "";
            if (lineIndex != -1 && lineIndex < stringsOnPage.Count)
            {
                stringValue = stringsOnPage[lineIndex];
            }

            if (stringValue.Split(',').Length == 2)
            {
                stringValue = stringValue.Split(',')[stringIndex];
            }
            return stringValue.Trim();
        }

        public string getValueBeforeSearchString(List<string> stringsOnPage, string searchedString, int stringIndex)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                stringValue = stringsOnPage[foundIndex - 1];
            }

            if (stringValue.Split(',').Length == 2)
            {
                stringValue = stringValue.Split(',')[stringIndex];
            }
            return stringValue.Trim();
        }

        public string getValueBeforeSearchStringSplitOnSpace(List<string> stringsOnPage, string searchedString, int stringIndex)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                stringValue = stringsOnPage[foundIndex - 1];
            }

            if (stringValue.Split(' ').Length == 2)
            {
                stringValue = stringValue.Split(' ')[stringIndex];
            }
            return stringValue.Trim();
        }

        public string getValueAfterSearchStringSplitOnSpace(List<string> stringsOnPage, string searchedString, int stringIndex)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            string stringValue = "";
            if (foundIndex != -1)
            {
                stringValue = stringsOnPage[foundIndex + 1];
            }

            if (stringValue.Split(' ').Length == 2)
            {
                stringValue = stringValue.Split(' ')[stringIndex];
            }
            return stringValue.Trim();
        }

        public bool foundKeyWord(List<string> stringsOnPage, string searchedString)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x == searchedString);
            return (foundIndex != -1);
        }

        public int  getIndexOnPageAfterStartIndex(List<string> stringsOnPage, int startIndex, string searchedString)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x.StartsWith(searchedString));
            return stringsOnPage.FindIndex(startIndex, x => x.StartsWith(searchedString));
        }

        public int getIndexOnPage(List<string> stringsOnPage, string searchedString)
        {
            return stringsOnPage.FindIndex(x => x.StartsWith(searchedString));
        }

        public string getStringAfterStartWithSearchString(List<string> stringsOnPage, string searchedString)
        {
            int foundIndex = stringsOnPage.FindIndex(x => x.StartsWith(searchedString));
            string stringValue = "";
            if (foundIndex != -1)
            {
                string fullString = stringsOnPage[foundIndex];
                int subStringLength = fullString.Length - searchedString.Length;
                stringValue = fullString.Substring(searchedString.Length, subStringLength);
            }
            return stringValue;
        }

        public int getIndexOnPageForSearchedStringFromIndex(List<string> stringsOnPage, int startIndex, string searchedString)
        {
            return stringsOnPage.FindIndex(startIndex, x => x == searchedString);
        }

        public int getIndexOnPageForStartWithStringFromIndex(List<string> stringsOnPage, int startIndex, string searchedString)
        {
            return stringsOnPage.FindIndex(startIndex, x => x.StartsWith(searchedString));
        }

        public int getIndexOnPageForStartWithSearchedStringFromIndex(List<string> stringsOnPage, int startIndex, string searchedString, string containString)
        {
            return stringsOnPage.FindIndex(startIndex, x => x.StartsWith(searchedString) && x.Contains(containString));
        }

        public List<List<string>> nColumnsRowsInInterval(int nColumns, List<string> stringsInIntervall)
        {
            List<List<string>> allItems = new List<List<string>>();
            int nRows = stringsInIntervall.Count / nColumns;

            for (int row = 0; row < nRows; row++)
            {
                List<string> tenItems = new List<string>();
                for (int column = 0; column < nColumns; column++)
                {
                    tenItems.Add(stringsInIntervall[column + (row * nColumns)]);
                }
                allItems.Add(tenItems);

            }
            return allItems;
        }

        public List<string> allValuesInInterval(List<string> stringsOnPage, int startIndex, int stopIndex)
        {
            List<string> lines = new List<string>();
            for (int i = startIndex + 1; i < stopIndex; i++)
            {
                lines.Add(stringsOnPage[i]);
            }
            return lines;
        }

        public List<Tuple<string, string>> valuesInIntervall(List<string> stringsOnPage, int startIndex, int stopIndex)
        {
            List<Tuple<string,string>> posAndTimeValues = new List<Tuple<string,string>>();
            for (int i = startIndex + 1 ; i < stopIndex; i++)
            {
                string position = stringsOnPage[i].Substring(0, 3);
                string time = stringsOnPage[i].Substring(stringsOnPage[i].Length - 4, 4);
                posAndTimeValues.Add(new Tuple<string, string>(position, time));
            }
            return posAndTimeValues;
        }

        public List<Tuple<string, string>> valuesUntilSearchedString(List<string> stringsOnPage, int startIndex, string searchedString)
        {
            int stopIndex = getIndexOnPageAfterStartIndex(stringsOnPage, startIndex, searchedString);
            List<Tuple<string, string>> posAndTimeValues = new List<Tuple<string, string>>();
            for (int i = startIndex + 1; i < stopIndex; i++)
            {
                string position = stringsOnPage[i].Substring(0, 3);
                string time = stringsOnPage[i].Substring(stringsOnPage[i].Length - 4, 4);
                posAndTimeValues.Add(new Tuple<string, string>(position, time));
            }
            return posAndTimeValues;
        }

        public List<Tuple<string, string>> valuesFromSearchedString(List<string> stringsOnPage, string searchedString, int stopIndex)
        {
            int startIndex = getIndexOnPage(stringsOnPage, searchedString);
            List<Tuple<string, string>> posAndTimeValues = new List<Tuple<string, string>>();
            for (int i = startIndex + 1; i < stopIndex; i++)
            {
                string position = stringsOnPage[i].Substring(0, 3);
                string time = stringsOnPage[i].Substring(stringsOnPage[i].Length - 4, 4);
                posAndTimeValues.Add(new Tuple<string, string>(position, time));
            }
            return posAndTimeValues;
        }

        public string decimalStringToZeroDecimalString(string decimalString)
        {
            string localDecimalString = decimalString;
            if (decimalString.Contains('.'))
            {
                localDecimalString = decimalString.Replace('.', ',');
            }
            Decimal decimalValue = Convert.ToDecimal(localDecimalString);
            Decimal decimalValueOneDec = Math.Round(decimalValue, 0, MidpointRounding.ToEven);
            return String.Format("{0:0}", Convert.ToDecimal(decimalValueOneDec));
        }

        public string decimalStringToOneDecimalString(string decimalString)
        {
            string localDecimalString = decimalString;
            if (decimalString.Contains('.'))
            {
                localDecimalString = decimalString.Replace('.', ',');
            }
            Decimal decimalValue = Convert.ToDecimal(localDecimalString);
            Decimal decimalValueOneDec = Math.Round(decimalValue, 1, MidpointRounding.ToEven);
            return String.Format("{0:0.0}", Convert.ToDecimal(decimalValueOneDec));
        }

        public string decimalStringToTwoDecimalString(string decimalString)
        {
            string localDecimalString = decimalString;
            if (decimalString.Contains('.'))
            {
                localDecimalString = decimalString.Replace('.', ',');
            }
            Decimal decimalValue = Convert.ToDecimal(localDecimalString);
            Decimal decimalValueOneDec = Math.Round(decimalValue, 2, MidpointRounding.ToEven);
            return String.Format("{0:0.00}", Convert.ToDecimal(decimalValueOneDec));
        }

        public string decimalToTwoDecimalString(decimal decimalValue)
        {   
            return String.Format("{0:0.00}", decimalValue);
        }

        public decimal decimalStringToDecimal(string decimalString)
        {
            string localDecimalString = decimalString;
            if (decimalString.Contains('.'))
            {
                localDecimalString = decimalString.Replace('.', ',');
            }
            return Convert.ToDecimal(localDecimalString);
        }

        public int catheterNumberFromString(string catheterString)
        {
            string integerString = catheterString.Replace('L', ' ');
            return int.Parse(integerString.Trim());
        }

        public bool isYesString(string inputString)
        {
            return inputString.ToLower() == "yes";
        }

        public bool isNoString(string inputString)
        {
            return inputString.Trim().ToLower() == "no";
        }
        // TODO: Date time format string is hard coded in lots of places
        // TODO: it should be defined in on place. For instance in Tyhpes.cs
        public DateTime stringToDateTime(string inputString)
        {
            return DateTime.ParseExact(inputString, Constants.DATE_AND_TIME_FORMAT,
                                           System.Globalization.CultureInfo.InvariantCulture);
        }

    }

    
}
