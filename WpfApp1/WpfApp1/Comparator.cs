using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Comparator
    {
        private TreatmentPlan _treatmentPlan;
        private TccPlan _tccPlan;
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

        public bool treatmentPlanHasExpectedDepth(decimal expectedDepth)
        {
            foreach (var catheter in _treatmentPlan.treatmentPlanCatheters())
            {
                if (Math.Abs(catheter.depth - expectedDepth) > _specifications.DepthEpsilon)
                {
                    return false;
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
            resultRow.Add("Personnummer i TP: " + _treatmentPlan.patientId() +
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
            resultRow.Add("Plankod i TP: " + _treatmentPlan.planCode() + 
                " i TCC: " + _tccPlan.planCode() );

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

            resultRow.Add("TP är : " + TPStatus + 
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
            resultRow.Add("Tiden för godkännande i TP och TCC är " + info);

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
            resultRow.Add("Fraktionsdos i TP: " + _treatmentPlan.fractionDose() +
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
            resultRow.Add("Planned Source Strength i TP: " + _treatmentPlan.plannedSourceStrength() +
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
            resultRow.Add("Behandlingstid i TP: " + _treatmentPlan.totalTreatmentTime() +
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
            resultRow.Add("<-- " + description + " -->");
            resultRow.Add("");
            return resultRow;
        }

        public List<string> checkTreatmentPlanDepth(decimal expectedDepth, decimal depthEpsilon)
        {
            List<string> resultRow = new List<string>();
            resultRow.Add("Samma nåldjup i dosplan");
            string descriptionString = "";
            if (treatmentPlanHasExpectedDepth(expectedDepth))
            {
                resultRow.Add("OK");
                ++_numberOfOk;
                descriptionString = "Nåldjupet är konstant (inom " + depthEpsilon + " mm)";
            }
            else
            {
                resultRow.Add("Inte OK");
                ++_numberOfErrors;
                descriptionString = "Nåldjupet avviker mer än " + depthEpsilon + " mm.";
            }
            resultRow.Add(descriptionString);

            return resultRow;
        }


        public List<List<string>> treatmentPlanResultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            // TODO Add a 'header' result row 
            resultRows.Add(headerResultRow("Kontroll av dosplan"));
            resultRows.Add(checkTreatmentPlanChannelLength(_specifications.ExpectedChannelLength));
            resultRows.Add(checkTreatmentPlanDepth(_specifications.NeedleDepth, _specifications.DepthEpsilon)); 
            return resultRows;
        }

            public List<List<string>> resultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
            resultRows.Add(headerResultRow("Kontroll av dosplan och tcc-rapport"));
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
