using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WpfApp1
{
    class Specifications
    {
        private decimal _needleDepth;
        private decimal _freeLength;
        private decimal _freeLengthEpsilon = 0.1m;
        private decimal _needleDepthEpsilon = 0.1m;
        private decimal _timeEpsilon = 0.0549m;
        private decimal _relativeTimeEpsilon = 0.3m;
        //private decimal _expectedChannelLength = 1190.0m;
        private decimal _expectedChannelLength;
        private decimal _treatmentTimeEpsilon = 0.1m;
        private decimal _cylinderTreatmentTimeEpsilon = 0.01m;
        private decimal _airKermaStrengthEpsilon = 0.3m;
        private decimal _prescriptionDose;
        private decimal _totalTimeEpsilon = 0.2m;
        private decimal _AKStrengthEpsilon = 0.1m;

        public decimal NeedleDepth { get => _needleDepth; set => _needleDepth = value; }
        public decimal FreeLength { get => _freeLength; set => _freeLength = value; }
        public decimal NeedleDepthEpsilon { get => _needleDepthEpsilon; set => _needleDepthEpsilon = value; }
        public decimal TimeEpsilon { get => _timeEpsilon; set => _timeEpsilon = value; }
        public decimal ExpectedChannelLength { get => _expectedChannelLength; set => _expectedChannelLength = value; }
        public decimal FreeLengthEpsilon { get => _freeLengthEpsilon; set => _freeLengthEpsilon = value; }
        public decimal TreatmentTimeEpsilon { get => _treatmentTimeEpsilon; set => _treatmentTimeEpsilon = value; }
        public decimal PrescriptionDose { get => _prescriptionDose; set => _prescriptionDose = value; }
        public decimal AirKermaStrengthEpsilon { get => _airKermaStrengthEpsilon; set => _airKermaStrengthEpsilon = value; }
        public decimal CylinderTreatmentTimeEpsilon { get => _cylinderTreatmentTimeEpsilon; set => _cylinderTreatmentTimeEpsilon = value; }
        public decimal TotalTimeEpsilon { get => _totalTimeEpsilon; set => _totalTimeEpsilon = value; }
        public decimal AKStrengthEpsilon { get => _AKStrengthEpsilon; set => _AKStrengthEpsilon = value; }
        public decimal RelativeTimeEpsilon { get => _relativeTimeEpsilon; set => _relativeTimeEpsilon = value; }
    }
}
