using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class UserInputIntrauterine
    {
        bool _applicatorTypeIsSet;
        bool _applicatorDiameterIsSet;
        bool _fractionDoseIsSet;
        bool _planCodeIsSet;
        bool _sameSourceIsSet;
        bool _applicatorDiameterNr2IsSet;

        public bool ApplicatorTypeIsSet { get => _applicatorTypeIsSet; set => _applicatorTypeIsSet = value; }
        public bool ApplicatorDiameterIsSet { get => _applicatorDiameterIsSet; set => _applicatorDiameterIsSet = value; }
        public bool FractionDoseIsSet { get => _fractionDoseIsSet; set => _fractionDoseIsSet = value; }
        public bool PlanCodeIsSet { get => _planCodeIsSet; set => _planCodeIsSet = value; }
        public bool SameSourceIsSet { get => _sameSourceIsSet; set => _sameSourceIsSet = value; }
        public bool ApplicatorDiameterNr2IsSet { get => _applicatorDiameterNr2IsSet; set => _applicatorDiameterNr2IsSet = value; }
    }
}
