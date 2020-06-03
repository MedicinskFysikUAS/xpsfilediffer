using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class TreatmentPlanCatheter
    {
        private int _catheterNumber;
        private decimal _selector;
        private decimal _depth;
        private decimal _freeLength;
        private decimal _offset;
        private decimal _tipField;
        private bool _isActiveLocked;
        private bool _isTimeLocked;

        public int catheterNumber { get => _catheterNumber; set => _catheterNumber = value; }
        public decimal selector { get => _selector; set => _selector = value; }
        public decimal depth { get => _depth; set => _depth = value; }
        public decimal freeLength { get => _freeLength; set => _freeLength = value; }
        public decimal offset { get => _offset; set => _offset = value; }
        public decimal tipField { get => _tipField; set => _tipField = value; }
        public bool isActiveLocked { get => _isActiveLocked; set => _isActiveLocked = value; }
        public bool isTimeLocked { get => _isTimeLocked; set => _isTimeLocked = value; }
    }
}
