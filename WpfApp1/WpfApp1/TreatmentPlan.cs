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
        private TabType _tabType;

        public TreatmentPlan(List<List<string>> pageList, TabType tabType)
        {
            _pageList = pageList;
            _tabType = tabType;
        }


        public string patientFirstName()
        {
            int pageIndex = 0;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 11, 1);
            }

            return stringValue;
        }

        public string patientLastName()
        {
            int pageIndex = 0;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 1);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 11, 0);
            }
            return stringValue;
        }

        public string patientId()
        {
            int pageIndex = 0;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient ID:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 10, 0);
            }
            return stringValue;
        }

        public string planCode()
        {
            int pageIndex = 0;

            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Plan Code:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 1, 0);
            }
            return stringValue;
        }

        // For Oncentra Brach is the plan status a png image and cannot be tested.
        public string planStatus()
        {
            int pageIndex = 0;
            return _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Plan status:", 0);
        }

        public string statusSetDateTime()
        {
            if (_tabType == TabType.PROSTATE)
            {
                return prostateStatusSetDateTime();
            }
            else if (_tabType == TabType.CYLINDER)
            {
                return cylindricStatusSetDateTime();
            }
            else
            {
                return "";
            }

        }

            public string prostateStatusSetDateTime()
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

        public string cylindricStatusSetDateTime()
        {
            int pageIndex = 0;
            string dateAndUserString = _pageList[pageIndex][3];
            int position = dateAndUserString.IndexOf("by otpuser");
            string dateString = dateAndUserString.Substring(0, position).Trim();
            string pattern = "dd MMM yyyy HH:mm:ss";
            string stringFromDateTime = "";
            DateTime result = DateTime.ParseExact(dateString, pattern, CultureInfo.InvariantCulture);
            if (result != null)
            {
                stringFromDateTime = result.ToString("yyyy-MM-dd HH:mm");
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

            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Prescribed Dose:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                pageIndex = 0;
                stringValue = _stringExtractor.getValueBeforeSearchString(_pageList[pageIndex], "Prescription dose per fraction/pulse (Gy):", pageIndex);
            }

            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
            }
            Decimal fractiondoseFl = -1m;
            if (stringValue.Length > 0)
            {
                fractiondoseFl = Convert.ToDecimal(stringValue);
            }
            return String.Format("{0:0.00}", Convert.ToDecimal(fractiondoseFl));
        }

        public decimal PrescribedDose()
        {
            int pageIndex = 1;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueBetweenSearchStrings(_pageList[pageIndex], "Prescribed Dose:", "Gy");
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueBeforeSearchString(_pageList[pageIndex], "Prescription dose per fraction/pulse (Gy):", pageIndex);
            }

            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
            }
            if (stringValue.Length == 0)
            {
                stringValue = "-1";
            }
            return Convert.ToDecimal(stringValue);
        }

        public string plannedSourceStrengthStrValue()
        {
            int pageIndex = 0;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Planned Source Strength:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "and treatment date and time (days):", pageIndex);
            }
            return stringValue;
        }

            public decimal plannedSourceStrength()
        {
            string stringValue = plannedSourceStrengthStrValue();
            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
            }
            if (stringValue.Length == 0)
            {
                stringValue = "-1";
            }
            decimal decimalValue = Convert.ToDecimal(stringValue);
            if (_tabType == TabType.CYLINDER)
            {
                decimalValue *= 1000.0m;
            }
            return decimalValue;
        }

        public decimal plannedSourceStrengthValue()
        {
            string stringValue = plannedSourceStrengthStrValue();

            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
            }
            if (stringValue.Length == 0)
            {
                stringValue = "-1";
            }
            return Convert.ToDecimal(stringValue);
        }


        public string totalTreatmentTimeStrValue()
        {
            int pageIndex = 1;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Total Treatment Time:", 0);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                pageIndex = 1;
                stringValue = _stringExtractor.getStringAfterStartWithSearchString(_pageList[pageIndex], "Total treatment time (sec.):");
            }
            return stringValue;
        }

        public string totalTreatmentTime()
        {
            string totalTreatmentTimeStr = totalTreatmentTimeStrValue();
            if (totalTreatmentTimeStr.Contains('.'))
            {
                totalTreatmentTimeStr = totalTreatmentTimeStr.Replace('.', ',');
            }
            if (totalTreatmentTimeStr.Length == 0)
            {
                totalTreatmentTimeStr = "-1";
            }
            Decimal totalTreatmentTime = Convert.ToDecimal(totalTreatmentTimeStr);
            Decimal totalTreatmentTimeOneDec = Math.Round(totalTreatmentTime, 1);
            return String.Format("{0:0.0}", Convert.ToDecimal(totalTreatmentTimeOneDec));
        }

        public decimal totalTreatmentTimeValue()
        {
            string totalTreatmentTimeStr = totalTreatmentTimeStrValue();
            if (totalTreatmentTimeStr.Contains('.'))
            {
                totalTreatmentTimeStr = totalTreatmentTimeStr.Replace('.', ',');
            }
            if (totalTreatmentTimeStr.Length == 0)
            {
                totalTreatmentTimeStr = "-1";
            }
            return Convert.ToDecimal(totalTreatmentTimeStr);
        }


        public List<LiveCatheter> liveCatheters()
        {
            if (_tabType == TabType.PROSTATE)
            {
                return prostateLiveCatheters();
            }
            else if (_tabType == TabType.CYLINDER)
            {
                return cylindricLiveCatheters();
            }
            else
            {
                List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
                return liveCatheters;
            }
        }

            public List<LiveCatheter> prostateLiveCatheters()
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

        public bool isCatheterTableHeader(string line)
        {
            return line == "locked";
        }

        public bool isLineAfterLastCatheterTable(string line)
        {
            return line.StartsWith("Page") && line.Contains("of") ||
                line == "Sources";
        }

        public int catheterNumberFromLine(string line)
        {
            return 1;
        }

        //public TreatmentPlanCatheter catheterFromLine(string line)
        //{
        //    TreatmentPlanCatheter catheter = new TreatmentPlanCatheter();
        //    catheter.catheterNumber = catheterNumberFromLine(line);
        //    catheter.offset = "";
        //    string tmp = catheter.offset;
        //    return catheter;
        //}

        public int getCatheterTableEndIndex(List<string> page, int startIndex)
        {
            if (_stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of") != -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Sources") == -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of");
            }
            else if (_stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of") == -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Sources") != -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Source");
            }
            else if (_stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of") != -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Sources") != -1)
            {
                if (_stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of") <
                    _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Sources"))
                {
                    return _stringExtractor.getIndexOnPageForStartWithSearchedStringFromIndex(page, startIndex, "Page", "of");
                }
                else
                {
                    return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Sources");
                }
            }
            else
            {
                return -1;
            }


        }

        public int getCylindricCatheterTableEndIndex(List<string> page, int startIndex)
        {
            if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") != -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") == -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") == -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") != -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Source");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") != -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") != -1)
            {
                if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") <
                    _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1"))
                {
                    return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval");
                }
                else
                {
                    return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1");
                }
            }
            else
            {
                return -1;
            }


        }

        public List<TreatmentPlanCatheter> treatmentPlanCatheters()
        {
            List<TreatmentPlanCatheter> catheters = new List<TreatmentPlanCatheter>();
            int startTableIndex = -1;
            int endTableIndex = -1;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1)
                {
                    int timesStringIndex = _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, currentIndex, "Times");
                    int lockedStringIndex = -1;
                    if (timesStringIndex != -1)
                    {
                        lockedStringIndex = _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, timesStringIndex, "locked");

                    }
                    if (timesStringIndex != -1 && lockedStringIndex != -1 && lockedStringIndex == timesStringIndex + 1)
                    {
                        startTableIndex = lockedStringIndex;
                        endTableIndex = getCatheterTableEndIndex(page, currentIndex);
                        List<string> allValues = _stringExtractor.allValuesInInterval(page, startTableIndex, endTableIndex);
                        List<List<string>> catheterTableLines = _stringExtractor.nColumnsRowsInInterval(10, allValues);
                        foreach (var catheterTableLine in catheterTableLines)
                        {
                            TreatmentPlanCatheter treatmentPlanCatheter = new TreatmentPlanCatheter();
                            treatmentPlanCatheter.catheterNumber = _stringExtractor.catheterNumberFromString(catheterTableLine[0]);
                            treatmentPlanCatheter.selector = _stringExtractor.decimalStringToDecimal(catheterTableLine[3]);
                            treatmentPlanCatheter.depth = _stringExtractor.decimalStringToDecimal(catheterTableLine[4]);
                            treatmentPlanCatheter.freeLength = _stringExtractor.decimalStringToDecimal(catheterTableLine[5]);
                            treatmentPlanCatheter.offset = _stringExtractor.decimalStringToDecimal(catheterTableLine[6]);
                            treatmentPlanCatheter.tipField = _stringExtractor.decimalStringToDecimal(catheterTableLine[7]);
                            treatmentPlanCatheter.isActiveLocked = _stringExtractor.isYesString(catheterTableLine[8]);
                            treatmentPlanCatheter.isTimeLocked = !_stringExtractor.isNoString(catheterTableLine[9]);
                            catheters.Add(treatmentPlanCatheter);
                        }
                        currentIndex = endTableIndex;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }


                //foreach (var line in page)
                //{
                //    if (isLineAfterLastCatheterTable(line))
                //    {
                //        startRead = false;
                //    }
                //    if (startRead)
                //    {
                //        catheters.Add(catheterFromLine(line));
                //    }
                //    if (isCatheterTableHeader(line))
                //    {
                //        startRead = true;
                //    }
                //}
            }
            return catheters;
        }


        public List<LiveCatheter> cylindricLiveCatheters()
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            LiveCatheter liveCatheter = new LiveCatheter();
            liveCatheter.setCatheterNumber(1); // Assume that only one catheter is used.
            List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
            int startTableIndex = -1;
            int endTableIndex = -1;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1)
                {
                    int offsetIndex = _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, currentIndex, "Offset (mm):");
                    if (offsetIndex != -1)
                    {
                        startTableIndex = offsetIndex;
                        endTableIndex = getCylindricCatheterTableEndIndex(page, currentIndex);
                        List<string> allValues = _stringExtractor.allValuesInInterval(page, startTableIndex, endTableIndex);
                        List<List<string>> catheterTableLines = _stringExtractor.nColumnsRowsInInterval(6, allValues);
                        foreach (var catheterTableLine in catheterTableLines)
                        {
                            Tuple<string, string> tuple = new Tuple<string, string>(catheterTableLine[5], catheterTableLine[0]);
                            positonTimePairs.Add(tuple);
                        }
                        currentIndex = endTableIndex;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }
            }
            liveCatheter.setPositonTimePairs(positonTimePairs);
            liveCatheters.Add(liveCatheter);
            return liveCatheters;
        }


    }
}
