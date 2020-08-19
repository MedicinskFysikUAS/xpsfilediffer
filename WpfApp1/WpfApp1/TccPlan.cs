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
        private List<LiveCatheter> _liveCatheters = new List<LiveCatheter>();


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

        string  toStdDateString(string dateString)
        {
            string pattern1 = "dd MMM yyyy / HH:mm (UTC +1:00)";
            string pattern2 = "d MMM yyyy / HH:mm (UTC +1:00)";
            string pattern3 = "dd MMM yyyy / HH:mm (UTC +2:00)";
            string pattern4 = "d MMM yyyy / HH:mm (UTC +2:00)";
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, pattern1, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern2, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern3, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern4, null, DateTimeStyles.None, out parsedDate)
                                      )
            {
                return parsedDate.ToString(Constants.DATE_AND_TIME_FORMAT);
            }
            else
            {
                return "";
            }

        }

        public string statusSetDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Godkänd den", 2);
            return toStdDateString(dateString);
        }

        public string calibrationDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Kalibreringsdatum/-tid", 4);
            return toStdDateString(dateString);
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

        public decimal PrescribedDose()
        {
            int pageIndex = 0;
            return Convert.ToDecimal(_stringExtractor.getValueBeforeSearchStringSplitOnSpace(_pageList[pageIndex], "Planerad TRAK", 0));
        }

        public decimal plannedSourceStrength()
        {
            int pageIndex = 0;
            return Convert.ToDecimal(_stringExtractor.getValueAfterSearchStringSplitOnSpace(_pageList[pageIndex], "Planerad AK-styrka", 0));
        }

        public decimal realizedSourceStrength()
        {
            int pageIndex = 0;
            return _stringExtractor.decimalStringToDecimal(_stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Realiserad AK-styrka", 1));
        }

        public decimal calibratedSourceStrength()
        {
            int pageIndex = 0;
            return _stringExtractor.decimalStringToDecimal(_stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Kalibrerad källstyrka", 4));
        }

        public string totalTreatmentTime()
        {
            int pageIndex = 0;
            return String.Format("{0:0.0}", Convert.ToDecimal(_stringExtractor.getValueAfterSearchStringSplitOnSpace(_pageList[pageIndex], "Total strålningstid", 0)));
        }

        public decimal totalTreatmentTimeValue()
        {
            int pageIndex = 0;
            return Convert.ToDecimal(_stringExtractor.getValueAfterSearchStringSplitOnSpace(_pageList[pageIndex], "Total strålningstid", 0));
        }

        public decimal realizedTotalTreatmentTime()
        {
            int pageIndex = 0;
            return _stringExtractor.decimalStringToDecimal((_stringExtractor.getValueNStepAfterSearchStringTwoTimes(_pageList[pageIndex], "Total strålningstid", 1)));
        }

        public string realizationDateAndTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Datum/tid", 1);
            return toStdDateString(dateString);
        }

        public List<LiveCatheter> liveCatheters()
        {
            return _liveCatheters;
        }
    }
}
