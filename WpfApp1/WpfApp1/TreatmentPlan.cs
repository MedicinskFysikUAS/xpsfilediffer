using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

namespace WpfApp1
{
    public class TreatmentPlan
    {
        private List<List<string>> _pageList;
        private StringExtractor _stringExtractor = new StringExtractor();
        private TabType _tabType;
        private Dictionary<string, string> _catheterTochannel = new Dictionary<string, string>();
        private List<string> _catheterLengths = new List<string>();
        private List<string> _catheterWithNamLengths = new List<string>();
        private List<string> _catheterWithoutNamLengths = new List<string>();
        private Dictionary<string, string> _catheterAndTime = new Dictionary<string, string>();
        private List<IntrauterineCatheter> _intrauterineCatheters = new List<IntrauterineCatheter>();
        private IntrauterineApplicatorType _intrauterineApplicatorType;

        public IntrauterineApplicatorType IntrauterineApplicatorType { get => _intrauterineApplicatorType; set => _intrauterineApplicatorType = value; }

        public TreatmentPlan(List<List<string>> pageList, TabType tabType, IntrauterineApplicatorType intrauterineApplicatorType = IntrauterineApplicatorType.UNKNOWN)
        {
            _pageList = pageList;
            _tabType = tabType;
            _intrauterineApplicatorType = intrauterineApplicatorType;
        }


