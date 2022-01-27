using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class IntrauterineCatheter
    {
        private string _catheterNumber;
        IntrauterineCatheterType _IntrauterineCatheterType;
        private decimal _catheterLength;

        public string CatheterNumber { get => _catheterNumber; set => _catheterNumber = value; }
        public IntrauterineCatheterType IntrauterineCatheterType { get => _IntrauterineCatheterType; set => _IntrauterineCatheterType = value; }
        public decimal CatheterLength { get => _catheterLength; set => _catheterLength = value; }
    }
}
