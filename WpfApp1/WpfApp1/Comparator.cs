﻿using System;
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
            // Debug
            string tmp1 = _treatmentPlan.calibrationDateTime();
            string tmp2 = _treatmentPlan.statusSetDateTime();
            string tmp3 = _tccPlan.realizationDateAndTime();
            // end
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
            Tuple<List<LiveCatheter>, List<LiveCatheter>> tuple =  makeCorrectedTpLiveCathetersAboveThreshold(_treatmentPlan.liveCatheters(), _tccPlan.liveCatheters());
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

        private bool treatmentPlanTccHasSameChannelLengths()
        {
            List<Tuple<int, decimal>> tccCatheterNumberAndLengths = _tccPlan.getCatheterNumberAndLengths();
            List<LiveCatheter>  treatmentPlanLiveCatheters = _treatmentPlan.liveCatheters();

            if (treatmentPlanLiveCatheters.Count== 0 ||
                tccCatheterNumberAndLengths.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var tccCatheterNumberAndLength in tccCatheterNumberAndLengths)
                {

                }

                List<string> treatmentPlanCatheterLengths = _treatmentPlan.catheterLengths();
                return true;
                //List<string> tccPlanCatheterLengths = toDotSeparated(_tccPlan.catheterLengths());
                //var firstNotSecond = treatmentPlanCatheterLengths.Except(tccPlanCatheterLengths).ToList();
                //var secondNotFirst = tccPlanCatheterLengths.Except(treatmentPlanCatheterLengths).ToList();
                //return firstNotSecond.Count == 0 &&
                //    secondNotFirst.Count == 0;
            }
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

        //private bool channelLengthInIntrauterinePlanIsOkOld()
        //{
        //    List<IntrauterineCatheter> intrauterineCatheters = _treatmentPlan.intrauterineCatheters();
        //    bool isOk = true;
        //    if (intrauterineCatheters.Count == 0)
        //    {
        //        isOk = false;
        //    }

        //    foreach (var item in intrauterineCatheters)
        //    {
        //        if (item.IntrauterineCatheterType == IntrauterineCatheterType.MODEL)
        //        {
        //            if (Math.Abs(item.CatheterLength - _specifications.ExpectedLengthModelCatheter) >
        //                _specifications.ExpectedLengthModelManualCatheterEpsilon)
        //            {
        //                isOk = false;
        //                break;
        //            }
        //        }
        //        else if (item.IntrauterineCatheterType == IntrauterineCatheterType.MANUAL)
        //        {
        //            if (Math.Abs(item.CatheterLength - _specifications.ExpectedLengthManualCatheter) >
        //                _specifications.ExpectedLengthModelManualCatheterEpsilon)
        //            {
        //                isOk = false;
        //                break;
        //            }
        //        }
        //    }
        //    return isOk;
        //}

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
                    if (Math.Abs(item.ChannelLength - _specifications.ExpectedLengthNonPipeCatheter) >
                        _specifications.ExpectedLengthPipeCatheterEpsilon)
                    {
                        isOk = false;
                        break;
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
                    if (Math.Abs(liveCatheters[i].offsetLength() - 0.0m) > 0.001m )
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
                " respektive " + _specifications.ExpectedLengthNonPipeCatheter + " mm.";
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
            string description = result ? "Alla nålar har korrekt offset" : "Alla nålar har inte korrekt offset";
            resultRow.Add(description);
            return resultRow;
        }

        private List<string> checkIntrauterineTreatmenPlanTccPlanChannelLengths()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Kanalängd i dosplan vs tcc");
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

        


        public List<string> checkPlanNameIntrauterine()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Plannamn");
            string treatmentPlanApplicatorName = _treatmentPlan.applicatorName();
            List<string> strings = treatmentPlanApplicatorName.Split(' ').ToList();
            string treatmentPlanPlanName = _treatmentPlan.intrauterinePlanName();
            bool planNameInTreatmentPlanIsConsistent = false;
            string userInputTypeString = _treatmentPlan.applicatorStringFromApplicationType(_specifications.IntrauterineApplicatorType);
            int userInputDiameter = _specifications.ApplicatorDiameter;
            string expectedPlanName = userInputDiameter.ToString();
            bool inputPlanNameIsOk = false;
            foreach (var item in strings)
            {
                if (treatmentPlanPlanName.Contains(item))
                {
                    planNameInTreatmentPlanIsConsistent = true;
                }
                if (item.Contains(expectedPlanName))
                {
                    inputPlanNameIsOk = true;
                }
            }
            string description = "";
            description += planNameInTreatmentPlanIsConsistent ? "Plannamnet och applikatorsnamnet i planen stämmer.":
            "Plannamnet och applikatorsnamnet i planen stämmer inte.";
            description += inputPlanNameIsOk ? " Den angivna applikatorstypen stämmer med applikatorsnamnet i planen." :
                " Den angivna applikatorstypen stämmer inte med applikatorsnamnet i planen.";
            if (planNameInTreatmentPlanIsConsistent && inputPlanNameIsOk)
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

        public List<List<string>> intrauterineInfoRows(UserInputIntrauterine userInputIntrauterine)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Info"));
            resultRows.Add(checkApplicatorTypeIsSet(userInputIntrauterine.ApplicatorTypeIsSet));
            resultRows.Add(checkApplicatorDiameterIsSet(userInputIntrauterine.ApplicatorDiameterIsSet));
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
            Calculator calculator = new Calculator();
            resultRows.Add(checkPlanNameIntrauterine());
            return resultRows;
        }

        public List<List<string>> resultRows(bool skipApprovalTest = false, bool useRelativeEpsilon = false, bool useRelativeEpsilonIntrauterine = false)
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & TCC"));
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
                if (useRelativeEpsilonIntrauterine)
                {
                    timeEpsilon = _specifications.RelativeTimeEpsilonÏntrauterine;

                }
                else
                {
                    timeEpsilon = _specifications.RelativeTimeEpsilon;
                }
            }
            resultRows.Add(checkCatheterPositionTimePairs(timeEpsilon, useRelativeEpsilon));
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
            foreach (var item in _treatmentPlan.liveCatheters())
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
