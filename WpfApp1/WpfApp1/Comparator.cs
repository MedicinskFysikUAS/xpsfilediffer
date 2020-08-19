using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Comparator
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

        ErrorCode hasSameCatheterPositionTimePairs(decimal timeEpsilon, bool useRelativeEpsilon)
        {
            List<LiveCatheter> tpLiveCatheters = _treatmentPlan.liveCatheters();
            List<LiveCatheter> tccLiveCatheters = _tccPlan.liveCatheters();
            ErrorCode errorCode = new ErrorCode();
            if (tpLiveCatheters.Count != tccLiveCatheters.Count)
            {
                errorCode.Number = -100;
                errorCode.Description = "Olika antal katetrar i dosplan och TCC plan";
                return errorCode;
            }
            int counter = 0;
            StringExtractor stringExtractor = new StringExtractor();
            foreach (var item in tpLiveCatheters)
            {
                if (item.catheterNumber() == tccLiveCatheters[counter].catheterNumber() &&
                    item.positonTimePairs().Count >= tccLiveCatheters[counter].positonTimePairs().Count)
                {
                    int subCounter = 0;
                    foreach (var subItem in item.positonTimePairs())
                    {
                        if (stringExtractor.decimalStringToDecimal(subItem.Item2) < timeEpsilon)
                        {
                            continue;
                        }
                        decimal correctedTpTime = stringExtractor.decimalStringToDecimal(subItem.Item2) * plannedToRelizationAKStrengthQuota();
                        decimal deltaTime = Math.Abs(correctedTpTime -
                        stringExtractor.decimalStringToDecimal(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item2));
                        if (useRelativeEpsilon)
                        {
                            deltaTime = (deltaTime / correctedTpTime) * 100.0m;
                        }
                        if (subItem.Item1 != stringExtractor.decimalStringToZeroDecimalString(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item1) ||
                             deltaTime > timeEpsilon)
                        {
                            errorCode.Number = -101;
                            errorCode.Description = "Kateter nummer " + (counter + 1) + " på " + (subCounter + 1) + ":e positionen avviker. " +
                                "Position i dosplan: " + subItem.Item1 + " i TCC plan: " + stringExtractor.decimalStringToZeroDecimalString(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item1) +
                                ". Korrigerad tid i dosplan: " + correctedTpTime + " tid i TCC plan: " + stringExtractor.decimalStringToDecimal(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item2);
                            return errorCode;
                        }
                        ++subCounter;
                    }
                }
                else
                {
                    errorCode.Number = -102;
                    errorCode.Description = "Kateternummer i dosplan och TCC plan är olika för kateter " + (counter + 1) + 
                        " eller så är antalet positioner för denna kateter inte det samma i dosplan och TCC plan.";
                    return errorCode;
                }
                ++counter;
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
            resultRow.Add("Kalibreringstidpunkt");
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
                descriptionString = "Channel length för samtilga kanaler är " + expectedChannelLength;
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
                descriptionString = "Nåldjupet är " + expectedDepth + " (inom " + needleDepthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                descriptionString = "Nåldjupet avviker från " + expectedDepth + " mer än " + needleDepthEpsilon + " mm.";
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
                descriptionString = "Den angivna ordinerade dosen är den samma som i planen och TCC planen";
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
            descriptionString = "Det förväntade plannamnet är: " + expectedPlanName + " planens plannamn är " + _treatmentPlan.cylindricPlanName();
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

        public List<List<string>> treatmentPlanAndDvhResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & DVH"));
            Calculator calculator = new Calculator();
            decimal estimatedTreatmentTime = calculator.estimatedTreatmentTime(
                _treatmentDvh.PtvVolume(), _treatmentDvh.PrescribedDose(), _treatmentPlan.plannedSourceStrengthValue());
            decimal reportedTreatmentTime = _treatmentPlan.totalTreatmentTimeValue();

            resultRows.Add(checkTreatmentTime(estimatedTreatmentTime, reportedTreatmentTime, _specifications.TreatmentTimeEpsilon));
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

        public List<List<string>> resultRows(bool skipApprovalTest = false, bool useRelativeEpsilon = false)
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
                timeEpsilon = _specifications.RelativeTimeEpsilon;
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

        public List<List<string>> prescriptionDoseResultRowsCylinder()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(checkGuiPlanTccPresciptionDoseylinder(_specifications.PrescriptionDoseCylinder, _treatmentPlan.PrescribedDose(),
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
