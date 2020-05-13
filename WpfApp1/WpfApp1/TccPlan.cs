using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class TccPlan
    {
        private List<List<string>> _pageList;

        public TccPlan(List<List<string>> pageList)
        {
            _pageList = pageList;
        }

        public string patientFirstName()
        {
            return "Roine";
        }

        public string patientLastName()
        {
            return "Österberg";
        }
    }
}
