using System;
using System.Collections.Generic;
using System.Text;

namespace archiveXps
{
    class XpsFileInfo
    {
        private string _outputDirectoryName;
        private string _planCode;

        public string OutputDirectoryName { get => _outputDirectoryName; set => _outputDirectoryName = value; }
        public string PlanCode { get => _planCode; set => _planCode = value; }
    }
}
