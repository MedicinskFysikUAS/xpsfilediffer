using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Controls;

namespace WpfApp1
{
    class TreatmentPlan
    {
        private List<List<string>> _pageList;

        public TreatmentPlan(List<List<string>> pageList)
        {
            _pageList = pageList;
        }

        //public string getValueFromSpaceSeparetedString(int pageIndex, string searchedString, int stringIndex)
        //{
        //    int foundIndex = _pageList[pageIndex].FindIndex(x => x.StartsWith(searchedString));
        //    string stringValue = "";
        //    if (foundIndex != -1)
        //    {
        //        string fullString = _pageList[pageIndex][foundIndex];
        //        int subStringLength = fullString.Length - searchedString.Length;
        //        stringValue = fullString.Substring(searchedString.Length, subStringLength);
        //    }

        //    if (stringValue.Split(' ').Length == 2)
        //    {
        //        stringValue = stringValue.Split(' ')[stringIndex];
        //    }
        //    return stringValue;
        //}

        public string patientFirstName()
        {
            StringExtractor stringExtractor = new StringExtractor();
            int pageIndex = 0;
            return stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 0);
        }

        public string patientLastName()
        {
            StringExtractor stringExtractor = new StringExtractor();
            int pageIndex = 0;
            return stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 1);
        }

        //public string patientFirstName()
        //{
        //    int pageIndex = 0;
        //    string searchedString = "Patient name:";
        //    int foundIndex = _pageList[pageIndex].FindIndex(x => x.StartsWith(searchedString));
        //    string stringValue = "";
        //    if (foundIndex != -1)
        //    {
        //        string fullString = _pageList[pageIndex][foundIndex];
        //        int subStringLength = fullString.Length - searchedString.Length;
        //        stringValue = fullString.Substring(searchedString.Length, subStringLength);
        //    }

        //    if (stringValue.Split(' ').Length == 2)
        //    {
        //        stringValue = stringValue.Split(' ')[0];
        //    }

        //    return stringValue;
        //}
    }
}
