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
        private StringExtractor _stringExtractor = new StringExtractor();

        public TreatmentPlan(List<List<string>> pageList)
        {
            _pageList = pageList;
        }

        
        public string patientFirstName()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 0);
        }

        public string patientLastName()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 1);
        }

    }
}
