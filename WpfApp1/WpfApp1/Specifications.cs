using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WpfApp1
{
    public class Specifications
    {
        private decimal _needleDepth;
        private decimal _freeLength;
        private decimal _freeLengthEpsilon = 0.1m;
        private decimal _needleDepthEpsilon = 0.2m;
        private decimal _timeEpsilon = 0.0549m;
        private decimal _timeEpsilonVenezia = 0.1m;
        private decimal _relativeTimeEpsilon = 0.8m;
        private decimal _relativeTimeEpsilonÏntrauterine = 0.9m;
        //private decimal _expectedChannelLength = 1190.0m;
        private decimal _expectedChannelLength;
        private decimal _treatmentTimeEpsilon = 0.1m;
        private decimal _cylinderTreatmentTimeEpsilon = 0.01m;
        private decimal _airKermaStrengthEpsilon = 0.6m;
        private decimal _prescriptionDose;
        private decimal _totalTimeEpsilon = 1.0m;
        private decimal _AKStrengthEpsilon = 0.1m;
        private decimal _prescriptionDoseCylinder;
        private string _planCode;
        private string _planCodeCylinder;
        private decimal _prescriptionDoseIntrauterine;
        private string _planCodeIntrauterine;
        private IntrauterineApplicatorType _intrauterineApplicatorType;
        private int _applicatorDiameter;
        private int _applicatorDiameterNr2;
        private decimal _expectedLengthPipeCatheter = 1300.0m;
        private decimal _expectedLengthNonPipeCatheter = 1288.0m;
        private decimal _expectedLengthNonPipeCatheterAbove29 = 1234.0m;
        private decimal _expectedLengthPipeCatheterEpsilon = 0.1m;
        private decimal _lengthOfCathetersUsedForEsofagus = 1000.0m;
        private decimal _maxChannelLengthEsofagus;
        private decimal _indexerLengthEpsilon = 0.1m;
        private double _approvalTimeToleranceHours = 2.0;
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
        public decimal PrescriptionDoseCylinder { get => _prescriptionDoseCylinder; set => _prescriptionDoseCylinder = value; }
        public string PlanCode { get => _planCode; set => _planCode = value; }
        public string PlanCodeCylinder { get => _planCodeCylinder; set => _planCodeCylinder = value; }
        public decimal PrescriptionDoseIntrauterine { get => _prescriptionDoseIntrauterine; set => _prescriptionDoseIntrauterine = value; }
        public string PlanCodeIntrauterine { get => _planCodeIntrauterine; set => _planCodeIntrauterine = value; }
        public IntrauterineApplicatorType IntrauterineApplicatorType { get => _intrauterineApplicatorType; set => _intrauterineApplicatorType = value; }
        public int ApplicatorDiameter { get => _applicatorDiameter; set => _applicatorDiameter = value; }
        public decimal ExpectedLengthPipeCatheter { get => _expectedLengthPipeCatheter; set => _expectedLengthPipeCatheter = value; }
        public decimal ExpectedLengthNonPipeCatheter { get => _expectedLengthNonPipeCatheter; set => _expectedLengthNonPipeCatheter = value; }
        public decimal ExpectedLengthPipeCatheterEpsilon { get => _expectedLengthPipeCatheterEpsilon; set => _expectedLengthPipeCatheterEpsilon = value; }
        public decimal RelativeTimeEpsilonÏntrauterine { get => _relativeTimeEpsilonÏntrauterine; set => _relativeTimeEpsilonÏntrauterine = value; }
        public decimal TimeEpsilonVenezia { get => _timeEpsilonVenezia; set => _timeEpsilonVenezia = value; }
        public decimal ExpectedLengthNonPipeCatheterAbove29 { get => _expectedLengthNonPipeCatheterAbove29; set => _expectedLengthNonPipeCatheterAbove29 = value; }
        public int ApplicatorDiameterNr2 { get => _applicatorDiameterNr2; set => _applicatorDiameterNr2 = value; }
        public decimal LengthOfCathetersUsedForEsofagus { get => _lengthOfCathetersUsedForEsofagus; set => _lengthOfCathetersUsedForEsofagus = value; }
        public decimal MaxChannelLengthEsofagus { get => _maxChannelLengthEsofagus; set => _maxChannelLengthEsofagus = value; }
        public decimal IndexerLengthEpsilon { get => _indexerLengthEpsilon; set => _indexerLengthEpsilon = value; }
        public double approvalTimeToleranceHours { get => _approvalTimeToleranceHours; set => _approvalTimeToleranceHours = value; }
    }
}
