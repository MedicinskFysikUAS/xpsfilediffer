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

        // TODO: Remove this when correct version is implemented
        public bool liveCatheterPositions()
        {
            return false;
        }

        public List<LiveCatheter> liveCatheters()
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();

            int pageIndex = 0;
            int startIndex = 0;
            foreach (var page in _pageList)
            {
                _stringExtractor.getIndexOnPageAfterStartIndex(page, startIndex + 1, "Posx [mm]y [mm]z [mm]WeightTime [s]");
            }

            return liveCatheters;

        }
    }
}
