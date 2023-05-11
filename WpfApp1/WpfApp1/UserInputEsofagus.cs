using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class UserInputEsofagus
    {
        private string _activeLengthString;
        private string _inactiveLengthString;
        private string _indexerLengthString;
        private string planCode;
        private bool isSameSource;
        private bool isFirstFraction;
        private string _prescribedDoseString;

        public string ActiveLengthString { get => _activeLengthString; set => _activeLengthString = value; }
        public string InactiveLengthString { get => _inactiveLengthString; set => _inactiveLengthString = value; }
        public string IndexerLengthString { get => _indexerLengthString; set => _indexerLengthString = value; }
        public string PlanCode { get => planCode; set => planCode = value; }
        public bool IsSameSource { get => isSameSource; set => isSameSource = value; }
        public bool IsFirstFraction { get => isFirstFraction; set => isFirstFraction = value; }
        public string PrescribedDoseString { get => _prescribedDoseString; set => _prescribedDoseString = value; }
    }
}
