using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Comparator
    {
        private TreatmentPlan _treatmentPlan;
        private TccPlan _tccPlan;

        public Comparator(TreatmentPlan treatmentPlan, TccPlan tccPlan)
        {
            _treatmentPlan = treatmentPlan;
            _tccPlan = tccPlan;
        }

        public bool hasSamePatientName()
        {
            return (_treatmentPlan.patientFirstName() == _tccPlan.patientFirstName()) && (_treatmentPlan.patientLastName() == _tccPlan.patientLastName());
        }

        public List<List<string>> resultRows()
        {
            List<List<string>> resultRows = new List<List<string>>();
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
            resultRow.Add("Patientnamn i TP: " + _treatmentPlan.patientFirstName() + " " + _treatmentPlan.patientLastName() +
                " i TCC: " + _tccPlan.patientFirstName() + " " + _tccPlan.patientLastName());
            resultRows.Add(resultRow);






            return resultRows;
        }
    }
}
