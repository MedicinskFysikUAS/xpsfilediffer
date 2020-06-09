using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Calculator
    {
        private decimal _needleLength;
        private decimal _probeDistance;
        private decimal _prescibedDose;
        private decimal _ptvVolume;


        public decimal NeedleLength { get => _needleLength; set => _needleLength = value; }
        public decimal ProbeDistance { get => _probeDistance; set => _probeDistance = value; }
        public decimal PrescibedDose { get => _prescibedDose; set => _prescibedDose = value; }
        public decimal PtvVolume { get => _ptvVolume; set => _ptvVolume = value; }

        public decimal freeLength()
        {
            return _needleLength - 5;
        }

        public decimal needleDepth()
        {
            return 205 - _needleLength - _probeDistance;
        }

        public decimal needleLengthPlusProbeDistance()
        {
            return _needleLength + _probeDistance;
        }

        public bool sufficientNeedleDepth()
        {
            return needleLengthPlusProbeDistance() < 195;
        }

        public decimal estimatedTreatmentTime(decimal ptvVolume, decimal prescribedDose, decimal sourceStrength)
        {
            decimal constant1 = 180.0m;
            decimal constant2 = 5597000.0m;
            return (prescribedDose / 10) * ((constant1 * ptvVolume + constant2) / sourceStrength);
        }
    }
}
