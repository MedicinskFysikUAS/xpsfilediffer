﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;

namespace WpfApp1
{
    public class TccPlan
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
        public string fractionNumber()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Fraktionsnummer", 0);
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
            return _liveCatheters.OrderBy(o => o.catheterNumber()).ToList();
        }

        public Dictionary<int, int>  getCatheterTochannel(int firstCatheterNumber = 0)
        {
            List<LiveCatheter> sortedLiveCatheters = liveCatheters();
            int counter = firstCatheterNumber;
            Dictionary<int, int> catheterTochannel = new Dictionary<int, int>();
            foreach (var sortedLiveCatheter in sortedLiveCatheters)
            {
                catheterTochannel.Add(counter, sortedLiveCatheter.catheterNumber());
                counter++;
            }
            return catheterTochannel;
        }

        public List<Tuple<int, decimal>> getChannelNumberAndLengths()
        {
            int pageIndex = 1;
            int start = _pageList[pageIndex].IndexOf("KateterKanalKanallängd (mm)Kanaltid (sek.)");
            int stop = _pageList[pageIndex].IndexOf("Positionstider (sek.)");
            List<string> allValues = _stringExtractor.allValuesInInterval(_pageList[pageIndex],
                start, stop);
            List<string> catheterLengthList = new List<string>();
            List<Tuple<int, decimal>> catheterNumberAndLengths = new List<Tuple<int, decimal>>();
            if (getCatheterTochannel().Count != allValues.Count)
            {
                return catheterNumberAndLengths;
            }
            char startIndexChar = allValues[0][0];
            int firstCatheterNumber = 0;
            if (Char.IsDigit(startIndexChar))
            {
                firstCatheterNumber = startIndexChar - '0';
            }
            Dictionary<int, int> catheterTochannelDict = getCatheterTochannel(firstCatheterNumber);
            for (int i = 0; i < catheterTochannelDict.Count; i++)
            {
                int characterLength = catheterTochannelDict[catheterTochannelDict.ElementAt(i).Key].ToString().Length +
                    catheterTochannelDict.ElementAt(i).Key.ToString().Length;
                int channelLengthCharacterLength = 6;
                int from = characterLength;
                catheterLengthList.Add(allValues[i].Substring(from, channelLengthCharacterLength).Trim());
                decimal x = 0.0m;
                decimal.TryParse(allValues[i].Substring(from, channelLengthCharacterLength).Trim(), out x);
                catheterNumberAndLengths.Add(new Tuple<int, decimal>(catheterTochannelDict[catheterTochannelDict.ElementAt(i).Key], x));
            }
            return catheterNumberAndLengths;

        }


    } // Class
}
