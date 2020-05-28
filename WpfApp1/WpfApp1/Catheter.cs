using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class Catheter
    {
        private int _catheterNumber;
        private string _depth;
        private string _freeLength;
        private string _offset;
        private string _tipField;
        private bool _isActiveLocked;
        private bool _isTimeLocked;

        
        public void setCatheterNumber(int catheterNumber)
        {
            _catheterNumber = catheterNumber;
        }
                
        public int catheterNumber()
        {
            return _catheterNumber;
        }
    }
}
