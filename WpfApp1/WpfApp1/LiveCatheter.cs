﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class LiveCatheter
    {
        private List<Tuple<string, string>> _positonTimePairs = new List<Tuple<string, string>>();
        private int _catheterNumber;
        private decimal _offsetLength;
        private bool _isPipe;
        private decimal _channelLength;

        public bool IsPipe { get => _isPipe; set => _isPipe = value; }
        public decimal ChannelLength { get => _channelLength; set => _channelLength = value; }

        public void setPositonTimePairs(List<Tuple<string, string>> positonTimePairs)
        {
            _positonTimePairs = positonTimePairs;
        }

        public void addPositonTimePairs(List<Tuple<string, string>> positonTimePairs)
        {
            _positonTimePairs.AddRange(positonTimePairs);
        }

        public void appendPositionTimePairs(List<Tuple<string, string>> positonTimePairs)
        {
            _positonTimePairs.AddRange(positonTimePairs);
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

        public void setOffsetLength(decimal offsetLength)
        {
            _offsetLength = offsetLength;
        }

        public decimal offsetLength()
        {
            return _offsetLength;
        }
    }
}
