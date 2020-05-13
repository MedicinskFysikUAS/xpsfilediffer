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

    }

    
}
