using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class CatheterPositonAndTimeTable
    {
        int _pageIndex;
        int _startPageIndex;
        int _stopPageIndex;
        int _startIndex;
        int _stopIndex;

        public int pageIndex()
        {
            return _pageIndex;
        }
        public int startPageIndex()
        {
            return _startPageIndex;
        }
        public int stopPageIndex()
        {
            return _stopPageIndex;
        }

        public int startIndex()
        {
            return _startIndex;
        }

        public int stopIndex()
        {
            return _stopIndex;
        }

        public void setPageIndex(int pageIndex)
        {
            _pageIndex = pageIndex;
        }

        public void setStartPageIndex(int pageIndex)
        {
            _startPageIndex = pageIndex;
        }

        public void setStopPageIndex(int pageIndex)
        {
            _stopPageIndex = pageIndex;
        }

        public void setStartIndex(int startIndex)
        {
            _startIndex = startIndex;
        }

        public void setStopIndex(int stopIndex)
        {
            _stopIndex = stopIndex;
        }
    }
}
