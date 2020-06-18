using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class DataForTreatmentTimeEstimate
    {
        private CylinderType _cylinderType;
        private int _cylinderDiameter;
        private decimal _prescriptionDose;
        private decimal _treatmentLength;

        public int CylinderDiameter { get => _cylinderDiameter; set => _cylinderDiameter = value; }
        public decimal PrescriptionDose { get => _prescriptionDose; set => _prescriptionDose = value; }
        public decimal TreatmentLength { get => _treatmentLength; set => _treatmentLength = value; }
        public CylinderType CylinderType { get => _cylinderType; set => _cylinderType = value; }
    }
}
