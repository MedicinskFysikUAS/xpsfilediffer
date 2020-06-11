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
        private int _numberOfOk; // TODO Remove these
        private int _numberOfWarnings;
        private int _numberOfErrors;
        private Specifications _specifications;

        public Comparator(Specifications specifications)
        {
            _specifications = specifications;
        }

        public TreatmentPlan treatmentPlan { get => _treatmentPlan; set => _treatmentPlan = value; }
        public TccPlan tccPlan { get => _tccPlan; set => _tccPlan = value; }

        public TreatmentDvh treatmentDvh { get => _treatmentDvh; set => _treatmentDvh = value; }

        // has --------------------------
        public bool hasSamePatientName()
        {
            return (_treatmentPlan.patientFirstName() == _tccPlan.patientFirstName()) && (_treatmentPlan.patientLastName() == _tccPlan.patientLastName());
        }

        public bool hasSamePatientId()
        {
            return (_treatmentPlan.patientId() == _tccPlan.patientId());
        }

        public bool hasSamePlanCode()
        {
            return (_treatmentPlan.planCode() == _tccPlan.planCode());
        }

        public bool hasApprovedStatus()
        {
            return (_treatmentPlan.planIsApproved() && _tccPlan.planIsApproved());

        }

        public bool hasSameStatusSetDateTime()
        {
            return (_treatmentPlan.statusSetDateTime() == _tccPlan.statusSetDateTime());
        }

        public bool hasSameFractionDose()
        {
            return (_treatmentPlan.fractionDose() == _tccPlan.fractionDose());
        }

        public bool hasSamePlannedSourceStrength()
        {
            return (_treatmentPlan.plannedSourceStrength() == _tccPlan.plannedSourceStrength());
        }

        bool hasSameTotalTreatmentTime()
        {
            return (_treatmentPlan.totalTreatmentTime() == _tccPlan.totalTreatmentTime());
        }


        bool hasSameCatheterPositionTimePairs(decimal timeEpsilon)
        {
            List<LiveCatheter> tpLiveCatheters = _treatmentPlan.liveCatheters();
            List<LiveCatheter> tccLiveCatheters = _tccPlan.liveCatheters();
            if (tpLiveCatheters.Count != tccLiveCatheters.Count)
            {
                return false;
            }
            int counter = 0;
            StringExtractor stringExtractor = new StringExtractor();
            foreach (var item in tpLiveCatheters)
            {
                if (item.catheterNumber() == tccLiveCatheters[counter].catheterNumber())
                {
                    int subCounter = 0;
                    foreach (var subItem in item.positonTimePairs())
                    {
                        decimal deltaTime = Math.Abs(stringExtractor.decimalStringToDecimal(subItem.Item2) -
                            stringExtractor.decimalStringToDecimal(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item2));
                        if (subItem.Item1 != stringExtractor.decimalStringToZeroDecimalString(tccLiveCatheters[counter].positonTimePairs()[subCounter].Item1) ||
                            deltaTime > timeEpsilon)
                        {
                            return false;
                        }
                        ++subCounter;
                    }
                }
                else
                {
                    return false;
                }
                ++counter;
            }
            return true;
        }

        public bool treatmentPlanHasSameChannelLength(decimal channelLength)
        {
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


        public bool prescriptionDoseIsTheSame(decimal guiPresciptionDose, decimal treatmentPlanPrescriptionDose, decimal dvhPrescriptionDose)
        {
            return ((guiPresciptionDose == treatmentPlanPrescriptionDose) && (treatmentPlanPrescriptionDose == dvhPrescriptionDose));
        }

        // check -----------------------

        public List<string> checkPatientName()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma patientnamn");
            if (hasSamePatientName())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
            }
            resultRow.Add("Patientnamn i TP: " + _treatmentPlan.patientFirstName() + " " + _treatmentPlan.patientLastName() +
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
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
            }
            resultRow.Add("Plankod i plan: " + _treatmentPlan.planCode() +
                " i TCC: " + _tccPlan.planCode());

            return resultRow;
        }


        public List<string> checkApproval()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Planerna är godkända");
            if (hasApprovedStatus())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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

            resultRow.Add("plan är : " + TPStatus +
                " TCC-planen är : " + TCCStatus);

            return resultRow;
        }

        public List<string> checkApproveDateTime()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma godkännande tidpunkt");
            string info = "";
            if (hasSameStatusSetDateTime())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
                info += "samma";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
                info += "inte samma";
            }
            resultRow.Add("Tiden för godkännande i plan och TCC är " + info);

            return resultRow;
        }

        public List<string> checkFractionDose()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Fraktionsdos");
            if (hasSameFractionDose())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
            }
            resultRow.Add("Fraktionsdos i plan: " + _treatmentPlan.fractionDose() +
                " i TCC: " + _tccPlan.fractionDose());

            return resultRow;
        }

        public List<string> checkPlannedSourceStrength()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Dosplanens källstyrka");
            if (hasSamePlannedSourceStrength())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
            }
            resultRow.Add("Planned Source Strength i plan: " + _treatmentPlan.plannedSourceStrength() +
                " planerad AK-styrka i TCC: " + _tccPlan.plannedSourceStrength());

            return resultRow;
        }


        public List<string> checkTotalTreatmentTime()
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Dosplanens behandlingstid");
            if (hasSameTotalTreatmentTime())
            {
                resultRow.Add("OK");
                ++_numberOfOk;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
            }
            resultRow.Add("Behandlingstid i plan: " + _treatmentPlan.totalTreatmentTime() +
                " och i TCC: " + _tccPlan.totalTreatmentTime());

            return resultRow;
        }

        public List<string> checkCatheterPositionTimePairs(decimal timeEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Bestrålningsposition och tid");
            string descriptionString = "";
            if (hasSameCatheterPositionTimePairs(timeEpsilon))
            {
                resultRow.Add("OK");
                ++_numberOfOk;
                descriptionString = "Alla positioner och tider är lika.";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
                descriptionString = "Någon/några positioner/tider är olika.";
            }
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
                ++_numberOfOk;
                descriptionString = "Channel length för samtilga kanaler är " + expectedChannelLength;
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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

        public List<string> checkTreatmentPlanDepth(decimal expectedDepth, decimal needleDepthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma nåldjup i dosplan");
            string descriptionString = "";
            if (treatmentPlanHasExpectedDepth(expectedDepth, needleDepthEpsilon))
            {
                resultRow.Add("OK");
                ++_numberOfOk;
                descriptionString = "Nåldjupet är " + expectedDepth + " (inom " + needleDepthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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
                ++_numberOfOk;
                descriptionString = "Free length är " + expectedFreeLength + " (inom " + needleDepthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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
                ++_numberOfOk;
                descriptionString = "Avvikelsen mellan uppskattad och rapporterad tid är mindre än " + precentageEpsilon.ToString("0") + "% " + " (" +
                    estimatedTreatmentTime.ToString("0.0") + " resp " + reportedTreatmentTime.ToString("0.0") + " sek)";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
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
            resultRow.Add("Ordinerad dos");
            string descriptionString = "";
            if (prescriptionDoseIsTheSame(guiPresciptionDose, treatmentPlanPrescriptionDose, dvhPrescriptionDose))
            {
                resultRow.Add("OK");
                ++_numberOfOk;
                descriptionString = "Den angivna ordinerade dosen är den samma som i planen, dvh och tcc-rapporten";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
                descriptionString = "Den angivna ordinerade dosen är INTE den samma som i planen, dvh och tcc-rapporten";
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

        public List<List<string>> treatmentPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan"));
            resultRows.Add(checkTreatmentPlanChannelLength(_specifications.ExpectedChannelLength));
            resultRows.Add(checkTreatmentPlanDepth(_specifications.NeedleDepth, _specifications.NeedleDepthEpsilon)); 
            resultRows.Add(checkTreatmentFreeLength(_specifications.FreeLength, _specifications.FreeLengthEpsilon)); 
            return resultRows;
        }

            public List<List<string>> resultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Plan & TCC"));
            resultRows.Add(checkPatientName());
            resultRows.Add(checkPatientId());
            resultRows.Add(checkPlanCode());
            resultRows.Add(checkApproval());
            resultRows.Add(checkApproveDateTime());
            resultRows.Add(checkFractionDose());
            resultRows.Add(checkPlannedSourceStrength());
            resultRows.Add(checkTotalTreatmentTime()); 
            resultRows.Add(checkCatheterPositionTimePairs(_specifications.TimeEpsilon));
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

        //calculator.PrescibedDose = stringExtractor.decimalStringToDecimal(prescribedDoseText.Text);


        public int numberOfOk()
        {
            return _numberOfOk;
        }

        public int numberOfWarnings()
        {
            return _numberOfWarnings;
        }

        public int numberOfErrors()
        {
            return _numberOfErrors;
        }
    }

}
