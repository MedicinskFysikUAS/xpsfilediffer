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


        public List<Tuple<string, string>> valuesInIntervall(List<string> stringsOnPage, int startIndex, int stopIndex)
        {
            List<Tuple<string,string>> posAndTimeValues = new List<Tuple<string,string>>();
            for (int i = startIndex + 2 ; i < stopIndex; i++)
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
            for (int i = startIndex + 2; i < stopIndex - 1; i++)
            {
                string position = stringsOnPage[i].Substring(0, 3);
                string time = stringsOnPage[i].Substring(stringsOnPage[i].Length - 4, 4);
                posAndTimeValues.Add(new Tuple<string, string>(position, time));
            }
            return posAndTimeValues;
        }

    }
}
