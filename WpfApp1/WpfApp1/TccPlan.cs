using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class TccPlan
    {
        private List<List<string>> _pageList;
        private StringExtractor _stringExtractor = new StringExtractor();


        public TccPlan(List<List<string>> pageList)
        {
            _pageList = pageList;
        }

        public string patientFirstName()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Patient-ID", 1);
        }

        public string patientLastName()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Patient-ID", 0);
        }
    }
}
