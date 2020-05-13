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
    }
}
