using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApp1
{
    public class Comparator
    {
        private TreatmentPlan _treatmentPlan;
        private TccPlan _tccPlan;
        private TreatmentDvh _treatmentDvh;
        private Specifications _specifications;

        public Comparator(Specifications specifications)
        {
            _specifications = specifications;
        }

        public TreatmentPlan treatmentPlan { get => _treatmentPlan; set => _treatmentPlan = value; }
        public TccPlan tccPlan { get => _tccPlan; set => _tccPlan = value; }

        public TreatmentDvh treatmentDvh { get => _treatmentDvh; set => _treatmentDvh = value; }

        // has --------------------------


        // TODO: Add check of dvh patients name
        public bool hasSamePatientName()
        {
            if (_treatmentPlan.patientFirstName() == "" || _tccPlan.patientFirstName() == "" ||
                _treatmentPlan.patientLastName() == "" || _tccPlan.patientLastName() == "")
            {
                return false;
            }
            return (_treatmentPlan.patientFirstName() == _tccPlan.patientFirstName()) && (_treatmentPlan.patientLastName() == _tccPlan.patientLastName());
        }

        public bool hasSamePatientId()
        {
            if (_treatmentPlan.patientId() == "" || _tccPlan.patientId() == "")
            {
                return false;
            }

            return (_treatmentPlan.patientId() == _tccPlan.patientId());
        }

        public bool hasSamePlanCode()
        {
            if (_treatmentPlan.planCode() == "" || _tccPlan.planCode() == "")
            {
                return false;
            }
            return (_treatmentPlan.planCode() == _tccPlan.planCode());
        }

        public bool hasApprovedStatus()
        {
            return (_treatmentPlan.planIsApproved() && _tccPlan.planIsApproved());
        }

        public bool tccPlanHasApprovedStatus()
        {
            return (_tccPlan.planIsApproved());
        }


        public bool hasSameStatusSetDateTime()
        {
            if (_treatmentPlan.statusSetDateTime() == "" || _tccPlan.statusSetDateTime() == "")
            {
                return false;
            }
            return (_treatmentPlan.statusSetDateTime() == _tccPlan.statusSetDateTime());
        }

        public bool hasSameCalibrationDateTime()
        {
            if (_treatmentPlan.calibrationDateTime() == "" || _tccPlan.calibrationDateTime() == "")
            {
                return false;
            }
            StringExtractor stringExtractor = new StringExtractor();
            TimeSpan duration = stringExtractor.stringToDateTime(_treatmentPlan.calibrationDateTime()) -
                stringExtractor.stringToDateTime(_tccPlan.calibrationDateTime());
            return Math.Abs(duration.TotalHours) <= 1;
        }

        public bool hasSameFractionDose()
        {
            if (_treatmentPlan.fractionDose() == "" || _tccPlan.fractionDose() == "")
            {
                return false;
            }

            return (_treatmentPlan.fractionDose() == _tccPlan.fractionDose());
        }

        public bool hasSamePlannedSourceStrength(decimal airKermaStrengthEpsilon)
        {
            return (Math.Abs((_treatmentPlan.plannedSourceStrength()) - _tccPlan.plannedSourceStrength()) < airKermaStrengthEpsilon);
        }

        bool hasSameTotalTreatmentTime()
        {
            if (_treatmentPlan.totalTreatmentTime() == "" || _tccPlan.totalTreatmentTime() == "")
            {
                return false;
            }
            return (_treatmentPlan.totalTreatmentTime() == _tccPlan.totalTreatmentTime());
        }

        bool hasSameTotalTreatmentTimeValue(decimal totalTreatmentTimeEpsilon)
        {
            if (_treatmentPlan.totalTreatmentTime() == "" || _tccPlan.totalTreatmentTime() == "")
            {
                return false;
            }
            return (Math.Abs(_treatmentPlan.totalTreatmentTimeValue() - _tccPlan.totalTreatmentTimeValue()) < totalTreatmentTimeEpsilon);
        }


        decimal plannedToRelizationAKStrengthQuota()
        {
            return _tccPlan.plannedSourceStrength() /
            calculatedRealizedAKStrength();
        }

        decimal decayCorrectedValue(decimal inputValue)
        {
            StringExtractor stringExtractor = new StringExtractor();
            Calculator calculator = new Calculator();
            return inputValue *
               (calculator.decayFactor(stringExtractor.stringToDateTime(_treatmentPlan.calibrationDateTime()),
               stringExtractor.stringToDateTime(_treatmentPlan.statusSetDateTime())) /
               calculator.decayFactor(stringExtractor.stringToDateTime(_treatmentPlan.calibrationDateTime()),
               stringExtractor.stringToDateTime(_tccPlan.realizationDateAndTime())));
        }

        bool hasCorrectRealizedTotalTreatmentTime(decimal totalTimeEpsilon)
        {
            decimal correctedRealizedTotalTime = _treatmentPlan.totalTreatmentTimeValue() * plannedToRelizationAKStrengthQuota();
            if (_tccPlan.realizedTotalTreatmentTime() < 0 || (correctedRealizedTotalTime < 0))
            {
                return false;
            }
            return ((Math.Abs(correctedRealizedTotalTime - _tccPlan.realizedTotalTreatmentTime()) /
                correctedRealizedTotalTime) * 100 < totalTimeEpsilon);
        }

        bool hasCorrectRealizedAKStrength(decimal AKStrengthEpsilon)
        {
            decimal calcRealizedAKStrength = calculatedRealizedAKStrength();
            if (calcRealizedAKStrength == -1.0m)
            {
                return false;
            }
            if ((Math.Abs(calcRealizedAKStrength - _tccPlan.realizedSourceStrength()) / calcRealizedAKStrength) * 100 < AKStrengthEpsilon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Tuple<List<LiveCatheter>, List<LiveCatheter>> makeCorrectedTpLiveCathetersAboveThreshold(List<LiveCatheter> tpLiveCatheters, List<LiveCatheter> tccLiveCatheters)
        {
            List<LiveCatheter> liveCathetersAboveThreshold = new List<LiveCatheter>();
            ErrorCode errorCode = new ErrorCode();
            StringExtractor stringExtractor = new StringExtractor();
            int counter = 0;
            foreach (var item in tpLiveCatheters)
            {
                List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
                int subCounter = 0;
                foreach (var subItem in item.positonTimePairs())
                {
                    decimal correctedTime = Math.Round(stringExtractor.decimalStringToDecimal(subItem.Item2) * plannedToRelizationAKStrengthQuota(), 2);
                    if (correctedTime > Constants.TIME_THRESHOLD)
                    {
                        positonTimePairs.Add(new Tuple<string, string>(subItem.Item1, stringExtractor.decimalToTwoDecimalString(correctedTime)));
                    }
                    else if (correctedTime == Constants.TIME_THRESHOLD && tccLiveCatheters.Count > counter)
                    {
                        foreach (var tccPositionTimePair in tccLiveCatheters[counter].positonTimePairs())
                        {
                            if ((stringExtractor.decimalStringToDecimal(subItem.Item1) -
                                stringExtractor.decimalStringToDecimal(tccPositionTimePair.Item1)) == 0.0m)
                            {
                                positonTimePairs.Add(new Tuple<string, string>(subItem.Item1, stringExtractor.decimalToTwoDecimalString(correctedTime)));
                                break;
                            }
                        }
                    }
                    ++subCounter;
                }
                LiveCatheter liveCatheter = new LiveCatheter();
                liveCatheter.setCatheterNumber(item.catheterNumber());
                liveCatheter.setPositonTimePairs(positonTimePairs);
                liveCathetersAboveThreshold.Add(liveCatheter);
                ++counter;
            }
            Tuple<List<LiveCatheter>, List<LiveCatheter>> tuple = new Tuple<List<LiveCatheter>, List<LiveCatheter>>(liveCathetersAboveThreshold, tccLiveCatheters);
            return tuple;
        }

        ErrorCode hasSameCatheterPositionTimePairs(decimal timeEpsilon, bool useRelativeEpsilon)
        {
            Tuple<List<LiveCatheter>, List<LiveCatheter>> tuple = makeCorrectedTpLiveCathetersAboveThreshold(_treatmentPlan.liveCatheters(true), _tccPlan.liveCatheters());
            List<LiveCatheter> tpLiveCatheters = tuple.Item1;
            List<LiveCatheter> tccLiveCatheters = tuple.Item2;
            ErrorCode errorCode = new ErrorCode();
            if (tpLiveCatheters.Count != tccLiveCatheters.Count)
            {
                errorCode.Number = -100;
                errorCode.Description = "Olika antal katetrar i dosplan och TCC plan." +
                        " I dosplan: " + tpLiveCatheters.Count +
                        " i TCC plan: " + tccLiveCatheters.Count; ;
                return errorCode;
            }
            var tpAndTccCatheters = tpLiveCatheters.Zip(tccLiveCatheters, (tp, tcc) => new { tpLiveCatheters = tp, tccLiveCatheters = tcc });
            foreach (var tpAndTccCatheter in tpAndTccCatheters)
            {
                if (tpAndTccCatheter.tpLiveCatheters.catheterNumber() != tpAndTccCatheter.tccLiveCatheters.catheterNumber())
                {
                    errorCode.Number = -101;
                    errorCode.Description = "Kateter nummer är inte det samma." +
                        " I dosplan: " + tpAndTccCatheter.tpLiveCatheters.catheterNumber() +
                        " i TCC plan: " + tpAndTccCatheter.tccLiveCatheters.catheterNumber();
                    return errorCode;
                }
                else
                {
                    List<Tuple<string, string>> tpPositonTimePairs = tpAndTccCatheter.tpLiveCatheters.positonTimePairs();
                    List<Tuple<string, string>> tccPositonTimePairs = tpAndTccCatheter.tccLiveCatheters.positonTimePairs();
                    if (tpPositonTimePairs.Count != tccPositonTimePairs.Count)
                    {
                        errorCode.Number = -102;
                        errorCode.Description = "Antalet positioner och tider för kateter nr " + tpAndTccCatheter.tpLiveCatheters.catheterNumber() +
                            " är olika." +
                            " Antalet (över tröskelvärdet på " + Constants.TIME_THRESHOLD + " sek) i dosplan: " + tpPositonTimePairs.Count +
                            ". Antalet i TCC plan: " + tccPositonTimePairs.Count;
                        return errorCode;
                    }
                    var tpAndTccPositonTimePairs = tpPositonTimePairs.Zip(
                       tccPositonTimePairs, (tpTimePairs, tccTimePairs) => new {
                           tpPositonTimePairs = tpTimePairs,
                           tccPositonTimePairs = tccTimePairs
                       });

                    StringExtractor stringExtractor = new StringExtractor();

                    foreach (var tpAndTccPositonTimePair in tpAndTccPositonTimePairs)
                    {
                        if (stringExtractor.decimalStringToDecimal(tpAndTccPositonTimePair.tpPositonTimePairs.Item1) -
                            stringExtractor.decimalStringToDecimal(tpAndTccPositonTimePair.tccPositonTimePairs.Item1) != 0.0m)
                        {
                            errorCode.Number = -103;
                            errorCode.Description = "Positionen för kateter nr " + tpAndTccCatheter.tpLiveCatheters.catheterNumber() +
                                " är olika." +
                            " I dosplan: " + tpAndTccPositonTimePair.tpPositonTimePairs.Item1 +
                            " i TCC plan: " + tpAndTccPositonTimePair.tccPositonTimePairs.Item1;
                            return errorCode;
                        }

                        decimal tpTime = stringExtractor.decimalStringToDecimal(tpAndTccPositonTimePair.tpPositonTimePairs.Item2);
                        decimal deltaTime = Math.Abs(tpTime - stringExtractor.decimalStringToDecimal(tpAndTccPositonTimePair.tccPositonTimePairs.Item2));
                        if (useRelativeEpsilon)
                        {
                            deltaTime = (deltaTime / tpTime) * 100.0m;
                        }

                        if (deltaTime > timeEpsilon)
                        {
                            errorCode.Number = -103;
                            errorCode.Description = "Tiden för kateter nr " + tpAndTccCatheter.tpLiveCatheters.catheterNumber() +
                                " är olika." +
                            " I dosplan (korrigerad): " + tpAndTccPositonTimePair.tpPositonTimePairs.Item2 +
                            " i TCC plan: " + tpAndTccPositonTimePair.tccPositonTimePairs.Item2 +
                            " för position " + tpAndTccPositonTimePair.tpPositonTimePairs.Item1;
                            return errorCode;
                        }
                    }
                }
            }
            errorCode.Number = 0;
            errorCode.Description = "Alla positioner och tider är lika";
            return errorCode;
        }

        public bool treatmentPlanHasSameChannelLength(decimal channelLength)
        {
            if (_treatmentPlan.treatmentPlanCatheters().Count == 0)
            {
                return false;
            }
            foreach (var catheter in _treatmentPlan.treatmentPlanCatheters())
            {
                if (catheter.selector != channelLength)
                {
                    return false;
                }
            }
            return true;
        }

        private List<string> toDotSeparated(List<string> commaSeparated)
        {
            List<string> dotSeparated = new List<string>();
            foreach (var item in commaSeparated)
            {
                dotSeparated.Add(item.Replace(',', '.'));
            }
            return dotSeparated;
        }

        private List<Tuple<int, decimal>> getNumberAndLengthFromLiveCatheters(List<LiveCatheter> liveCatheters)
        {
            List<Tuple<int, decimal>> numberAndLengthFromLiveCatheters = new List<Tuple<int, decimal>>();
            foreach (var item in liveCatheters)
            {
                numberAndLengthFromLiveCatheters.Add(new Tuple<int, decimal>(item.catheterNumber(), item.ChannelLength));
            }
            return numberAndLengthFromLiveCatheters;
        }

        private bool treatmentPlanTccHasSameChannelLengths()
        {
            List<Tuple<int, decimal>> tccChannelNumberAndLengths = _tccPlan.getChannelNumberAndLengths();
            List<Tuple<int, decimal>> tpsChannelNumberAndLengths = getNumberAndLengthFromLiveCatheters(_treatmentPlan.liveCatheters());
            bool foundAll = true;
            if (tccChannelNumberAndLengths.Count == 0 || tpsChannelNumberAndLengths.Count == 0)
            {
                foundAll = false;
            }
            foreach (var tccItem in tccChannelNumberAndLengths)
            {
                bool foundItem = false;
                foreach (var tpsItem in tpsChannelNumberAndLengths)
                {
                    if (tccItem.Item1 == tpsItem.Item1 && tccItem.Item2 == tpsItem.Item2)
                    {
                        foundItem = true;
                        break;
                    }
                }
                if (!foundItem)
                {
                    foundAll = false;
                    break;
                }
            }
            return foundAll;
        }

        public bool treatmentPlanHasExpectedDepth(decimal expectedDepth, decimal needleDepthEpsilon)
        {
            foreach (var catheter in _treatmentPlan.treatmentPlanCatheters())
            {
                if (Math.Abs(catheter.depth - expectedDepth) > needleDepthEpsilon)
                {
                    return false;
                }
            }
            return true;
        }

        int treatmentCatheterNumberWithWrongDepth(decimal expectedDepth, decimal needleDepthEpsilon)
        {
            foreach (var catheter in _treatmentPlan.treatmentPlanCatheters())
            {
                if (Math.Abs(catheter.depth - expectedDepth) > needleDepthEpsilon)
                {
                    return catheter.catheterNumber;
                }
            }
            return -1;
        }

        public bool treatmentPlanHasExpectedFreeLength(decimal expectedFreeLength, decimal freeLengthEpsilon)
        {
            foreach (var catheter in _treatmentPlan.treatmentPlanCatheters())
            {
                if (Math.Abs(catheter.freeLength - expectedFreeLength) > freeLengthEpsilon)
                {
                    return false;
                }
            }
            return true;
        }

        public decimal treatmentTimeDeviation(decimal estimatedTreatmentTime, decimal reportedTreatmentTime)
        {
            return Math.Abs((estimatedTreatmentTime - reportedTreatmentTime) / reportedTreatmentTime);
        }

        public bool treatmentTimeAsEstimated(decimal estimatedTreatmentTime, decimal reportedTreatmentTime, decimal treatmentTimeEpsilon)
        {
            return treatmentTimeDeviation(estimatedTreatmentTime, reportedTreatmentTime) < treatmentTimeEpsilon;
        }


        public bool prescriptionDoseIsTheSame(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose, decimal dvhPrescriptionDose, decimal tccPrescriptionDose)
        {
            return ((guiPresciptionDose == treatmentPlanPrescriptionDose) &&
                (treatmentPlanPrescriptionDose == dvhPrescriptionDose) &&
                (treatmentPlanPrescriptionDose == tccPrescriptionDose));
        }

        public bool guiPlanTccPresciptionDoseIsTheSame(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose, decimal tccPrescriptionDose)
        {
            return ((guiPresciptionDose == treatmentPlanPrescriptionDose) &&
                (treatmentPlanPrescriptionDose == tccPrescriptionDose));
        }

        public bool guiPlanTccPlanCodeIsTheSame(string guiPlanCode, string treatmentPlanCode, string tccPlanCode)
        {
            return ((guiPlanCode.ToUpper().Trim() == treatmentPlanCode.ToUpper()) &&
                (treatmentPlanCode.ToUpper() == tccPlanCode.ToUpper()));
        }

        decimal calculatedRealizedAKStrength()
        {
            if (_tccPlan.calibrationDateTime() == "" | _tccPlan.realizationDateAndTime() == "")
            {
                return -1.0m;
            }
            Calculator calculator = new Calculator();
            StringExtractor stringExtractor = new StringExtractor();
            return _tccPlan.calibratedSourceStrength() *
                calculator.decayFactor(
                    stringExtractor.stringToDateTime(_tccPlan.calibrationDateTime()),
                    stringExtractor.stringToDateTime(_tccPlan.realizationDateAndTime()));

        }

        private bool channelLengthInIntrauterinePlanIsOk()
        {
            List<LiveCatheter> liveCatheters = _treatmentPlan.liveCatheters();
            bool isOk = true;
            if (liveCatheters.Count == 0)
            {
                isOk = false;
            }

            foreach (var item in liveCatheters)
            {
                if (item.IsPipe)
                {
                    if (Math.Abs(item.ChannelLength - _specifications.ExpectedLengthPipeCatheter) >
                        _specifications.ExpectedLengthPipeCatheterEpsilon)
                    {
                        isOk = false;
                        break;
                    }
                }
                else
                {
                    if (item.catheterNumber() < 30)
                    {

                        if (Math.Abs(item.ChannelLength - _specifications.ExpectedLengthNonPipeCatheter) >
                            _specifications.ExpectedLengthPipeCatheterEpsilon)
                        {
                            isOk = false;
                            break;
                        }
                    }
                    else
                    {
                        if (Math.Abs(item.ChannelLength - _specifications.ExpectedLengthNonPipeCatheterAbove29) >
                           _specifications.ExpectedLengthPipeCatheterEpsilon)
                        {
                            isOk = false;
                            break;
                        }
                    }
                }
            }
            return isOk;
        }

        private bool offsetLengthInPlanIsOk()
        {
            List<LiveCatheter> liveCatheters = _treatmentPlan.liveCatheters();
            List<IntrauterineCatheter> intrauterineCatheters = _treatmentPlan.intrauterineCatheters();
            if (liveCatheters.Count != intrauterineCatheters.Count)
            {
                return false;
            }

            for (int i = 0; i < liveCatheters.Count; i++)
            {
                if (liveCatheters[i].IsPipe)
                {
                    if (Math.Abs(liveCatheters[i].offsetLength() - 0.0m) > 0.001m)
                    {
                        return false;
                    }
                }
                else if (_treatmentPlan.IntrauterineApplicatorType == IntrauterineApplicatorType.MCVC)
                {
                    if (Math.Abs(liveCatheters[i].offsetLength() - 0.0m) > 0.001m)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Math.Abs(liveCatheters[i].offsetLength() + 6.0m) > 0.001m)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private decimal lastSourcePositionInPlan()
        {
            StringExtractor stringExtractor = new StringExtractor();
            List<LiveCatheter> liveCatheters = _treatmentPlan.liveCatheters();
            decimal lastSourcePosition = -1.0m;
            if (liveCatheters.Count == 1)
            {
                lastSourcePosition = stringExtractor.decimalStringToDecimal(liveCatheters.First().positonTimePairs().First().Item1);
            }
            return lastSourcePosition;
        }

        private decimal firstSourcePositionInPlan()
        {
            StringExtractor stringExtractor = new StringExtractor();
            List<LiveCatheter> liveCatheters = _treatmentPlan.liveCatheters();
            decimal lastSourcePosition = -1.0m;
            if (liveCatheters.Count == 1)
            {
                lastSourcePosition = stringExtractor.decimalStringToDecimal(liveCatheters.First().positonTimePairs().Last().Item1);
            }
            return lastSourcePosition;
        }

        public decimal activeLengthInPlan()
        {
            return lastSourcePositionInPlan() - firstSourcePositionInPlan();
        }

        private decimal indexerLengthPositionTableInPlan()
        {
            decimal indexerLength = -1;
            decimal lastSourcePosition = lastSourcePositionInPlan();
            if (lastSourcePosition != -1)
            {
                StringExtractor stringExtractor = new StringExtractor();
                indexerLength = _specifications.LengthOfCathetersUsedForEsofagus +
                        lastSourcePosition;
            }
            return indexerLength;
        }

        private decimal lastSourcePositionInTcc()
        {
            StringExtractor stringExtractor = new StringExtractor();
            List<LiveCatheter> liveCatheters = _tccPlan.liveCatheters();
            decimal lastSourcePosition = -1.0m;
            if (liveCatheters.Count == 1)
            {
                lastSourcePosition = stringExtractor.decimalStringToDecimal(liveCatheters.First().positonTimePairs().First().Item1);
            }
            return lastSourcePosition;
        }

        private decimal firstSourcePositionInTcc()
        {
            StringExtractor stringExtractor = new StringExtractor();
            List<LiveCatheter> liveCatheters = _tccPlan.liveCatheters();
            decimal lastSourcePosition = -1.0m;
            if (liveCatheters.Count == 1)
            {
                lastSourcePosition = stringExtractor.decimalStringToDecimal(liveCatheters.First().positonTimePairs().Last().Item1);
            }
            return lastSourcePosition;
        }

        public decimal activeLengthInTcc()
        {
            return lastSourcePositionInTcc() - firstSourcePositionInTcc();
        }

        private decimal indexerLengthPositionTableInTcc()
        {
            decimal indexerLength = -1;
            decimal lastSourcePosition = lastSourcePositionInTcc();
            if (lastSourcePosition != -1)
            {
                StringExtractor stringExtractor = new StringExtractor();
                indexerLength = _specifications.LengthOfCathetersUsedForEsofagus +
                        lastSourcePosition;
            }
            return indexerLength;
        }

        // check -----------------------

        public List<string> checkPatientName()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma patientnamn");
            if (hasSamePatientName())
            {
                resultRow.Add("OK");
                
            }
            else
            {
                resultRow.Add("Inte OK");
                
            }
            resultRow.Add("Patientnamn i plan: " + _treatmentPlan.patientFirstName() + " " + _treatmentPlan.patientLastName() +
                " i TCC: " + _tccPlan.patientFirstName() + " " + _tccPlan.patientLastName());

            return resultRow;
        }

        public List<string> checkPatientId()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma personnummer");
            if (hasSamePatientId())
            {
                resultRow.Add("OK");
                
            }
            else
            {
                resultRow.Add("Inte OK");
                
            }
            resultRow.Add("Personnummer i plan: " + _treatmentPlan.patientId() +
                " i TCC: " + _tccPlan.patientId());

            return resultRow;
        }

        public List<string> checkPlanCode()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma plankod");
            if (hasSamePlanCode())
            {
                resultRow.Add("OK");
                
            }
            else
            {
                resultRow.Add("Inte OK");
                
            }
            resultRow.Add("Plankod i plan: " + _treatmentPlan.planCode() +
                " i TCC: " + _tccPlan.planCode());

            return resultRow;
        }


        public List<string> checkApproval(bool skipApprovalTest = false)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Planerna är godkända");
            if (!skipApprovalTest)
            {
                if (hasApprovedStatus())
                {
                    resultRow.Add("OK");
                }
                else
                {
                    resultRow.Add("Inte OK");
                }
                string TPStatus = "Inte godkänd";
                if (_treatmentPlan.planIsApproved())
                {
                    TPStatus = "Godkänd";
                }
                string TCCStatus = "Inte godkänd";
                if (_tccPlan.planIsApproved())
                {
                    TCCStatus = "Godkänd";
                }

                resultRow.Add("Dosplanen är : " + TPStatus +
                    " TCC-planen är : " + TCCStatus);
            }
            else
            {
                resultRow.Add(" ");
                string description = "Dosplanens status kan inte testas. TCC-planen är ";
                if (tccPlanHasApprovedStatus())
                {
                    description += "godkänd.";
                }
                else
                {
                    description += "INTE godkänd.";
                }
                resultRow.Add(description);
            }


            return resultRow;
        }

        public List<string> checkApproveDateTime()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Planens godkännandetidpunkt");
            string info = "";
            if (hasSameStatusSetDateTime())
            {
                resultRow.Add("OK");
                
                info += "samma";
            }
            else
            {
                resultRow.Add("Inte OK");
                info += "inte samma";
            }
            info += ". Dosplanens godkännande: " + _treatmentPlan.statusSetDateTime() +
                ", TCC planens godkännande: " + _tccPlan.statusSetDateTime();
            resultRow.Add("Tiden för godkännande i plan och TCC är " + info);

            return resultRow;
        }

        public List<string> checkCalibrationDateTime(bool sameSource)
        {
            List<string> resultRow = new List<string>();
            string description = "";
            if (sameSource)
            {
                description = "Samma kalibreringstidpunkt";
            }
            else
            {
                description = "Olika kalibreringstidpunkt";

            }
            resultRow.Add(description);
            string info = "";
            if (hasSameCalibrationDateTime())
            {
                if (sameSource)
                {
                    resultRow.Add("OK");
                }
                else
                {
                    resultRow.Add("Inte OK");
                }
                info += "samma.";
            }
            else
            {
                if (!sameSource)
                {
                    resultRow.Add("OK");

                }
                else
                {
                    resultRow.Add("Inte OK");
                }
                info += "inte samma.";
            }
            info += " Dosplanens kalibreringsdatum: " + _treatmentPlan.calibrationDateTime() +
                " TCC planens kalibreringsdatum: " + _tccPlan.calibrationDateTime() + 
                " . Tiden kan ibland avvika exakt 1 h.";
            resultRow.Add("Kalibreringstidpunkten i plan och TCC är " + info);

            return resultRow;
        }

        public List<string> checkFractionDose()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Fraktionsdos");
            if (hasSameFractionDose())
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add("Fraktionsdos i plan: " + _treatmentPlan.fractionDose() +
                " i TCC: " + _tccPlan.fractionDose());

            return resultRow;
        }

        public List<string> checkPlannedSourceStrength(decimal airKermaStrengthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Dosplanens källstyrka");
            if (hasSamePlannedSourceStrength(airKermaStrengthEpsilon))
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add("Planned Source Strength i plan: " + _treatmentPlan.plannedSourceStrength() +
                " planerad AK-styrka i TCC: " + _tccPlan.plannedSourceStrength() + " ( +- " + airKermaStrengthEpsilon + " )");

            return resultRow;
        }


        public List<string> checkTotalTreatmentTime(decimal totalTreatmentTimeEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Dosplanens behandlingstid");
            if (hasSameTotalTreatmentTimeValue(totalTreatmentTimeEpsilon))
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add("Behandlingstid i plan: " + _treatmentPlan.totalTreatmentTime() +
                " och i TCC: " + _tccPlan.totalTreatmentTime() + " tolerans: " + totalTreatmentTimeEpsilon);

            return resultRow;
        }

      

        public List<string> checkRealizedAKStrength(decimal AKStrengthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Realiserade AK Strength");
            if (hasCorrectRealizedAKStrength(AKStrengthEpsilon))
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add("Beräknad realiserad AK Strength: " + Math.Round(calculatedRealizedAKStrength(), 2) +
                " och i TCC: " + _tccPlan.realizedSourceStrength() + " (tolerans:  " + AKStrengthEpsilon + " % ).");
            return resultRow;
        }
        public List<string> checkRealizedTotalTreatmentTime(decimal totalTimeEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("TCC realiserade behandlingstid");
            if (hasCorrectRealizedTotalTreatmentTime(totalTimeEpsilon))
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add("Beräknad realiserad behandlingstid utifrån plan: " + 
                Math.Round(_treatmentPlan.totalTreatmentTimeValue() * plannedToRelizationAKStrengthQuota(), 2) +
                " och i TCC: " + _tccPlan.realizedTotalTreatmentTime() + " (tolerans:  " + totalTimeEpsilon + " % ).");
            return resultRow;
        }

        public List<string> checkCatheterPositionTimePairs(decimal timeEpsilon, bool useRelativeEpsilon = false)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Bestrålningsposition och tid");
            string descriptionString = "";
            ErrorCode errorCode = hasSameCatheterPositionTimePairs(timeEpsilon, useRelativeEpsilon);
            if (errorCode.Number == 0)
            {
                resultRow.Add("OK");
                descriptionString = errorCode.Description;
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = errorCode.Description;
            }

            string toleranceUnit = "sek";
            if (useRelativeEpsilon)
            {
                toleranceUnit = "%";
            }
            descriptionString += "  (tolerans: " + timeEpsilon + " " + toleranceUnit + "). Tider från dosplan har korrigerats för sönderfall.";
            resultRow.Add(descriptionString);

            return resultRow;
        }

        public List<string> checkTreatmentPlanChannelLength(decimal expectedChannelLength)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Channel length i dosplan = " + expectedChannelLength);
            string descriptionString = "";
            if (treatmentPlanHasSameChannelLength(expectedChannelLength))
            {
                resultRow.Add("OK");
                descriptionString = "Channel length för samtliga kanaler är " + expectedChannelLength;
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Channel length är inte lika med " + expectedChannelLength + " för en eller fler kanaler.";
            }
            resultRow.Add(descriptionString);

            return resultRow;
        }


        public List<string> headerResultRow(string description)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add(" ");
            resultRow.Add(" " + description + " ");
            resultRow.Add("");
            return resultRow;
        }

        public List<string> errorResultRow(string testName, string desciption)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add(testName);
            resultRow.Add("Inte OK");
            resultRow.Add(desciption);
            return resultRow;
        }

        public List<string> informationResultRow(string testColumnString, string resultColumnString, string descriptonColumnString)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add(testColumnString);
            resultRow.Add(resultColumnString);
            resultRow.Add(descriptonColumnString);
            return resultRow;

        }

        public List<string> checkTreatmentPlanDepth(decimal expectedDepth, decimal needleDepthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma nåldjup i dosplan");
            string descriptionString = "";
            if (treatmentPlanHasExpectedDepth(expectedDepth, needleDepthEpsilon))
            {
                resultRow.Add("OK");
                descriptionString = "Nåldjupet är " + expectedDepth + " mm (inom " + needleDepthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Nåldjupet avviker från det förväntade djupet på " + expectedDepth + " mm mer än " + needleDepthEpsilon + " mm.";
                int catheterNumber = treatmentCatheterNumberWithWrongDepth(expectedDepth, needleDepthEpsilon);
                if (catheterNumber != -1)
                {
                    descriptionString += " Detta gäller kateter nr: " + catheterNumber + ".";
                }
            }
            resultRow.Add(descriptionString);

            return resultRow;
        }

        public List<string> checkTreatmentFreeLength(decimal expectedFreeLength, decimal needleDepthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma free length i dosplan");
            string descriptionString = "";
            if (treatmentPlanHasExpectedFreeLength(expectedFreeLength, needleDepthEpsilon))
            {
                resultRow.Add("OK");
                descriptionString = "Free length är " + expectedFreeLength + " (inom " + needleDepthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Free length avviker från " + expectedFreeLength + " mer än " + needleDepthEpsilon + " mm.";
            }
            resultRow.Add(descriptionString);

            return resultRow;
        }

        public List<string> checkTreatmentTime(decimal estimatedTreatmentTime, decimal reportedTreatmentTime, decimal treatmentTimeEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Uppskattning av behandlingstid");
            string descriptionString = "";
            decimal precentageEpsilon = treatmentTimeEpsilon * 100.0m;
            if (treatmentTimeAsEstimated(estimatedTreatmentTime, reportedTreatmentTime, treatmentTimeEpsilon))
            {
                resultRow.Add("OK");
                descriptionString = "Avvikelsen mellan uppskattad och rapporterad tid är mindre än " + precentageEpsilon.ToString("0") + "% " + " (" +
                    estimatedTreatmentTime.ToString("0.0") + " resp " + reportedTreatmentTime.ToString("0.0") + " sek)";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Avvikelsen mellan uppskattad och rapporterad tid är mer än " + precentageEpsilon.ToString("0") + "% " + " (" +
                     estimatedTreatmentTime.ToString("0.0") + " resp " + reportedTreatmentTime.ToString("0.0") + " sek)";
            }
            resultRow.Add(descriptionString);
            return resultRow;

        }

        public List<string> checkPlanAndDvhPlanCode(string dvhPlanCode, string treatmentPlanPlanCode)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Plankod i DVH & Plan");
            string descriptionString = "";
            if (dvhPlanCode.ToUpper().Trim() == treatmentPlanPlanCode.ToUpper().Trim())
            {
                resultRow.Add("OK");
                descriptionString = "Samma plankod i DVH och Plan.";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "INTE Samma plankod i DVH och Plan. Plankod i DVH: " + dvhPlanCode +
                    " i Plan: " + treatmentPlanPlanCode;
            }
            resultRow.Add(descriptionString);
            return resultRow;
        }


        public List<string> checkPresciptionDose(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose, decimal dvhPrescriptionDose, 
            decimal tccPrescriptionDose)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Ordinerad dos i plan, dvh & tcc");
            string descriptionString = "";
            if (prescriptionDoseIsTheSame(guiPresciptionDose, treatmentPlanPrescriptionDose, dvhPrescriptionDose, tccPrescriptionDose))
            {
                resultRow.Add("OK");
                descriptionString = "Den angivna ordinerade dosen är den samma som i dosplanen, DVH och TCC planen";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Den angivna ordinerade dosen är INTE den samma som i dosplanen, dvh och TCC planen";
            }
            descriptionString += ". I dosplan: " + treatmentPlanPrescriptionDose.ToString("0.00") + 
                " i dvh: " + dvhPrescriptionDose.ToString("0.00") + " TCC: " + tccPrescriptionDose.ToString("0.00");
            resultRow.Add(descriptionString);
            return resultRow;
        }
        public List<string> checkGuiPlanTccPresciptionDose(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose, 
           decimal tccPrescriptionDose)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Ordinerad dos i plan & tcc");
            string descriptionString = "";
            if (guiPlanTccPresciptionDoseIsTheSame(guiPresciptionDose, treatmentPlanPrescriptionDose, tccPrescriptionDose))
            {
                resultRow.Add("OK");
                descriptionString = "Den angivna ordinerade dosen är den samma som i dosplanen och TCC planen";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Den angivna ordinerade dosen är INTE den samma som i dosplanen och TCC planen";
            }
            descriptionString += ". I dosplan: " + treatmentPlanPrescriptionDose.ToString("0.00") +
                " TCC: " + tccPrescriptionDose.ToString("0.00");
            resultRow.Add(descriptionString);
            return resultRow;
        }

        public List<string> checkGuiPlanTccPlanCode(string guiPlanCode, string treatmentPlanCode,
          string tccPlanCode)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Angiven plankod");
            string descriptionString = "";
            if (guiPlanTccPlanCodeIsTheSame(guiPlanCode, treatmentPlanCode, tccPlanCode))
            {
                resultRow.Add("OK");
                descriptionString = "Den angivna plankoden den samma som i planen och TCC planen";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Den angivna plankoden är INTE den samma som i dosplanen och TCC planen";
            }
            descriptionString += ". I dosplan: " + treatmentPlanCode +
                " TCC: " + tccPlanCode;
            resultRow.Add(descriptionString);
            return resultRow;
        }

        public List<string> checkGuiPlanTccPlanCodeCylinder(string guiPlanCode, string treatmentPlanCode,
          string tccPlanCode)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Angiven plankod");
            string descriptionString = "";
            if (guiPlanTccPlanCodeIsTheSame(guiPlanCode, treatmentPlanCode, tccPlanCode))
            {
                resultRow.Add("OK");
                descriptionString = "Den angivna plankoden den samma som i planen och TCC planen";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Den angivna plankoden är INTE den samma som i dosplanen och TCC planen";
            }
            descriptionString += ". I dosplan: " + treatmentPlanCode +
                " TCC: " + tccPlanCode;
            resultRow.Add(descriptionString);
            return resultRow;
        }

        public List<string> checkGuiPlanTccPresciptionDoseylinder(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose,
          decimal tccPrescriptionDose)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Ordinerad dos i plan & tcc");
            string descriptionString = "";
            if (guiPlanTccPresciptionDoseIsTheSame(guiPresciptionDose, treatmentPlanPrescriptionDose, tccPrescriptionDose))
            {
                resultRow.Add("OK");
                descriptionString = "Den angivna ordinerade dosen är den samma som i dosplanen och TCC planen";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Den angivna ordinerade dosen är INTE den samma som i dosplanen och TCC planen";
            }
            descriptionString += ". I dosplan: " + treatmentPlanPrescriptionDose.ToString("0.00") +
                " TCC: " + tccPrescriptionDose.ToString("0.00");
            resultRow.Add(descriptionString);
            return resultRow;
        }

        public List<string> checkPlanName(string expectedPlanName)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Plannamn");
            string descriptionString = "";
            descriptionString = "Det förväntade plannamnet är: " + expectedPlanName + " dosplanens plannamn är " + _treatmentPlan.cylindricPlanName();
            if (expectedPlanName == _treatmentPlan.cylindricPlanName())
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add(descriptionString);
            return resultRow;
        }

        public List<string> checkIntrauterinePlanCatheterLengths()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Kanallängd i dosplan");
            bool resultOK = channelLengthInIntrauterinePlanIsOk();
            string expectedValueString = "de förväntade längderna på " + _specifications.ExpectedLengthPipeCatheter +
                " respektive " + _specifications.ExpectedLengthNonPipeCatheter;
            if (_treatmentPlan.IntrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS)
            {
                expectedValueString += " respektive " + _specifications.ExpectedLengthNonPipeCatheterAbove29;
            }
            expectedValueString += " mm.";
            string descriptionString = resultOK ? "Ring/IU-rör respektive ringnålar har " + expectedValueString :
                "Ring/IU-rör respektive ringnålar har inte " + expectedValueString;
            if (resultOK)
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add(descriptionString);
            return resultRow;
        }

        private List<string> checkIntrauterineOffsetLengths()
        {
            List<string> resultRow = new List<string>();
            bool result = offsetLengthInPlanIsOk();
            resultRow.Add("Offsetlängder i dosplan");
            string resultString = result ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = result ? "Alla nålar har korrekt offset." : "Alla nålar har inte korrekt offset.";
            resultRow.Add(description);
            return resultRow;
        }

        private List<string> checkIntrauterineTreatmenPlanTccPlanChannelLengths()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Kanallängd i dosplan vs tcc");
            string descriptionString = "";
            if (treatmentPlanTccHasSameChannelLengths())
            {
                resultRow.Add("OK");
                descriptionString = "Kanallängderna i dosplan och TCC plan är lika för samtliga kanaler.";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Kanallängderna i dosplan och TCC plan är INTE lika.";
            }
            resultRow.Add(descriptionString);
            return resultRow;
        }


        private string getAppicationTypeString(IntrauterineApplicatorType intrauterineApplicatorType)
        {
            string appicationTypeString = "";
            if (intrauterineApplicatorType == IntrauterineApplicatorType.MCVC)
            {
                appicationTypeString = "MCVC";
            }
            else if (intrauterineApplicatorType == IntrauterineApplicatorType.RINGAPPLIKATOR)
            {
                appicationTypeString = "Ring";
            }
            else if (intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA ||
                intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS)
            {
                appicationTypeString = "Venezia";
            }
            else if (intrauterineApplicatorType == IntrauterineApplicatorType.VMIX ||
                intrauterineApplicatorType == IntrauterineApplicatorType.VMIX_M_MATRIS)
            {
                appicationTypeString = "Vmix";
            }
            return appicationTypeString;
        }

        public List<string> checkSelectedApplicatorTypeAndDiamterWithApplicatorName()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Vald applikator och diameter");
            string treatmentPlanApplicatorName = _treatmentPlan.applicatorName();
            List<string> strings = treatmentPlanApplicatorName.Split(' ').ToList();
            string selectedApplicationTypeString = getAppicationTypeString(_specifications.IntrauterineApplicatorType);
            string expectedFirstCharacter = _treatmentPlan.applicatorStringFromApplicationType(_specifications.IntrauterineApplicatorType);
            string selectedDiameter = _specifications.ApplicatorDiameter.ToString();
            string selectedDiameterNr2 = _specifications.ApplicatorDiameterNr2.ToString();
            string searchedDiameterString = expectedFirstCharacter + selectedDiameter;
            string searchedDiameterStringNr2 = expectedFirstCharacter + selectedDiameter + selectedDiameterNr2;
            bool applicatorNameIsOk = false;
            bool applicatorDiameterIsOk = false;
            bool applicatorDiameterNr2IsOk = false;
            bool isVmix = false;
            foreach (var item in strings)
            {
                if (item.Contains(selectedApplicationTypeString))
                {
                    applicatorNameIsOk = true;
                    if (selectedApplicationTypeString  == "Vmix")
                    {
                        isVmix = true;
                    }
                }
                if (item.Contains(searchedDiameterString))
                {
                    applicatorDiameterIsOk = true;
                }
                if (isVmix)
                {
                    if (item != "Vmix" && 
                        item.ToUpper().StartsWith("V") &&
                        item.Length > 4)
                    {
                        string tmp = item.Substring(3, item.Length - 3);
                        if (item.Substring(3, item.Length - 3).Contains(selectedDiameterNr2))
                        {
                            applicatorDiameterNr2IsOk = true;
                        }   
                    }
                }
            }
            string description = "";
            description += applicatorNameIsOk ? "Vald applikator stämmer med planen. " :
            "Vald applikator stämmer inte med planen. ";
            description += applicatorDiameterIsOk ? "Vald diameter stämmer med planen. " :
            "Vald diameter stämmer inte med planen. ";
            if (isVmix)
            {
                description += applicatorDiameterNr2IsOk ? "Vald 2:a diameter stämmer med planen. " :
                "Vald 2:a diameter stämmer inte med planen. ";
            }
            if ((applicatorNameIsOk && applicatorDiameterIsOk) &&
                (isVmix == applicatorDiameterNr2IsOk))
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkPlanNameIntrauterine()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Plannamn");
            string treatmentPlanName = _treatmentPlan.intrauterinePlanName();
            List<string> strings = treatmentPlanName.Split(' ').ToList();
            string expectedFirstCharacter = _treatmentPlan.applicatorStringFromApplicationType(_specifications.IntrauterineApplicatorType);
            string userInputDiameter = _specifications.ApplicatorDiameter.ToString();
            string searchedString = expectedFirstCharacter + userInputDiameter;
            bool applicatorNameAndDiameterIsOk = false;
            foreach (var item in strings)
            {
                if (item.Contains(searchedString))
                {
                    applicatorNameAndDiameterIsOk = true;
                }
            }
            string description = "";
            description += applicatorNameAndDiameterIsOk ? "Vald applikator och diameter stämmer med plannamnet. " :
            "Vald applikator och diameter stämmer inte med plannamnet. ";
            if (applicatorNameAndDiameterIsOk)
            {
                resultRow.Add("OK");
            }
            else
            {
                resultRow.Add("Inte OK");
            }
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkApplicatorTypeIsSet(bool applicatorTypeIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Applikatortyp är angiven");
            string resultString = applicatorTypeIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = applicatorTypeIsSet ? "Applikatorstyp är angiven" : "Applikatorstyp är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkApplicatorDiameterIsSet(bool applicatordiameterIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Applikatorsdiameter är angiven");
            string resultString = applicatordiameterIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = applicatordiameterIsSet ? "Applikatorsdiameter är angiven" : "Applikatorsdiameter är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkApplicatorDiameterNr2IsSet(bool applicatordiameterNrIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Andra applikatorsdiametern är angiven");
            string resultString = applicatordiameterNrIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = applicatordiameterNrIsSet ? "Den andra applikatorsdiameter är angiven" :
                "Den andra applikatorsdiameter är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkFractionDoseIsSet(bool fractionDoseIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Ordinerad dos är angiven");
            string resultString = fractionDoseIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = fractionDoseIsSet ? "Ordinerad dos är angiven" : "Ordinerad dos är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkPlanCodeIsSet(bool sameSourceIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Plankod är angiven");
            string resultString = sameSourceIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = sameSourceIsSet ? "Plankod är angiven" : "Plankod är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkSameSourceIsSet(bool sameSourceIsSet)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma källa är angiven");
            string resultString = sameSourceIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = sameSourceIsSet ? "Samma källa är angiven" : "Samma källa är inte angiven";
            resultRow.Add(description);
            return resultRow;
        }

        public List<string> checkEsofagusUserInputIsSet(bool esofagusUserInputIsSet, 
            string checkDescription, string passDescription, string failDescription)
        {

            List<string> resultRow = new List<string>();
            resultRow.Add(checkDescription);
            string resultString = esofagusUserInputIsSet ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            string description = esofagusUserInputIsSet ? passDescription : failDescription;
            resultRow.Add(description);
            return resultRow;

        }

        public List<string> checkEsofagusIndexerLengthInputAndPlan(UserInputEsofagus userInputEsofagus)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Angiven indexer length och i plan");
            Calculator calculator = new Calculator();
            bool resultOK = treatmentPlanHasSameChannelLength(
                calculator.indexerLengthFromUserInput(_specifications, userInputEsofagus));
            string resultString = resultOK ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            resultString = resultOK ? "Den angiven Indexer Length och Indexer Length i plan är lika. " :
                "Den angiven Indexer Length och Indexer Length i plan INTE är lika. ";
            resultRow.Add(resultString);
            return resultRow;
        }

        public List<string> checkEsofagusIndexerLengthInputPlanAndTcc(UserInputEsofagus userInputEsofagus)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Indexer length i tabeller i plan och TCC");
            Calculator calculator = new Calculator();
            decimal indexerLengthFromInput = calculator.indexerLengthFromUserInput(_specifications, userInputEsofagus);
            decimal indexerLengthInPlan = indexerLengthPositionTableInPlan();
            decimal indexerLengthInTcc = indexerLengthPositionTableInTcc();
            string indexerLengthInfo = "Angiven: " + indexerLengthFromInput.ToString();
            indexerLengthInfo += " I plan: " + indexerLengthInPlan.ToString();
            indexerLengthInfo += " I TCC: " + indexerLengthInTcc.ToString();
            bool resultOK = Math.Abs(indexerLengthFromInput - indexerLengthInPlan) < _specifications.FreeLengthEpsilon &&
                Math.Abs(indexerLengthFromInput - indexerLengthInTcc) < _specifications.FreeLengthEpsilon;
            string resultString = resultOK ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            resultString = resultOK ? "Angiven Indexer Length och i tabeller i plan och TCC är lika." :
                "Angiven Indexer Length och i tabeller i plan och TCC är INTE lika.";
            resultString += indexerLengthInfo;
            resultRow.Add(resultString);
            return resultRow;
        }
        //        resultOK? resultRow.Add("OK"): resultRow.Add("Inte OK");
        //            //checkTreatmentPlanChannelLength(decimal expectedChannelLength)
        //            if (treatmentPlanHasSameChannelLength(expectedChannelLength))

        //                StringExtractor stringExtractor = new StringExtractor();
        //        string tmp = userInputEsofagus.InactiveLengthString;
        //        decimal indexerLengthFromUserInput = _specifications.MaxChannelLengthEsofagus -
        //            stringExtractor.decimalStringToDecimal(userInputEsofagus.InactiveLengthString);
        //            if (_treatmentPlan.esofagusPlanCathteters().Count == 1)
        //            {
        //                decimal channelLengthInPlan = _treatmentPlan.treatmentPlanCatheters()[0].selector;
        //        List<LiveCatheter> liveCatheters = _treatmentPlan.liveCatheters();
        //                if (liveCatheters.Count == 1)
        //                {
        //                    decimal indexerLengthFromPlan = _specifications.LengthOfCathetersUsedForEsofagus +
        //                        stringExtractor.decimalStringToDecimal(liveCatheters.First().positonTimePairs().First().Item1);
        //    }
        //}
        //            if (_tccPlan.getChannelNumberAndLengths().Count == 1)
        //            {
        //                decimal channelLengthInTcc = _tccPlan.getChannelNumberAndLengths()[0].Item2;
        //List<LiveCatheter> tccLiveCatheters = _tccPlan.liveCatheters();
        //                if (tccLiveCatheters.Count == 1)
        //                {
        //                    decimal indexerLengthFromTcc = _specifications.LengthOfCathetersUsedForEsofagus +
        //                        stringExtractor.decimalStringToDecimal(tccLiveCatheters.First().positonTimePairs().First().Item1);
        //                }

        //            }
        //            resultRow.Add("Indexer length fraktion 1");
        //            resultRow.Add("Indexer length fraktion 1");


        public List<string> checkActiveLengthInputPlanAndTcc(UserInputEsofagus userInputEsofagus, Tuple<decimal, decimal> planTccActiveLength)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Aktiv längd length i tabeller i plan och TCC");
            StringExtractor stringExtractor = new StringExtractor();
            decimal activeLengthFromInput = stringExtractor.decimalStringToDecimal(userInputEsofagus.ActiveLengthString);
            decimal activeLengthPlan = activeLengthInPlan();
            decimal activengthTcc = activeLengthInTcc();
            string activeLengthInfo = "Angiven: " + activeLengthFromInput.ToString();
            activeLengthInfo += " I plan: " + activeLengthPlan.ToString();
            activeLengthInfo += " I TCC: " + activengthTcc.ToString();
            bool resultOK = Math.Abs(activeLengthFromInput - activeLengthPlan) < _specifications.FreeLengthEpsilon &&
                Math.Abs(activeLengthFromInput - activengthTcc) < _specifications.FreeLengthEpsilon;
            string resultString = resultOK ? "OK" : "Inte OK";
            resultRow.Add(resultString);
            resultString = resultOK ? "Den aktiva längden och i tabeller i plan och TCC är lika med den angivna längden. " :
                "Den aktiva längden och i tabeller i plan och TCC är INTE lika med den angivna längden. ";
            resultString += activeLengthInfo;
            resultRow.Add(resultString);
            if (planTccActiveLength.Item1 != -1.0m &&
                planTccActiveLength.Item2 != -1.0m)
            {
                bool resultOKFractionX = (Math.Abs(activeLengthFromInput - planTccActiveLength.Item1) < _specifications.FreeLengthEpsilon) &&
                    (Math.Abs(activeLengthFromInput - planTccActiveLength.Item2) < _specifications.FreeLengthEpsilon);
                bool resultAllOK = resultOKFractionX && resultOK;
                resultRow[1] = resultAllOK ? "OK" : "Inte OK";
                resultRow[2] = resultAllOK ? resultRow[2] + " Den aktiva är längden lika med den första fraktionen" :
                    resultRow[2] + " Den aktiva är längden INTE lika med den första fraktionen";

            }
            return resultRow;
        }

            public List<List<string>> intrauterineTreatmentPlanAndTccPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkIntrauterineTreatmenPlanTccPlanChannelLengths());
            return resultRows;

        }
        public List<List<string>> treatmentPlanAndDvhResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & DVH"));
            Calculator calculator = new Calculator();
            decimal estimatedTreatmentTime = calculator.estimatedTreatmentTime(
                _treatmentDvh.PtvVolume(), _treatmentDvh.PrescribedDose(), _treatmentPlan.plannedSourceStrengthValue());
            decimal reportedTreatmentTime = _treatmentPlan.totalTreatmentTimeValue();

            resultRows.Add(checkTreatmentTime(estimatedTreatmentTime, reportedTreatmentTime, _specifications.TreatmentTimeEpsilon));
            resultRows.Add(checkPlanAndDvhPlanCode(_treatmentDvh.planCode(), treatmentPlan.planCode()));
            return resultRows;
        }

        public List<List<string>> prostateTreatmentPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan"));
            resultRows.Add(checkTreatmentPlanChannelLength(_specifications.ExpectedChannelLength));
            resultRows.Add(checkTreatmentPlanDepth(_specifications.NeedleDepth, _specifications.NeedleDepthEpsilon)); 
            resultRows.Add(checkTreatmentFreeLength(_specifications.FreeLength, _specifications.FreeLengthEpsilon)); 
            return resultRows;
        }

        public List<List<string>> informationResultRows(string testColumnString, string resultColumnString, string descriptonColumnString)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(informationResultRow(testColumnString, resultColumnString, descriptonColumnString));
            return resultRows;
        }

        public List<List<string>> cylinderTreatmentPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan"));
            resultRows.Add(checkTreatmentPlanChannelLength(_specifications.ExpectedChannelLength));
            return resultRows;
        }

        public List<List<string>> esofagusInfoRows(UserInputEsofagus userInputEsofagus)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Info"));
            resultRows.Add(checkEsofagusUserInputIsSet(userInputEsofagus.InactiveLengthString.Length > 0,
           "Inaktiv längd", "Inaktiv längd är given", "Inaktiv längd är inte given"));
            resultRows.Add(checkEsofagusUserInputIsSet(userInputEsofagus.ActiveLengthString.Length > 0,
           "Aktiv längd", "Aktiv längd är given", "Aktiv längd är inte given"));
            resultRows.Add(checkEsofagusUserInputIsSet(userInputEsofagus.PlanCode.Length > 0,
           "Plankod", "Plankod är given", "Plankod är inte given"));
            resultRows.Add(headerResultRow("Plan"));
            StringExtractor stringExtractor = new StringExtractor();
            Calculator calculator = new Calculator();
            decimal estimatedTreatmentTime = 
                calculator.estimateEsofagusTreatmentTime(stringExtractor.decimalStringToDecimal(userInputEsofagus.PrescribedDoseString),
                stringExtractor.decimalStringToDecimal(userInputEsofagus.ActiveLengthString),
                _treatmentPlan.plannedSourceStrengthValue());
            decimal reportedTreatmentTime = _treatmentPlan.totalTreatmentTimeValue();
            resultRows.Add(checkTreatmentTime(estimatedTreatmentTime,
                reportedTreatmentTime, _specifications.TreatmentTimeEpsilon));
            return resultRows;
        }

        public List<List<string>> intrauterineInfoRows(UserInputIntrauterine userInputIntrauterine)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Info"));
            resultRows.Add(checkApplicatorTypeIsSet(userInputIntrauterine.ApplicatorTypeIsSet));
            resultRows.Add(checkApplicatorDiameterIsSet(userInputIntrauterine.ApplicatorDiameterIsSet));
            if (_specifications.IntrauterineApplicatorType == IntrauterineApplicatorType.VMIX ||
                _specifications.IntrauterineApplicatorType == IntrauterineApplicatorType.VMIX_M_MATRIS)
            {
                resultRows.Add(checkApplicatorDiameterNr2IsSet(userInputIntrauterine.ApplicatorDiameterNr2IsSet));
            }
            resultRows.Add(checkFractionDoseIsSet(userInputIntrauterine.FractionDoseIsSet));
            resultRows.Add(checkPlanCodeIsSet(userInputIntrauterine.PlanCodeIsSet));
            resultRows.Add(checkSameSourceIsSet(userInputIntrauterine.SameSourceIsSet));
            return resultRows;
        }

        public List<List<string>> intrauterineTreatmentPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan"));
            resultRows.Add(checkIntrauterinePlanCatheterLengths());
            resultRows.Add(checkIntrauterineOffsetLengths());
            return resultRows;
        }

    public List<List<string>> cylinderTreatmentPlanAndCylinderSettingsResultRows(DataForTreatmentTimeEstimate dataForTreatmentTimeEstimate)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & info"));
            Calculator calculator = new Calculator();
            decimal estimatedTreatmentTime =
                calculator.estimateCylindricTreatmentTime(dataForTreatmentTimeEstimate.CylinderType,
                dataForTreatmentTimeEstimate.CylinderDiameter,
                dataForTreatmentTimeEstimate.PrescriptionDose,
                _treatmentPlan.plannedSourceStrengthValue(),
                dataForTreatmentTimeEstimate.TreatmentLength);
            decimal reportedTreatmentTime = _treatmentPlan.totalTreatmentTimeValue();
            resultRows.Add(checkTreatmentTime(estimatedTreatmentTime, reportedTreatmentTime, _specifications.CylinderTreatmentTimeEpsilon));
            string expectedPlanName = dataForTreatmentTimeEstimate.CylinderType.ToString() + 
                dataForTreatmentTimeEstimate.CylinderDiameter.ToString() + 
                "0" +
                dataForTreatmentTimeEstimate.TreatmentLength.ToString() +
                "flex";
            resultRows.Add(checkPlanName(expectedPlanName));
            return resultRows;
        }

        public List<List<string>> intrauterineTreatmentPlanAndIntrauterineSettingsResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & info"));
            //Calculator calculator = new Calculator();
            resultRows.Add(checkSelectedApplicatorTypeAndDiamterWithApplicatorName());
            resultRows.Add(checkPlanNameIntrauterine());
            return resultRows;
        }

        public List<List<string>> esofagusFractionXHeaderRow()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & TCC fraktion X"));
            return resultRows;
        }

        public List<List<string>> esofagusFirstFractionHeaderRow()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & TCC första frakt."));
            return resultRows;
        }

        public List<List<string>> resultRows(bool skipApprovalTest = false, bool useRelativeEpsilon = false, bool useTimeEpsilonVenezia = false)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & TCC."));
            resultRows.Add(checkPatientName());
            resultRows.Add(checkPatientId());
            resultRows.Add(checkPlanCode());
            resultRows.Add(checkApproval(skipApprovalTest));
            resultRows.Add(checkApproveDateTime());
            resultRows.Add(checkFractionDose());
            resultRows.Add(checkPlannedSourceStrength(_specifications.AirKermaStrengthEpsilon));
            resultRows.Add(checkTotalTreatmentTime(_specifications.TotalTimeEpsilon)); 
            resultRows.Add(checkRealizedAKStrength(_specifications.AKStrengthEpsilon)); 
            resultRows.Add(checkRealizedTotalTreatmentTime(_specifications.TotalTimeEpsilon));
            decimal timeEpsilon = _specifications.TimeEpsilon;
            if (useRelativeEpsilon)
            {
                timeEpsilon = _specifications.RelativeTimeEpsilon;
            }
            if (useTimeEpsilonVenezia)
            {
                timeEpsilon = _specifications.TimeEpsilonVenezia;
            }
            resultRows.Add(checkCatheterPositionTimePairs(timeEpsilon, useRelativeEpsilon));
            return resultRows;
        }

        public List<List<string>> esofagusTreatmentLengthResultRows(UserInputEsofagus userInputEsofagus, Tuple<decimal, decimal> planTccActiveLength)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkEsofagusIndexerLengthInputAndPlan(userInputEsofagus));
            resultRows.Add(checkEsofagusIndexerLengthInputPlanAndTcc(userInputEsofagus));
            resultRows.Add(checkActiveLengthInputPlanAndTcc(userInputEsofagus, planTccActiveLength));
            return resultRows;
        }

        public List<List<string>> sourceComparisonResultRows(bool sameSource)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkCalibrationDateTime(sameSource));
            return resultRows;
        }

        public List<List<string>> allXpsResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan, DVH & TCC"));
            resultRows.Add(checkPresciptionDose(_specifications.PrescriptionDose, _treatmentPlan.PrescribedDose(), 
                _treatmentDvh.PrescribedDose(), _tccPlan.PrescribedDose()));

            return resultRows;
        }

        public List<List<string>>  prescriptionDoseResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPresciptionDose(_specifications.PrescriptionDose, _treatmentPlan.PrescribedDose(),
                _tccPlan.PrescribedDose()));
            return resultRows;
        }

        public List<List<string>> errorResultRows(string testName, string desciption)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(errorResultRow(testName, desciption));
            return resultRows;
        }

        public List<List<string>> planCodeResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPlanCode(_specifications.PlanCode, _treatmentPlan.planCode(),
                _tccPlan.planCode()));
            return resultRows;
        }

        public List<List<string>> planCodeResultRowsCylinder()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPlanCodeCylinder(_specifications.PlanCodeCylinder, _treatmentPlan.planCode(),
                _tccPlan.planCode()));
            return resultRows;
        }

        public List<List<string>> prescriptionDoseResultRowsCylinder()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPresciptionDoseylinder(_specifications.PrescriptionDoseCylinder, _treatmentPlan.PrescribedDose(),
                _tccPlan.PrescribedDose()));
            return resultRows;
        }
        public List<List<string>> planCodeResultRowsIntrauterine()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPlanCodeCylinder(_specifications.PlanCodeIntrauterine, _treatmentPlan.planCode(),
                _tccPlan.planCode()));
            return resultRows;
        }

        public List<List<string>> prescriptionDoseResultRowsIntrauterine()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPresciptionDoseylinder(_specifications.PrescriptionDoseIntrauterine, _treatmentPlan.PrescribedDose(),
                _tccPlan.PrescribedDose()));
            return resultRows;
        }

        public List<LiveCatheter> treatmentPlanLiveCatheters()
        {
            return _treatmentPlan.liveCatheters();
        }

        public List<LiveCatheter> treatmentPlanLiveCathetersDecayCorrected()
        {
            List<LiveCatheter> LiveCatheters = new List<LiveCatheter>();
            StringExtractor stringExtractor = new StringExtractor();
            foreach (var item in _treatmentPlan.liveCatheters(true))
            {
                List<Tuple<string, string>> positionTimePairs = new List<Tuple<string, string>>();
                foreach (var subItem in item.positonTimePairs())
                {
                    decimal correctedTime = stringExtractor.decimalStringToDecimal(subItem.Item2) * plannedToRelizationAKStrengthQuota();
                    Tuple<string, string> tuple = new Tuple<string, string>(subItem.Item1, stringExtractor.decimalToTwoDecimalString(correctedTime));
                    positionTimePairs.Add(tuple);
                }
                LiveCatheter liveCatheter = new LiveCatheter();
                liveCatheter.appendPositionTimePairs(positionTimePairs);
                liveCatheter.setCatheterNumber(item.catheterNumber());
                LiveCatheters.Add(liveCatheter);
            }
            return LiveCatheters;
        }

        public List<LiveCatheter> tccPlanLiveCatheters()
        {
            return _tccPlan.liveCatheters();
        }
    }

}
