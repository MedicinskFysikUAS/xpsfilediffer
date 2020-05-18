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

        public Tuple<int, int>  getStartAndStopIndex((List<string> stringsOnPage, string searchedStartString, string searchedStopString)
        {
            Tuple<int, int> startStopIndex = new Tuple<int, int>(0,1);
            return startStopIndex;
        }

    }
}
