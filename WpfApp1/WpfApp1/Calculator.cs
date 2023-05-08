using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            return needleLengthPlusProbeDistance() <= 195;
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



        private decimal treatmentTimeFromTable(decimal activeLength)
        {
            List<Tuple<decimal, decimal>> tableTuples = new List<Tuple<decimal, decimal>>();
            tableTuples = tableFromConfig("Active_lengths_mm", "Treatment_times_s");
            return getTableTime(tableTuples, activeLength);

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
                else if (cylinderDiameter == 25)
                {
                    tableTuples = tableFromConfig("VC_25_Lengths", "VC_25_Times");
                }
                else if (cylinderDiameter == 30)
                {
                    tableTuples = tableFromConfig("VC_30_Lengths", "VC_30_Times");
                }
                else if (cylinderDiameter == 35)
                {
                    tableTuples = tableFromConfig("VC_35_Lengths", "VC_35_Times");
                }
                else if (cylinderDiameter == 40)
                {
                    tableTuples = tableFromConfig("VC_40_Lengths", "VC_40_Times");
                }
            }
            else if (cylinderType == CylinderType.SVC)
            {
                if (cylinderDiameter == 25)
                {
                    tableTuples = tableFromConfig("SVC_25_Lengths", "SVC_25_Times");
                }
                else if (cylinderDiameter == 30)
                {
                    tableTuples = tableFromConfig("SVC_30_Lengths", "SVC_30_Times");
                }
                else if (cylinderDiameter == 35)
                {
                    tableTuples = tableFromConfig("SVC_35_Lengths", "SVC_35_Times");
                }
                else if (cylinderDiameter == 40)
                {
                    tableTuples = tableFromConfig("SVC_40_Lengths", "SVC_40_Times");
                }
            }
           return getTableTime(tableTuples, treatmentLength);
        }

        public decimal estimateCylindricTreatmentTime(CylinderType cylinderType, int cylinderDiameter, 
            decimal prescriptionDose, decimal currentSourceStrength, decimal treatmentLength)
        {
            return timeFromTable(cylinderType, cylinderDiameter, treatmentLength) *
                (prescriptionDose / 1.0m) *
                (40.0m / currentSourceStrength); 
        }

        public decimal estimateEsofagusTreatmentTime(decimal prescriptionDose, decimal activeLength, decimal currentSourceStrength)
        {
            return treatmentTimeFromTable(activeLength) *
                (prescriptionDose / 10.0m) *
                (40.0m / currentSourceStrength);
        }


        public decimal decayFactor(DateTime calibrationDateTime, DateTime currentDateTime)
        {
            // It was found that each individual iridium-192 source has a 
            // single decay constant and half-life ranging from 
            // 73.81 to 73.84 days with a mean value of 73.825 days.
            // The standard deviation was found to be± 0.0084.
            // decayConstant. However a half tim of 73.81 days is used
            // as it is used on the tcc plan.

            double halfTimeSec = 73.81 * 24.0 * 60.0 * 60.0;
            double decayConstant = Math.Log(2) / halfTimeSec;
            TimeSpan duration = currentDateTime - calibrationDateTime;
            double seconds = duration.TotalSeconds;
            return Convert.ToDecimal(Math.Exp(-1.0 * seconds * decayConstant));
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
