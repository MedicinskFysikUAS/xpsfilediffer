using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace WpfApp1
{
    class TccPlan
    {
        private List<List<string>> _pageList;
        private StringExtractor _stringExtractor = new StringExtractor();
        private List<LiveCatheter> _liveCatheters;


        public TccPlan(List<List<string>> pageList, List<LiveCatheter> liveCatheters)
        {
            _pageList = pageList;
            _liveCatheters = liveCatheters;
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

        public string patientId()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueBeforeSearchString(_pageList[pageIndex], "Plankod", 0);
        }

        public string planCode()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Plankod", 0);
        }

        public string statusSetDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Sparat datum/tid", 0);
            string pattern = "dd MMM yyyy / HH:mm (UTC +2:00)";
            string stringFromDateTime = "";
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, pattern, null,
                                      DateTimeStyles.None, out parsedDate))
            {
                stringFromDateTime = parsedDate.ToString("yyyy-MM-dd HH:mm");
            }
            return stringFromDateTime;
        }

        public bool planIsApproved()
        {
            return _stringExtractor.foundKeyWord(_pageList[0], "Godkänd den");
        }

        public string fractionDose()
        {
            int pageIndex = 0;
            return String.Format("{0:0.00}", Convert.ToDecimal(_stringExtractor.getValueBeforeSearchStringSplitOnSpace(_pageList[pageIndex], "Planerad TRAK", 0)));
        }

        public string plannedSourceStrength()
        {
            int pageIndex = 0;
            return String.Format("{0:0}", Convert.ToDecimal(_stringExtractor.getValueAfterSearchStringSplitOnSpace(_pageList[pageIndex], "Planerad AK-styrka", 0)));
        }

        public string totalTreatmentTime()
        {
            int pageIndex = 0;
            return String.Format("{0:0.0}", Convert.ToDecimal(_stringExtractor.getValueAfterSearchStringSplitOnSpace(_pageList[pageIndex], "Total strålningstid", 0)));
        }

        public List<LiveCatheter> liveCatheters()
        {
            return _liveCatheters;
        }
    }
}
