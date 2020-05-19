using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class LiveCatheter
    {
        private List<Tuple<string, string>> _positonTimePairs;
        private int _catheterNumber;

        public void setPositonTimePairs(List<Tuple<string, string>> positonTimePairs)
        {
            _positonTimePairs = positonTimePairs;
        }

        public void setCatheterNumber(int catheterNumber)
        {
            _catheterNumber = catheterNumber;
        }

        public List<Tuple<string, string>> positonTimePairs()
        {
            return _positonTimePairs;
        }

        public int catheterNumber()
        {
            return _catheterNumber;
        }
    }
}
