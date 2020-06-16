using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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


        private decimal getTableTime(List<Tuple<decimal, decimal>> tableTuples, decimal treatmentLength)
        {


            return 10.0m;
        }

        private decimal timeFromTable(CylinderType cylinderType, decimal cylinderDiameter, decimal treatmentLength)
        {
            List<Tuple<decimal, decimal>> tableTuples = new List<Tuple<decimal, decimal>>();
            if (cylinderType == CylinderType.VC)
            {
                if (cylinderDiameter == 20.0m)
                {
                    tableTuples = vc20Table();
                }
            }
            decimal tableTime = getTableTime(tableTuples, treatmentLength);
            return 10.0m;
        }

        public decimal estimateCylindricTreatmentTime()
        {
            CylinderType cylinderType = CylinderType.VC; // from GUI and plan?
            decimal cylinderDiameter = 10.0m; // from GUI and plan?
            decimal prescribedDose = 1.0m; // from the treatment plan
            decimal tableDose = 1.0m; // from the app.config file
            decimal tableSourceStrength = 1.0m; // from the app.config file
            decimal currentSourceStrength = 1.0m; // from the treatment plan
            decimal treatmentLength = 10.0m; // GUI and plan
            decimal tableTime = timeFromTable(cylinderType, cylinderDiameter, treatmentLength);
            decimal estimatedTime = tableTime * (prescribedDose / tableDose) * (tableSourceStrength / currentSourceStrength);
            return estimatedTime;
        }


        private List<Tuple<decimal, decimal>> vc20Table()
        {
            List<decimal> lengths = ConfigurationManager.AppSettings["VC_20_Lengths"].Split(';').ToList().Select(decimal.Parse).ToList();
            List<decimal> times = ConfigurationManager.AppSettings["VC_20_Times"].Split(';').ToList().Select(decimal.Parse).ToList();
            List<Tuple<decimal, decimal>> tuples = new List<Tuple<decimal, decimal>>();
            int counter = 0;
            foreach (var item in lengths)
            {
                tuples.Add(new Tuple<decimal, decimal>(item, times[counter]));
            }
            return tuples;
        }
    }
}
