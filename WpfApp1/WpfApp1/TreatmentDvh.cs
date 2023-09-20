using System;
using System.Collections.Generic;

namespace WpfApp1
{
    public class TreatmentDvh
    {
        private List<List<string>> _pageList;
        private StringExtractor _stringExtractor = new StringExtractor();

        public TreatmentDvh(List<List<string>> pageList)
        {
            _pageList = pageList;
        }

        public decimal PrescribedDose()
        {
            int pageIndex = 0;
            string stringValue = _stringExtractor.getValueBetweenSearchStrings(_pageList[pageIndex], "Prescribed Dose:", "Gy");
            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
            }
            return Convert.ToDecimal(stringValue);
        }

        public decimal PtvVolume()
        {
            int pageIndex = 0;
            string stringValue = _stringExtractor.getValueBetweenSearchStringsUsingContains(_pageList[pageIndex], "PTV (CTV1):V =", "mm");
            if (stringValue.Length == 0)
            {
                stringValue = _stringExtractor.getValueBetweenSearchStringsUsingContains(_pageList[pageIndex], "PTV (CTV2):V =", "mm");
            }
            if (stringValue.Length == 0)
            {
                pageIndex = 1;
                stringValue = _stringExtractor.getValueBetweenSearchStringsUsingContains(_pageList[pageIndex], "PTV (CTV1):V =", "mm");
            }
            if (stringValue.Length == 0)
            {
                stringValue = _stringExtractor.getValueBetweenSearchStringsUsingContains(_pageList[pageIndex], "PTV (CTV2):V =", "mm");
            }
            if (stringValue.Contains('.'))
            {
                stringValue = stringValue.Replace('.', ',');
                decimal number;
                if (!Decimal.TryParse(stringValue, out number))
                {
                    stringValue = "-1.0";
                }
            }
            return Convert.ToDecimal(stringValue);
        }

        public string planCode()
        {
            int pageIndex = 0;
            string stringValue = "";
            stringValue = _stringExtractor.getValueFromSpaceSeparetedString(_pageList[pageIndex], "Plan Code:", 0);
            return stringValue;
        }



    }
}
