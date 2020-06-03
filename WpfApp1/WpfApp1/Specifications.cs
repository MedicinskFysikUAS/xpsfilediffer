﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Specifications
    {
        private decimal _needleDepth;
        private decimal _freeLength;
        private decimal _depthEpsilon;
        private decimal _timeEpsilon;

        public decimal NeedleDepth { get => _needleDepth; set => _needleDepth = value; }
        public decimal FreeLength { get => _freeLength; set => _freeLength = value; }
        public decimal DepthEpsilon { get => _depthEpsilon; set => _depthEpsilon = value; }
        public decimal TimeEpsilon { get => _timeEpsilon; set => _timeEpsilon = value; }
    }
}