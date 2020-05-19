using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Controls;
using System.Globalization;


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

        public string patientId()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient ID:", 0);
        }

        public string planCode()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Plan Code:", 0);
        }

        public string planStatus()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Plan status:", 0);
        }

        public string statusSetDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Status set at:", 0);
            string pattern = "HH:mm:ss, dd. MMMMM yyyy";
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
            return planStatus() == "APPROVED";
        }

        public string fractionDose()
        {
            int pageIndex = 1;
            string fractiondoseStr = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Prescribed Dose:", 0);
            fractiondoseStr = fractiondoseStr.Replace('.', ',');
            Decimal fractiondoseFl = Convert.ToDecimal(fractiondoseStr);
            return String.Format("{0:0.00}", Convert.ToDecimal(fractiondoseFl));
        }

        public string plannedSourceStrength()
        {
            int pageIndex = 0;
            string fractiondoseStr = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Planned Source Strength:", 0);
            if (fractiondoseStr.Contains('.'))
            {
                fractiondoseStr = fractiondoseStr.Replace('.', ',');
            }
            Decimal fractiondoseFl = Convert.ToDecimal(fractiondoseStr);
            Decimal zeroDecfractiondose = Math.Round(fractiondoseFl, 0);
            return String.Format("{0:0}", Convert.ToDecimal(zeroDecfractiondose));
        }

        public string totalTreatmentTime()
        {
            int pageIndex = 1;
            string totalTreatmentTimeStr = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Total Treatment Time:", 0);
            if (totalTreatmentTimeStr.Contains('.'))
            {
                totalTreatmentTimeStr = totalTreatmentTimeStr.Replace('.', ',');
            }
            Decimal totalTreatmentTime = Convert.ToDecimal(totalTreatmentTimeStr);
            Decimal totalTreatmentTimeOneDec = Math.Round(totalTreatmentTime, 1);
            return String.Format("{0:0.0}", Convert.ToDecimal(totalTreatmentTimeOneDec));

        }



        public List<LiveCatheter> liveCatheters()
        {
            List<CatheterPositonAndTimeTable> startPositonAndTimeTables = new List<CatheterPositonAndTimeTable>();
            List<CatheterPositonAndTimeTable> stopPositonAndTimeTables = new List<CatheterPositonAndTimeTable>();

            int pageIndex = 0;
            foreach (var page in _pageList)
            {
                int startIndex = 0;
                int stopIndex = 0;
                while (startIndex != -1)
                {
                    CatheterPositonAndTimeTable startPositonAndTimeTable = new CatheterPositonAndTimeTable();
                    startIndex = _stringExtractor.getIndexOnPageAfterStartIndex(page, startIndex + 1, "Posx [mm]y [mm]z [mm]WeightTime [s]");
                    if (startIndex != -1)
                    {
                        startPositonAndTimeTable.setStartIndex(startIndex);
                        startPositonAndTimeTable.setPageIndex(pageIndex);
                        startPositonAndTimeTables.Add(startPositonAndTimeTable);
                    }
                }

                while (stopIndex != -1)
                {
                    CatheterPositonAndTimeTable stopPositonAndTimeTable = new CatheterPositonAndTimeTable();
                    stopIndex = _stringExtractor.getIndexOnPageAfterStartIndex(page, stopIndex + 1, "Total Time");
                    if (stopIndex != -1)
                    {
                        stopPositonAndTimeTable.setStopIndex(stopIndex);
                        stopPositonAndTimeTable.setPageIndex(pageIndex);
                        stopPositonAndTimeTables.Add(stopPositonAndTimeTable);
                    }
                }
                ++pageIndex;
            }

            int counter = 0;
            List<CatheterPositonAndTimeTable> startAndStopPositonAndTimeTables = new List<CatheterPositonAndTimeTable>();
            foreach (var startPositonAndTimeTable in startPositonAndTimeTables)
            {
                CatheterPositonAndTimeTable startAndStopPositonAndTimeTable = new CatheterPositonAndTimeTable();
                startAndStopPositonAndTimeTable.setStartPageIndex(startPositonAndTimeTable.pageIndex());
                startAndStopPositonAndTimeTable.setStartIndex(startPositonAndTimeTable.startIndex());
                startAndStopPositonAndTimeTable.setStopPageIndex(stopPositonAndTimeTables[counter].pageIndex());
                startAndStopPositonAndTimeTable.setStopIndex(stopPositonAndTimeTables[counter].stopIndex());
                startAndStopPositonAndTimeTables.Add(startAndStopPositonAndTimeTable);
                ++counter;
            }

            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            counter = 1;
            foreach (var startAndStopPositonAndTimeTable in startAndStopPositonAndTimeTables)
            {
                LiveCatheter liveCatheter = new LiveCatheter();
                if (startAndStopPositonAndTimeTable.startPageIndex() == startAndStopPositonAndTimeTable.stopPageIndex())
                {
                    List<Tuple<string, string>> values = _stringExtractor.valuesInIntervall(_pageList[startAndStopPositonAndTimeTable.startPageIndex()],
                        startAndStopPositonAndTimeTable.startIndex(),
                        startAndStopPositonAndTimeTable.stopIndex());
                    liveCatheter.setPositonTimePairs(values);
                    liveCatheter.setCatheterNumber(counter);
                }
                else if (startAndStopPositonAndTimeTable.startPageIndex() < startAndStopPositonAndTimeTable.stopPageIndex())
                {
                    List<Tuple<string, string>> values = _stringExtractor.valuesUntilSearchedString(_pageList[startAndStopPositonAndTimeTable.startPageIndex()],
                         startAndStopPositonAndTimeTable.startIndex(),
                         "Page");
                    List<Tuple<string, string>> values2 = _stringExtractor.valuesFromSearchedString(_pageList[startAndStopPositonAndTimeTable.stopPageIndex()],                                             
                                             "Printed at",
                                             startAndStopPositonAndTimeTable.stopIndex());

                    List<Tuple<string, string>> allValues = new List<Tuple<string, string>>();
                    allValues.AddRange(values);
                    allValues.AddRange(values2);
                    liveCatheter.setPositonTimePairs(allValues);
                    liveCatheter.setCatheterNumber(counter);
                }
                else
                {
                    // Unhandled case
                }

                liveCatheters.Add(liveCatheter);
                ++counter;
            }

            return liveCatheters;
        }

    }
}
