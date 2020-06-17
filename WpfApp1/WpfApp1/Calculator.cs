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

        public decimal linear(decimal x, decimal x0, decimal x1, decimal y0, decimal y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        private decimal getTableTime(List<Tuple<decimal, decimal>> tableTuples, decimal treatmentLength)
        {
            int counter = 0;
            foreach (var item in tableTuples)
            {
                if (counter < tableTuples.Count - 1)
                {
                    if (treatmentLength >= item.Item1  && treatmentLength  < tableTuples[counter + 1].Item1)
                    {
                        return linear(treatmentLength, item.Item1, tableTuples[counter + 1].Item1, item.Item2, tableTuples[counter + 1].Item2);
                    }
                }
                else
                {
                    return linear(treatmentLength, tableTuples[counter - 1].Item1, item.Item1, tableTuples[counter - 1].Item2, item.Item2);
                }
                ++counter;
            }
            return -1.0m;
        }

       

        private decimal timeFromTable(CylinderType cylinderType, int cylinderDiameter, decimal treatmentLength)
        {
            List<Tuple<decimal, decimal>> tableTuples = new List<Tuple<decimal, decimal>>();
            if (cylinderType == CylinderType.VC)
            {
                if (cylinderDiameter == 20)
                {
                    tableTuples = tableFromConfig("VC_20_Lengths", "VC_20_Times");
                }
            }
           return getTableTime(tableTuples, treatmentLength);
        }

        public decimal estimateCylindricTreatmentTime()
        {
            CylinderType cylinderType = CylinderType.VC; // from GUI and plan?
            int cylinderDiameter = 20; // from GUI and plan?
            decimal prescribedDose = 1.0m; // from the treatment plan
            decimal tableDose = 1.0m; // from the app.config file
            decimal tableSourceStrength = 1.0m; // from the app.config file
            decimal currentSourceStrength = 1.0m; // from the treatment plan
            decimal treatmentLength = 10.0m; // GUI and plan
            decimal tableTime = timeFromTable(cylinderType, cylinderDiameter, treatmentLength);

            treatmentLength = 5.0m; // GUI and plan
            tableTime = timeFromTable(cylinderType, cylinderDiameter, treatmentLength);


            treatmentLength = 55.0m; // GUI and plan
            tableTime = timeFromTable(cylinderType, cylinderDiameter, treatmentLength);

            treatmentLength = 91.0m; // GUI and plan
            tableTime = timeFromTable(cylinderType, cylinderDiameter, treatmentLength);


            decimal estimatedTime = tableTime * (prescribedDose / tableDose) * (tableSourceStrength / currentSourceStrength);
            return estimatedTime;
        }


        private List<Tuple<decimal, decimal>> tableFromConfig(string tableLengthsName, string tableTimesName)
        {
            List<decimal> lengths = ConfigurationManager.AppSettings[tableLengthsName].Split(';').ToList().Select(decimal.Parse).ToList();
            List<decimal> times = ConfigurationManager.AppSettings[tableTimesName].Split(';').ToList().Select(decimal.Parse).ToList();
            List<Tuple<decimal, decimal>> tuples = new List<Tuple<decimal, decimal>>();
            int counter = 0;
            foreach (var item in lengths)
            {
                tuples.Add(new Tuple<decimal, decimal>(item, times[counter]));
                ++counter;
            }
            return tuples;
        }

        
    }
}
