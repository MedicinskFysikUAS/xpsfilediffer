using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;


namespace WpfApp1
{
    class PageReader
    {
        private string xpsFilePath;
        //private List<Tuple<string, string>> _positonTimePairs;
        private List<LiveCatheter> _liveCatheters = new List<LiveCatheter>();

        public PageReader(string xpsFilePath)
        {
            this.xpsFilePath = xpsFilePath;
        }

        public void WriteOutXml(System.Xml.XmlReader xmlReader, string fileName)
        {
            var writer = System.Xml.XmlWriter.Create(fileName);
            writer.WriteNode(xmlReader, true);
        }

        public bool isFileType(XpsFileType xpsFileType)
        {
            XpsDocument _xpsDocument = new XpsDocument(xpsFilePath, System.IO.FileAccess.Read);
            IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            List<List<string>> pageList = new List<List<string>>();
            IXpsFixedPageReader _page = _document.FixedPages[0];
            System.Xml.XmlReader _pageContentReader = _page.XmlReader;
            List<string> stringsOnPage = new List<string>();

            if (_pageContentReader != null)
            {
                while (_pageContentReader.Read())
                {
                    if (_pageContentReader.Name == "Glyphs")
                    {
                        if (_pageContentReader.HasAttributes)
                        {
                            if (_pageContentReader.GetAttribute("UnicodeString") != null)
                            {
                                stringsOnPage.Add(_pageContentReader.
                                  GetAttribute("UnicodeString"));
                            }
                        }
                    }
                }
            }
            if (xpsFileType == XpsFileType.ONCENTRA_PROSTATE_TREATMENT_PLAN&& stringsOnPage.Count > 1 && stringsOnPage[0].StartsWith("Oncentra Prostate") && stringsOnPage[1] == "Treatment Plan")
            {
                return true;
            }
            else if (xpsFileType == XpsFileType.ONCENTRA_PROSTATE_DVH && stringsOnPage.Count > 1 && stringsOnPage[0].StartsWith("Oncentra Prostate") && stringsOnPage[1] == "DVH Evaluation")
            {
                return true;
            }
            else if (xpsFileType == XpsFileType.PROSTATE_TCC && stringsOnPage.Count > 90 && stringsOnPage[90] == "Förbehandlingsrapport")
            {
                return true;
            }
            else if (xpsFileType == XpsFileType.ONCENTRA_CYLINDER_TREATMENT_PLAN && stringsOnPage.Count > 51 && stringsOnPage[51] == "Oncentra Brachy ")
            {
                return true;
            }
            else if (xpsFileType == XpsFileType.CYLINDER_TCC && stringsOnPage.Count > 90 && stringsOnPage[90] == "Förbehandlingsrapport")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<List<string>> getPages()
        {
            XpsDocument _xpsDocument = new XpsDocument(xpsFilePath, System.IO.FileAccess.Read);
            IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            List<List<string>> pageList = new List<List<string>>();
            for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
            {
                IXpsFixedPageReader _page = _document.FixedPages[pageCount];
                System.Xml.XmlReader _pageContentReader = _page.XmlReader;
                //WriteOutXml(_pageContentReader, "xmlFile" + pageCount + ".xml");
                List<string> stringsOnPage = new List<string>();
                string startPageString = "Page number " + pageCount;
                stringsOnPage.Add(startPageString);

                if (_pageContentReader != null)
                {
                    while (_pageContentReader.Read())
                    {
                        if (_pageContentReader.Name == "Glyphs")
                        {
                            if (_pageContentReader.HasAttributes)
                            {
                                if (_pageContentReader.GetAttribute("UnicodeString") != null)
                                {
                                    stringsOnPage.Add(_pageContentReader.
                                      GetAttribute("UnicodeString"));
                                }
                            }
                        }
                    }
                }
                pageList.Add(stringsOnPage);
            }
            return pageList;
        }

        public void setTccLiveCatheters(TabType tabType)
        {
            XpsDocument _xpsDocument = new XpsDocument(xpsFilePath, System.IO.FileAccess.Read);
            IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            List<List<string>> pageList = new List<List<string>>();
            for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
            {
                IXpsFixedPageReader _page = _document.FixedPages[pageCount];
                System.Xml.XmlReader _pageContentReader = _page.XmlReader;
                if (_pageContentReader != null)
                {
                    bool readCatheterPositions = false;
                    string readCatheterPositionsAtY = "";
                    string channelNumber = "";
                    List<string> catheterPositions = new List<string>();

                    while (_pageContentReader.Read())
                    {
                        if (_pageContentReader.Name == "Glyphs")
                        {
                            if (_pageContentReader.HasAttributes)
                            {
                                /*
                                 *  Each entry in the list of indices comprises a glyph ID, optionally a comma, followed by an AdvanceWidth, and finally, delimited with a semi-colon. There is actually a lot more that could be present in Indices, but this is about the limit of what you'll see being pumped out by the XPS printer driver. I
                                 */

                                if (_pageContentReader.GetAttribute("UnicodeString") != null)
                                {  
                                    // If the previous Glyphs included the a unicode string starting with Kanal the next Glyphs will
                                    // include the indecies for the time positoins
                                    if (readCatheterPositions && readCatheterPositionsAtY == _pageContentReader.
                                        GetAttribute("OriginY"))
                                    {
                                        string originX = (_pageContentReader.
                                        GetAttribute("OriginX"));
                                        int startColumn = columnFromOrigX(originX, tabType);
                                        List<Tuple<int, double>> columnAndTimes = getColumnAndTimes(startColumn, _pageContentReader);
                                        setPositonTimePairs(channelNumber, catheterPositions, columnAndTimes);
                                        readCatheterPositions = false;
                                        readCatheterPositionsAtY = "";
                                        channelNumber = "";
                                    }

                                    string unicodeString = (_pageContentReader.
                                      GetAttribute("UnicodeString"));

                                    if (unicodeString.StartsWith("Kanal") && unicodeString.Length < 9)
                                    {
                                        readCatheterPositionsAtY = (_pageContentReader.
                                        GetAttribute("OriginY"));
                                        readCatheterPositions = true;
                                        channelNumber = unicodeString;
                                    }

                                    if (unicodeString.StartsWith("Stråln"))
                                    {
                                        catheterPositions = catheterPositionsInHeader(_pageContentReader);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int stringToInt(string stringValue)
        {
            double doubleValue = double.Parse(stringValue.Replace('.', ','));
            return Convert.ToInt32(doubleValue);
        }
        public double stringToDouble(string stringValue)
        {
            if (stringValue.Contains('.'))
            {
                return double.Parse(stringValue.Replace('.', ','));
            }
            else if (stringValue.Contains(','))
            {
                return double.Parse(stringValue);
            }
            else
            {
                return -1.0;
            }
        }

        public int columnFromOrigX(string originX, TabType tabType)
        {
            int startOrigin = -1;
            if (tabType == TabType.PROSTATE)
            {
                startOrigin = 6849; // Prostate
            }
            if (tabType == TabType.CYLINDER)
            {
                startOrigin = 6750; // Cylinder
            }

            int deltaWidth = 1833;
            int margin = 50;
            int originXint = stringToInt(originX);
            if ((originXint > ((startOrigin + deltaWidth * 0) - margin)) && (originXint < ((startOrigin + deltaWidth * 0) + margin)))
            {
                return 0;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 1) - margin)) && (originXint < ((startOrigin + deltaWidth * 1) + margin)))
            {
                return 1;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 2) - margin)) && (originXint < ((startOrigin + deltaWidth * 2) + margin)))
            {
                return 2;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 3) - margin)) && (originXint < ((startOrigin + deltaWidth * 3) + margin)))
            {
                return 3;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 4) - margin)) && (originXint < ((startOrigin + deltaWidth * 4) + margin)))
            {
                return 4;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 5) - margin)) && (originXint < ((startOrigin + deltaWidth * 5) + margin)))
            {
                return 5;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 6) - margin)) && (originXint < ((startOrigin + deltaWidth * 6) + margin)))
            {
                return 6;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 7) - margin)) && (originXint < ((startOrigin + deltaWidth * 7) + margin)))
            {
                return 7;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 8) - margin)) && (originXint < ((startOrigin + deltaWidth * 8) + margin)))
            {
                return 8;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 9) - margin)) && (originXint < ((startOrigin + deltaWidth * 9) + margin)))
            {
                return 9;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 10) - margin)) && (originXint < ((startOrigin + deltaWidth * 10) + margin)))
            {
                return 10;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 11) - margin)) && (originXint < ((startOrigin + deltaWidth * 11) + margin)))
            {
                return 11;
            }
            else if ((originXint > ((startOrigin + deltaWidth * 12) - margin)) && (originXint < ((startOrigin + deltaWidth * 12) + margin)))
            {
                return 12;
            }
            else
            {
                return -1;
            }

        }

        public bool newColumnForwidthDiff(int widthDiff)
        {
            return widthDiff > 100;
        }

            public int columnOffsetfromWidthDiff(int widthDiff)
        {
            int oneColumnWidth = 561;
            if (widthDiff < 100)
            {
                return 0;
            }
            else if (widthDiff < oneColumnWidth * 1)
            {
                return 1;
            }
            else if (widthDiff < oneColumnWidth * 2)
            {
                return 2;
            }
            else if (widthDiff < oneColumnWidth * 3)
            {
                return 3;
            }
            else if (widthDiff < oneColumnWidth * 4)
            {
                return 4;
            }
            else if (widthDiff < oneColumnWidth * 5)
            {
                return 5;
            }
            else if (widthDiff < oneColumnWidth * 6)
            {
                return 6;
            }
            else if (widthDiff < oneColumnWidth * 7)
            {
                return 7;
            }
            else if (widthDiff < oneColumnWidth * 8)
            {
                return 8;
            }
            else if (widthDiff < oneColumnWidth * 9)
            {
                return 9;
            }
            else if (widthDiff < oneColumnWidth * 10)
            {
                return 10;
            }
            else if (widthDiff < oneColumnWidth * 11)
            {
                return 11;
            }
            else if (widthDiff < oneColumnWidth * 12)
            {
                return 12;
            }
            else if (widthDiff < oneColumnWidth * 13)
            {
                return 13;
            }
            else
            {
                return -1;
            }
        }

        List<Tuple<int, double>> getColumnAndTimes(int startColumn, System.Xml.XmlReader pageContentReader)
        {
            string valueString = pageContentReader.GetAttribute("UnicodeString");

            string indicesString = pageContentReader.GetAttribute("Indices");
            List<string> Indices = indicesString.Split(';').ToList();
            int column = startColumn;
            List<int> columns = new List<int>();
            int firstWidth = 0;
            int widthDiff = 0;
            columns.Add(column);
            string valueStringWithColons = "";
            int counter = 0;
            foreach (var item in Indices)
            {
                if (item.Split(',').Length > 1)
                {
                    string AdvanceWidthStr = item.Split(',')[1].Trim();
                    widthDiff = stringToInt(AdvanceWidthStr) - firstWidth;
                    column += columnOffsetfromWidthDiff(widthDiff);
                    columns.Add(column);
                    firstWidth = stringToInt(AdvanceWidthStr);

                    if (counter < (valueString.Length))
                    {
                        valueStringWithColons += valueString[counter];
                    }

                    if (newColumnForwidthDiff(widthDiff))
                    {
                        valueStringWithColons += ";"; 
                    }
                }
                else
                {
                    if (counter < (valueString.Length))
                    {
                        valueStringWithColons += valueString[counter];
                    }

                }
                counter++;
            }

            List<int> distinctColumns = columns.Distinct().ToList();
            List<string> times = valueStringWithColons.Split(';').ToList();
            List<Tuple<int, double>> tuples = new List<Tuple<int, double>>();
            if (distinctColumns.Count == times.Count)
            {
                counter = 0;
                foreach (var item in distinctColumns)
                {
                    Tuple<int, double> tuple = new Tuple<int, double>(item, stringToDouble(times[counter]));
                    tuples.Add(tuple);
                    ++counter;
                }
            }
            return tuples;
        }

        public List<string> catheterPositionsInHeader(System.Xml.XmlReader pageContentReader)
        {
            string headerString = pageContentReader.GetAttribute("UnicodeString");
            int startIndex = headerString.IndexOf(')') + 1;
            string positionString = headerString.Substring(startIndex, headerString.Length - startIndex);
            startIndex = 0;
            int stringLength = 5;
            List<string> catheterPositions = new List<string>();
            while (startIndex < positionString.Length)
            {
                string substring = positionString.Substring(startIndex, stringLength);
                catheterPositions.Add(substring);
                startIndex += stringLength;
            }
            return catheterPositions;
        }

        public void addLiveCatheter(LiveCatheter liveCatheter)
        {
            int index = -1;
            int counter = 0;
            foreach (var item in _liveCatheters)
            {
                if (item.catheterNumber() == liveCatheter.catheterNumber())
                {
                    index = counter;
                    break;
                }
                ++counter;
            }

            if (index != -1)
            {
                LiveCatheter newLiveCatheter = _liveCatheters[index];
                newLiveCatheter.appendPositionTimePairs(liveCatheter.positonTimePairs());
                _liveCatheters[index] = newLiveCatheter;
            }
            else
            {
                _liveCatheters.Add(liveCatheter);
            }
        }

        public int channelNumberToInt(string channelNumber)
        {
            int channelNumberInt = -1;
            string channelNumberPrefix = "Kanal";
            if (channelNumberPrefix.Length < channelNumber.Length)
            {
                string channelNumberStr = channelNumber.Substring(channelNumberPrefix.Length, channelNumber.Length - channelNumberPrefix.Length).Trim();
                channelNumberInt = int.Parse(channelNumberStr);
            }
            return channelNumberInt;
        }


        public void setPositonTimePairs(string channelNumber, List<string>  catheterPositions, List<Tuple<int, double>> columnAndTimes)
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
            foreach (var item in columnAndTimes)
            {
                if (item.Item1 != -1)
                {
                    string position = catheterPositions[item.Item1];
                    double timeDouble = Math.Round(item.Item2, 1);
                    string time = String.Format("{0:0.0}", Convert.ToDecimal(timeDouble));
                    Tuple<string, string> tuple = new Tuple<string, string>(position, time);
                    positonTimePairs.Add(tuple);
                }
            }

            LiveCatheter liveCatheter = new LiveCatheter();
            liveCatheter.setPositonTimePairs(positonTimePairs);
            liveCatheter.setCatheterNumber(channelNumberToInt(channelNumber));
            addLiveCatheter(liveCatheter);
        }

        public List<LiveCatheter> tccLiveCatheters(TabType tabType)
        {
            this.setTccLiveCatheters(tabType);
            return _liveCatheters;
        }


    }


}


/*
 
  
 
     */