        public string patientFirstName()
        {
            int pageIndex = 0;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 0, true);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 11, 1);
            }
            else if (_tabType == TabType.INTRAUTERINE)
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
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Patient name:", 1, true);
            }
            else if (_tabType == TabType.CYLINDER)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 11, 0);
            }
            else if (_tabType == TabType.INTRAUTERINE)
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
            else if (_tabType == TabType.INTRAUTERINE)
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
            else if (_tabType == TabType.INTRAUTERINE)
            {
                stringValue = _stringExtractor.getValueAtIndex(_pageList[pageIndex], 1, 0);
            }
            return stringValue;
        }

        // For Oncentra Brachy is the plan status a png image and cannot be tested.
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
            else if (_tabType == TabType.INTRAUTERINE)
            {
                return cylindricStatusSetDateTime();
            }
            else
            {
                return "";
            }
        }

        public string calibrationDateTime()
        {
            if (_tabType == TabType.PROSTATE)
            {
                return prostateCalibrationDateTime();
            }
            else if (_tabType == TabType.CYLINDER)
            {
                return cylinderCalibrationDateTime();
            }
            else if (_tabType == TabType.INTRAUTERINE)
            {
                return cylinderCalibrationDateTime();
            }
            else
            {
                return "";
            }
        }

        string toStdDateStringInTreatmentPlan(string dateString)
        {
            string pattern1 = "HH:mm:ss, dd. MMMMM yyyy";
            string pattern2 = "dd MMM yyyy HH:mm:ss";
            string pattern3 = "yyyy-MM-dd HH:mm:ss";
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, pattern1, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern1, enUS, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern2, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern2, enUS, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern3, null, DateTimeStyles.None, out parsedDate) ||
                                      DateTime.TryParseExact(dateString, pattern3, enUS, DateTimeStyles.None, out parsedDate)
                                      )
            {
                return parsedDate.ToString(Constants.DATE_AND_TIME_FORMAT);
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
            string timeString = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Status set at:", 1);
            // string pattern3 = "yyyy-MM-dd HH:mm:ss";
            return toStdDateStringInTreatmentPlan(dateString + " " + timeString);
        }

        public string cylindricStatusSetDateTime()
        {
            int pageIndex = 0;
            string dateAndUserString = _pageList[pageIndex][3];
            int position = dateAndUserString.IndexOf("by otpuser");
            string dateString = dateAndUserString.Substring(0, position).Trim();
            return toStdDateStringInTreatmentPlan(dateString);
        }

        public string prostateCalibrationDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getStringAfterStartWithSearchString(_pageList[pageIndex], "Calibration Date/Time:");
            return toStdDateStringInTreatmentPlan(dateString);
        }

        public string cylinderCalibrationDateTime()
        {
            int pageIndex = 0;
            string dateString = _stringExtractor.getValueNStepAfterSearchString(_pageList[pageIndex], "Calibration date/time:", -1);
            return toStdDateStringInTreatmentPlan(dateString);
        }


        public string cylindricPlanName()
        {
            int pageIndex = 0;
            string stringValue = _pageList[pageIndex][5];
            return stringValue.Trim();
        }

        public bool planIsApproved()
        {
            return planStatus() == "APPROVED";
        }

        public string fractionDose()
        {
            int pageIndex = 2;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Prescribed Dose:", 0);
            }
            else if (_tabType == TabType.CYLINDER ||
                _tabType == TabType.INTRAUTERINE)
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
            int pageIndex = 2;
            string stringValue = "";
            if (_tabType == TabType.PROSTATE)
            {
                stringValue = _stringExtractor.getValueBetweenSearchStrings(_pageList[pageIndex], "Prescribed Dose:", "Gy");
            }
            else if (_tabType == TabType.CYLINDER)
            {
                pageIndex = 0;
                stringValue = _stringExtractor.getValueBeforeSearchString(_pageList[pageIndex], "Prescription dose per fraction/pulse (Gy):", pageIndex);
            }
            else if (_tabType == TabType.INTRAUTERINE)
            {
                pageIndex = 0;
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
                stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Treatment Source Strength:", 0);
            }
            else if (_tabType == TabType.CYLINDER ||
                _tabType == TabType.INTRAUTERINE)
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
            if (_tabType == TabType.CYLINDER ||
                _tabType == TabType.INTRAUTERINE)
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
            else if (_tabType == TabType.INTRAUTERINE)
            {
                pageIndex = 1;
                stringValue = _stringExtractor.getStringAfterStartWithSearchString(_pageList[pageIndex], "Total treatment time (sec.):");
                if (stringValue.Length == 0)
                {
                    stringValue = _stringExtractor.getStringAfterStartWithSearchString(_pageList[pageIndex + 1], "Total treatment time (sec.):");
                }
                if (stringValue.Length == 0)
                {
                    stringValue = _stringExtractor.getStringAfterStartWithSearchString(_pageList[pageIndex + 2], "Total treatment time (sec.):");
                }
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


        public List<LiveCatheter> liveCatheters(bool skipNoActivePositions = false)
        {
            if (_tabType == TabType.PROSTATE)
            {
                return prostateLiveCatheters().OrderBy(o => o.catheterNumber()).ToList();
            }
            else if (_tabType == TabType.CYLINDER)
            {
                return cylindricLiveCatheters().OrderBy(o => o.catheterNumber()).ToList();
            }
            else if (_tabType == TabType.INTRAUTERINE)
            {
                if (_intrauterineApplicatorType == IntrauterineApplicatorType.UNKNOWN)
                {
                    List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
                    return liveCatheters;
                }
                else
                {
                    setCatheterToChannelNumberAndLengths(_intrauterineApplicatorType);
                    if (_intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA ||
                        _intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS ||
                        _intrauterineApplicatorType == IntrauterineApplicatorType.VMIX ||
                        _intrauterineApplicatorType == IntrauterineApplicatorType.VMIX_M_MATRIS)
                    {
                        return intrauterineLiveCathetersVenezia(skipNoActivePositions).OrderBy(o => o.catheterNumber()).ToList();
                    }
                    else
                    {
                        return intrauterineLiveCatheters(skipNoActivePositions).OrderBy(o => o.catheterNumber()).ToList();
                    }
                }
            }
            else // Only added to get rid of build warnings.
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
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") - 8;
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
                    return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") - 8;
                }
            }
            else
            {
                return -1;
            }
        }

        public bool pageEndsWithContinued(List<string> page, int startIndex)
        {
            return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Offset (mm):") == -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Continued") != -1;
        }

    public int getIntrauterineCatheterTableEndIndex(List<string> page, int startIndex)
        {
            int nColumnsInPatientPointsTable = 6;
            int nStepsBack = nColumnsInPatientPointsTable - 1;
            if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Offset (mm):") != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Offset (mm):");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Continued") != -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Continued");
            }
            else if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "AP") != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "AP");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "AR") != -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "AR");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") != -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") == -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval");
            }
            else if (_stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "Signed for approval") == -1 &&
                _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") != -1)
            {
                return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") -
                    nStepsBack;
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
                    return _stringExtractor.getIndexOnPageForSearchedStringFromIndex(page, startIndex, "P1") -
                        nStepsBack;
                }
            }
            else
            {
                return -1;
            }
        }

        private int getIntrauterineSourcePositionsTableEndIndex(List<string> page, int startIndex)
        {
            if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Source position separation (mm):") != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Source position separation (mm):");
            } 
            else if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Signed for approval") != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Signed for approval");
            }
            {
                return -1;
            }

        }

        private int getIntrauterineCatheterTimesTableEndIndex(List<string> page, int startIndex)
        {
            if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Total treatment time") != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, "Total treatment time");
            }
            else
            {
                return -1;
            }

        }

        private int getIndexForSearchedString(List<string> page, int startIndex, string searchedString)
        {
            if (_stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, searchedString) != -1)
            {
                return _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, startIndex, searchedString);
            }
            else
            {
                return -1;
            }

        }

        public List<TreatmentPlanCatheter> treatmentPlanCatheters()
        {
            if (_tabType == TabType.PROSTATE)
            {
                return prostateTreatmentPlanCatheters();
            }
            else if (_tabType == TabType.CYLINDER)
            {
                return cylinderTreatmentPlanCatheters();
            }
            else
            {
                List <TreatmentPlanCatheter> emptyList = new List<TreatmentPlanCatheter>();
                return emptyList;
            }

        }

        public List<TreatmentPlanCatheter> cylinderTreatmentPlanCatheters()
        {
            // Only set the Selector that is the Chanell length in the cylinder protocol //before "Cathet..."
            TreatmentPlanCatheter treatmentPlanCatheter = new TreatmentPlanCatheter();
            int pageIndex = 1;
            //getValueBeforeSearchString(List<string> stringsOnPage, string searchedString, int stringIndex)
            string stringValue = _stringExtractor.getValueBeforeSearchString(_pageList[pageIndex], "Cathet...", 0);
            int position = stringValue.IndexOf("(mm)");
            treatmentPlanCatheter.selector = _stringExtractor.decimalStringToDecimal(stringValue.Substring(0, position).Trim());
            List<TreatmentPlanCatheter> treatmentPlanCatheters = new List<TreatmentPlanCatheter>();
            treatmentPlanCatheters.Add(treatmentPlanCatheter);
            return treatmentPlanCatheters;

        }

            public List<TreatmentPlanCatheter> prostateTreatmentPlanCatheters()
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

        public string applicatorName()
        {
            string applicatorName = "";
            int pageIndex = 1;
            if (_tabType == TabType.INTRAUTERINE)
            {
                applicatorName = _stringExtractor.getValueAfterSearchString(_pageList[pageIndex], "Applicator set:", 0);

            }
            return applicatorName;
        }

        public string applicatorStringFromApplicationType(IntrauterineApplicatorType applicatorType)
        {
            string applicatorString = "";
            if (applicatorType == IntrauterineApplicatorType.RINGAPPLIKATOR)
            {
                applicatorString = "R";
            }
            else if (applicatorType == IntrauterineApplicatorType.VENEZIA ||
                applicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS ||
                applicatorType == IntrauterineApplicatorType.VMIX ||
                applicatorType == IntrauterineApplicatorType.VMIX_M_MATRIS)
            {
                applicatorString = "V";
            }
            else if (applicatorType == IntrauterineApplicatorType.MCVC)
            {
                applicatorString = "MCVC";
            }
            return applicatorString;
        }

        public string intrauterinePlanName()
        {
            int pageIndex = 0;
            string stringValue = _pageList[pageIndex][5];
            return stringValue.Trim();
        }

        private void setCatheterToChannelNumberAndLengths(IntrauterineApplicatorType intrauterineApplicatorType)
        {
            if (intrauterineApplicatorType == IntrauterineApplicatorType.MCVC ||
                intrauterineApplicatorType == IntrauterineApplicatorType.RINGAPPLIKATOR ||
                intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS ||
                intrauterineApplicatorType == IntrauterineApplicatorType.VMIX_M_MATRIS)
            {
                setCatheterToChannelNumberAndLengthsMcvcRingVeneziaMMatris();
                return;
            }
            else if (intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA ||
                intrauterineApplicatorType == IntrauterineApplicatorType.VMIX)
            {
                setCatheterToChannelNumberAndLengthsVenezia();
                return;
            }
            else
            {
                return;
            }
        }

        private Tuple<int,int> getPosAndmmIndex(List<string> page)
        {
            int rowIndexCount = 0;
            int posIndex = -1;
            int mmIndex = -1;
            int itemsInPosRow = -1;
            foreach (var row in page)
            {
                if (row.Contains("(pos)"))
                {
                    posIndex = rowIndexCount;
                    itemsInPosRow = row.Count() - 1; // pos vs mm
                }
                if (row.Contains("mm") && 
                    row.Count() == itemsInPosRow)
                {
                    mmIndex = rowIndexCount;
                }
                ++rowIndexCount;
            }
            return Tuple.Create(posIndex, mmIndex);
        }

        private List<string> allSourcePositionsValues(List<string> page)
        {
            Tuple<int, int> posAndmmIndex = getPosAndmmIndex(page);
            return page.GetRange(posAndmmIndex.Item1, (posAndmmIndex.Item2 - posAndmmIndex.Item1));
        }

        private void setCatheterToChannelNumberAndLengthsVenezia()
        {
            _catheterTochannel.Clear();
            int startTableIndex = -1;
            int endTableIndex = -1;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1)
                {
                    int offsetIndex = _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, currentIndex, "Material info:");
                    if (offsetIndex != -1)
                    {
                        List<string> allValues = allSourcePositionsValues(page);
                        LiveCatheter liveCatheter = new LiveCatheter();
                        startTableIndex = offsetIndex;
                        endTableIndex = getIntrauterineSourcePositionsTableEndIndex(page, startTableIndex + 1);
                        List<string> catheterAndCannels = new List<string>();
                        _intrauterineCatheters.Clear();
                        int counter = 1;
                        for (int i = 0; i < allValues.Count; ++i)
                        {
                            if (allValues[i].Contains("(") &&
                                allValues[i].Contains(")"))
                            {
                                if (!allValues[i].Contains("mm"))
                                {
                                    catheterAndCannels.Add(allValues[i]);
                                }
                                else
                                {
                                    int from = 0;
                                    int to = allValues[i].IndexOf("(");
                                    string lengthString = allValues[i].Substring(from, to).Trim();
                                    _catheterLengths.Add(allValues[i].Substring(from, to).Trim());
                                    if (i == 0 || ((i + 4 < allValues.Count) && allValues[i + 4].Contains("mm")))
                                    {
                                        _catheterWithNamLengths.Add(lengthString);
                                        IntrauterineCatheter intrauterineCatheter = new IntrauterineCatheter();
                                        intrauterineCatheter.CatheterNumber = counter.ToString();
                                        intrauterineCatheter.IntrauterineCatheterType = IntrauterineCatheterType.MODEL;
                                        intrauterineCatheter.CatheterLength = _stringExtractor.decimalStringToDecimal(lengthString);
                                        _intrauterineCatheters.Add(intrauterineCatheter);
                                        counter++;
                                    }
                                    else
                                    {
                                        _catheterWithoutNamLengths.Add(lengthString);
                                        IntrauterineCatheter intrauterineCatheter = new IntrauterineCatheter();
                                        intrauterineCatheter.CatheterNumber = counter.ToString();
                                        intrauterineCatheter.IntrauterineCatheterType = IntrauterineCatheterType.MANUAL;
                                        intrauterineCatheter.CatheterLength = _stringExtractor.decimalStringToDecimal(lengthString);
                                        _intrauterineCatheters.Add(intrauterineCatheter);
                                        counter++;
                                    }

                                }
                            }
                        }

                        foreach (var catheterAndCannel in catheterAndCannels)
                        {
                            int from = 0;
                            int to = catheterAndCannel.IndexOf("(");
                            string catheterNumber = catheterAndCannel.Substring(from, to).Trim();
                            from = catheterAndCannel.IndexOf("(") + "(".Length;
                            to = catheterAndCannel.LastIndexOf(")");
                            string channelNumber = catheterAndCannel.Substring(from, to - from).Trim();
                            int n;
                            if (int.TryParse(catheterNumber, out n) &&
                                int.TryParse(channelNumber, out n))
                            {
                                if (!_catheterTochannel.ContainsKey(catheterNumber))
                                {
                                    _catheterTochannel.Add(catheterNumber, channelNumber);
                                }
                            }
                        }
                        currentIndex = -1;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }
            }
        }

        private void setCatheterToChannelNumberAndLengthsMcvcRingVeneziaMMatris()
        {
            _catheterTochannel.Clear();
            int startTableIndex = -1;
            int endTableIndex = -1;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1)
                {
                    int offsetIndex = _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, currentIndex, "Source position separation (mm):");
                    if (offsetIndex != -1)
                    {
                        LiveCatheter liveCatheter = new LiveCatheter();
                        startTableIndex = getIndexForSearchedString(page, 0, "|    |");
                        endTableIndex = offsetIndex - 2;
                        List<string> allValues = allSourcePositionsValues(page);
                        List<string> catheterAndCannels = new List<string>();
                        _intrauterineCatheters.Clear();
                        int counter = 1;
                        for (int i = 0; i < allValues.Count; ++i)
                        {
                            if (allValues[i].Contains("(") &&
                                allValues[i].Contains(")"))
                            {
                                if (!allValues[i].Contains("mm"))
                                {
                                    catheterAndCannels.Add(allValues[i]);
                                }
                                else
                                {
                                    int from = 0;
                                    int to = allValues[i].IndexOf("(");
                                    string lengthString = allValues[i].Substring(from, to).Trim();
                                    _catheterLengths.Add(allValues[i].Substring(from, to).Trim());
                                    if ((i == 0 && allValues[i + 1] != "VT")|| ((i + 4 < allValues.Count) && allValues[i + 4].Contains("mm")))
                                    {
                                        _catheterWithNamLengths.Add(lengthString);
                                        IntrauterineCatheter intrauterineCatheter = new IntrauterineCatheter();
                                        intrauterineCatheter.CatheterNumber = counter.ToString();
                                        intrauterineCatheter.IntrauterineCatheterType = IntrauterineCatheterType.MODEL;
                                        intrauterineCatheter.CatheterLength = _stringExtractor.decimalStringToDecimal(lengthString);
                                        _intrauterineCatheters.Add(intrauterineCatheter);
                                        counter++;
                                    }
                                    else
                                    {
                                        _catheterWithoutNamLengths.Add(lengthString);
                                        IntrauterineCatheter intrauterineCatheter = new IntrauterineCatheter();
                                        intrauterineCatheter.CatheterNumber = counter.ToString();
                                        intrauterineCatheter.IntrauterineCatheterType = IntrauterineCatheterType.MODEL;
                                        intrauterineCatheter.CatheterLength = _stringExtractor.decimalStringToDecimal(lengthString);
                                        _intrauterineCatheters.Add(intrauterineCatheter);
                                        counter++;
                                    }

                                }
                            }
                        }

                        foreach (var catheterAndCannel in catheterAndCannels)
                        {
                            int from = 0;
                            int to = catheterAndCannel.IndexOf("(");
                            string catheterNumber = catheterAndCannel.Substring(from, to).Trim();
                            from = catheterAndCannel.IndexOf("(") + "(".Length;
                            to = catheterAndCannel.LastIndexOf(")");
                            string channelNumber = catheterAndCannel.Substring(from, to - from).Trim();
                            int n;
                            if (int.TryParse(catheterNumber, out n) &&
                                int.TryParse(channelNumber, out n))
                            {
                                if (!_catheterTochannel.ContainsKey(catheterNumber))
                                {
                                    _catheterTochannel.Add(catheterNumber, channelNumber);
                                }
                            }
                        }
                        currentIndex = -1;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }
            }
        }
              
        public List<IntrauterineCatheter> intrauterineCatheters()
        {
            setCatheterToChannelNumberAndLengths(_intrauterineApplicatorType);
            return _intrauterineCatheters;
        }

        private Tuple<decimal, bool> getOffsetValueHasNoActivePair(string offsetString, int offsetIndex, List<string> page)
        {
            int startIndex = page[offsetIndex].IndexOf(':') + 1;
            int length = offsetString.Length - startIndex;
            string offsetValueString = offsetString.Substring(startIndex, length);
            bool hasNoActiveDwell = false;
            if (offsetValueString.Contains("No active dwell"))
            {
                hasNoActiveDwell = true;
                int newLengthIndex = offsetValueString.LastIndexOf("-") - 1;
                offsetValueString = offsetValueString.Substring(0, newLengthIndex).Trim();

            }
            decimal offsetValue = _stringExtractor.decimalStringToDecimal(offsetValueString);
            Tuple<decimal, bool> offsetValueHasNoActivePair = new Tuple<decimal, bool>(offsetValue, hasNoActiveDwell);
            return offsetValueHasNoActivePair;
        }


        private bool channelNumberIsPipe(int channelNumber)
        {
            return channelNumber == 1 ||
                channelNumber == 3 ||
                channelNumber == 5;
        }

        private decimal channelLengthFromCatheterNumber(int catheterNumber)
        {
            foreach (var item in _intrauterineCatheters)
            {
                int x = 0;
                Int32.TryParse(item.CatheterNumber, out x);
                if (Int32.TryParse(item.CatheterNumber, out x) &&
                    x == catheterNumber)
                {
                    return item.CatheterLength;
                }
            }

            return -1.0m;
        }

        public List<LiveCatheter> intrauterineLiveCatheters(bool skipNoActivePositions = false)
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            int startTableIndex = -1;
            int endTableIndex = -1;
            int catheterNumberCounter = 1;
            bool increaseCatheterNumberCounter = true;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1 && catheterNumberCounter < 200)
                {
                    int offsetIndex = _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, currentIndex, "Offset (mm):");
                    if (offsetIndex != -1)
                    {
                        string offsetString  = page[offsetIndex];
                        int startIndex = page[offsetIndex].IndexOf(':') + 1;
                        int length = offsetString.Length - startIndex;
                        string offsetValueString = offsetString.Substring(startIndex, length);
                        Tuple<decimal, bool> offsetValueHasNoActivePair = getOffsetValueHasNoActivePair(offsetString, offsetIndex, page);
                        decimal offsetValue = offsetValueHasNoActivePair.Item1;
                        startTableIndex = offsetIndex;
                        endTableIndex = getIntrauterineCatheterTableEndIndex(page, startTableIndex + 1);
                        if (endTableIndex != -1 && (page[endTableIndex] == "Continued"))
                        {
                            increaseCatheterNumberCounter = false;
                        }
                        if (endTableIndex != -1 && (page[endTableIndex].Contains("AR")))
                        {
                            endTableIndex = startTableIndex;
                        }
                        List<string> allValues = _stringExtractor.allValuesInInterval(page, startTableIndex, endTableIndex);
                        List<List<string>> catheterTableLines = _stringExtractor.nColumnsRowsInInterval(6, allValues);
                        List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
                        foreach (var catheterTableLine in catheterTableLines)
                        {
                            Tuple<string, string> tuple = new Tuple<string, string>(catheterTableLine[5], catheterTableLine[0]);
                            if (tuple.Item1 != "AR" &&
                                tuple.Item1 != "AL" &&
                                tuple.Item1 != "BR" &&
                                tuple.Item1 != "BL")
                            {
                                if (_stringExtractor.decimalStringToDecimal(catheterTableLine[0]) != -1 &&
                                (_stringExtractor.decimalStringToDecimal(catheterTableLine[5]) != -1))
                                {
                                    positonTimePairs.Add(tuple);
                                }
                            }
                        }
                        int channelNumber = -1;
                        if (_catheterTochannel.Count > 0 && _catheterTochannel.ContainsKey(catheterNumberCounter.ToString()))
                        {
                            bool conversionWasOk = Int32.TryParse(_catheterTochannel[catheterNumberCounter.ToString()], out channelNumber);
                        }
                        LiveCatheter liveCatheter = new LiveCatheter();
                        bool existingCatheter = false;
                        foreach (var item in liveCatheters)
                        {
                            if (item.catheterNumber() == channelNumber)
                            {
                                item.addPositonTimePairs(positonTimePairs);
                                existingCatheter = true;
                            }
                        }
                        if (!existingCatheter)
                        {
                            liveCatheter.IsPipe = channelNumberIsPipe(channelNumber);
                            liveCatheter.ChannelLength = channelLengthFromCatheterNumber(catheterNumberCounter);
                            liveCatheter.setCatheterNumber(channelNumber);
                            liveCatheter.setOffsetLength(offsetValue);
                            liveCatheter.setPositonTimePairs(positonTimePairs);
                            if (skipNoActivePositions)
                            {
                                if (positonTimePairs.Count != 0)
                                {
                                    liveCatheters.Add(liveCatheter);
                                }
                            }
                            else
                            {
                                liveCatheters.Add(liveCatheter);
                            }
                        }

                        if (increaseCatheterNumberCounter)
                        {
                            catheterNumberCounter++;
                        }
                        else
                        {
                            increaseCatheterNumberCounter = true;
                        }
                        currentIndex = endTableIndex;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }
            }
            return liveCatheters;
        }


        public List<LiveCatheter> intrauterineLiveCathetersVenezia(bool skipNoActivePositions = false)
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            int startTableIndex = -1;
            int endTableIndex = -1;
            int catheterNumberCounter = 1;
            foreach (var page in _pageList)
            {
                int currentIndex = 0;
                while (currentIndex != -1)
                {
                    int offsetIndex = _stringExtractor.getIndexOnPageForStartWithStringFromIndex(page, currentIndex, "Offset (mm):");
                    if (offsetIndex != -1)
                    {
                        string offsetString = page[offsetIndex];
                        int startIndex = page[offsetIndex].IndexOf(':') + 1;
                        int length = offsetString.Length - startIndex;
                        string offsetValueString = offsetString.Substring(startIndex, length);
                        Tuple<decimal, bool> offsetValueHasNoActivePair = getOffsetValueHasNoActivePair(offsetString, offsetIndex, page);
                        decimal offsetValue = offsetValueHasNoActivePair.Item1;
                        LiveCatheter liveCatheter = new LiveCatheter();
                        startTableIndex = offsetIndex;
                        endTableIndex = getIntrauterineCatheterTableEndIndex(page, startTableIndex + 1);
                        bool pageEndsWithCont = pageEndsWithContinued(page, startTableIndex + 1);
                        List<string> allValues = _stringExtractor.allValuesInInterval(page, startTableIndex, endTableIndex);
                        List<List<string>> catheterTableLines = _stringExtractor.nColumnsRowsInInterval(6, allValues);
                        List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
                        foreach (var catheterTableLine in catheterTableLines)
                        {
                            Tuple<string, string> tuple = new Tuple<string, string>(catheterTableLine[5], catheterTableLine[0]);
                            if (tuple.Item1 != "AR" &&
                                tuple.Item1 != "AL" &&
                                tuple.Item1 != "BR" &&
                                tuple.Item1 != "BL")
                            {
                                positonTimePairs.Add(tuple);
                            }
                        }
                        int channelNumber = -1;
                        if (_catheterTochannel.ContainsKey(catheterNumberCounter.ToString()))
                        {
                            bool conversionWasOk = Int32.TryParse(_catheterTochannel[catheterNumberCounter.ToString()], out channelNumber);
                        }
                        liveCatheter.IsPipe = channelNumberIsPipe(channelNumber);
                        liveCatheter.ChannelLength = channelLengthFromCatheterNumber(catheterNumberCounter);
                        liveCatheter.setCatheterNumber(channelNumber);
                        liveCatheter.setOffsetLength(offsetValue);
                        liveCatheter.setPositonTimePairs(positonTimePairs);
                        if (skipNoActivePositions)
                        {
                            if (positonTimePairs.Count != 0 &&
                                liveCatheter.ChannelLength != -1)
                            {
                                liveCatheters.Add(liveCatheter);
                            }
                        }
                        else
                        {
                            if (liveCatheter.ChannelLength != -1)
                            {
                                liveCatheters.Add(liveCatheter);
                            }
                        }
                        if (!pageEndsWithCont)
                        {
                            catheterNumberCounter++;
                        }
                        currentIndex = endTableIndex;
                    }
                    else
                    {
                        currentIndex = -1;
                    }
                }
            }
            List<LiveCatheter> mergedLiveCatheters = new List<LiveCatheter>();
            for (int i = 0; i < liveCatheters.Count; i++)
            {
                if (i < liveCatheters.Count - 1 &&
                    liveCatheters[i].catheterNumber() == liveCatheters[i + 1].catheterNumber())
                {
                    LiveCatheter liveCatheter = new LiveCatheter();
                    liveCatheter = liveCatheters[i];
                    liveCatheter.appendPositionTimePairs(liveCatheters[i + 1].positonTimePairs());
                    mergedLiveCatheters.Add(liveCatheter);
                    i++;
                }
                else
                {
                    mergedLiveCatheters.Add(liveCatheters[i]);
                }
            }

            return mergedLiveCatheters;
        }
    }


} // Namespace
