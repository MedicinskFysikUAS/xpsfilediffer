using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class TreatmentPlanCatheter
    {
        private int _catheterNumber;
        private int _selector;
        private string _depth;
        private string _freeLength;
        private string _offset;
        private string _tipField;
        private bool _isActiveLocked;
        private bool _isTimeLocked;

        public int catheterNumber { get => _catheterNumber; set => _catheterNumber = value; }
        public int selector { get => _selector; set => _selector = value; }
        public string depth { get => _depth; set => _depth = value; }
        public string freeLength { get => _freeLength; set => _freeLength = value; }
        public string offset { get => _offset; set => _offset = value; }
        public string tipField { get => _tipField; set => _tipField = value; }
        public bool isActiveLocked { get => _isActiveLocked; set => _isActiveLocked = value; }
        public bool isTimeLocked { get => _isTimeLocked; set => _isTimeLocked = value; }
    }
}
