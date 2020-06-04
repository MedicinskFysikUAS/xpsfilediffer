using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Calculator
    {
        private decimal _needleLength;
        private decimal _probeDistance;


        public decimal NeedleLength { get => _needleLength; set => _needleLength = value; }
        public decimal ProbeDistance { get => _probeDistance; set => _probeDistance = value; }

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
    }
}
