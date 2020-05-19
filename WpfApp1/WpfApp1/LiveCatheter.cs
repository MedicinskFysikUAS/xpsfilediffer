using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class LiveCatheter
    {
        private List<Tuple<string, string>> _positonTimePairs;

        public void addPositonTimePair(Tuple<string, string> positonTimePair)
        {
            _positonTimePairs.Add(positonTimePair);
        }

        public List<Tuple<string, string>> positonTimePairs()
        {
            return _positonTimePairs;
        }
    }
}
