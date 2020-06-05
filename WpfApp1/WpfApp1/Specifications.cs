using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Specifications
    {
        private decimal _needleDepth;
        private decimal _freeLength;
        private decimal _freeLengthEpsilon = 0.3m;
        private decimal _needleDepthEpsilon = 0.3m;
        private decimal _timeEpsilon = 0.051m;
        private decimal _expectedChannelLength = 1190.0m;

        public decimal NeedleDepth { get => _needleDepth; set => _needleDepth = value; }
        public decimal FreeLength { get => _freeLength; set => _freeLength = value; }
        public decimal NeedleDepthEpsilon { get => _needleDepthEpsilon; set => _needleDepthEpsilon = value; }
        public decimal TimeEpsilon { get => _timeEpsilon; set => _timeEpsilon = value; }
        public decimal ExpectedChannelLength { get => _expectedChannelLength; set => _expectedChannelLength = value; }
        public decimal FreeLengthEpsilon { get => _freeLengthEpsilon; set => _freeLengthEpsilon = value; }
    }
}
