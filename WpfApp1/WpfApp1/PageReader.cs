using System;
using System.Collections.Generic;
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

        public PageReader(string xpsFilePath)
        {
            this.xpsFilePath = xpsFilePath;
        }

        public void WriteOutXml(System.Xml.XmlReader xmlReader, string fileName)
        {
            var writer = System.Xml.XmlWriter.Create(fileName);
            writer.WriteNode(xmlReader, true);
        }

        public List<List<string>> getPages()
        {
            //XpsDocument _xpsDocument = new XpsDocument(@"C:\work\git\xpsFileDiff\treatmentPlan.xps", System.IO.FileAccess.Read);
            XpsDocument _xpsDocument = new XpsDocument(xpsFilePath, System.IO.FileAccess.Read);
            IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            List<List<string>> pageList = new List<List<string>>();
            for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
            {
                IXpsFixedPageReader _page = _document.FixedPages[pageCount];
                //StringBuilder _currentText = new StringBuilder();
                System.Xml.XmlReader _pageContentReader = _page.XmlReader;
                //string xmlFilename = "xmlFileForPage" + pageCount + ".xml";
                //WriteOutXml(_pageContentReader, xmlFilename);
                List<string> stringsOnPage = new List<string>();
                string startPageString = "Page number " + pageCount;
                stringsOnPage.Add(startPageString);

                if (_pageContentReader != null)
                {
                    bool readCatherPositions = false;
                    string readCatherPositionsAtY = "";
                    List<string> catheterPositions = new List<string>();

                    while (_pageContentReader.Read())
                    {
                        string tmp = _pageContentReader.Name;
                        string tmp2 = _pageContentReader.Value;

                        if (_pageContentReader.Name == "Glyphs")
                        {
                            if (_pageContentReader.HasAttributes)
                            {
                                /*
                                 *  Each entry in the list of indices comprises a glyph ID, optionally a comma, followed by an AdvanceWidth, and finally, delimited with a semi-colon. There is actually a lot more that could be present in Indices, but this is about the limit of what you'll see being pumped out by the XPS printer driver. I
                                 */

                                if (_pageContentReader.GetAttribute("UnicodeString") != null)
                                {
                                    stringsOnPage.Add(_pageContentReader.
                                      GetAttribute("UnicodeString"));


                                    // Debug:
                                    string tmp3 = (_pageContentReader.
                                      GetAttribute("UnicodeString"));

                                    if (readCatherPositions && readCatherPositionsAtY == _pageContentReader.
                                        GetAttribute("OriginY"))
                                    {
                                        string originX = (_pageContentReader.
                                        GetAttribute("OriginX"));

                                        int startColumn = columnFromOrigX(originX);
                                        string timeValuesStr = (_pageContentReader.GetAttribute("UnicodeString"));
                                        List<Tuple<int, double>> columnAndTimes = getColumnAndTimes(startColumn, _pageContentReader);
                                        setPositonTimePairsTmp(catheterPositions, columnAndTimes);
                                        readCatherPositions = false;
                                        readCatherPositionsAtY = "";
                                    }


                                    if (tmp3.StartsWith("Kanal") && tmp3.Length < 9)
                                    {
                                        readCatherPositionsAtY = (_pageContentReader.
                                        GetAttribute("OriginY"));
                                        readCatherPositions = true;
                                    }


                                        if (tmp3.StartsWith("Stråln"))
                                    {
                                        catheterPositions = catheterPositionsInHeader(_pageContentReader);
                                        List<int> headerPositions = new List<int>();
                                        headerPositions = getPositions(_pageContentReader);
                                        // index 24 gives position for the ,-sign in the first column
                                        // every 5:th index gives the following columns
                                        // 12 columns
                                        int columnNummer = 3;
                                        int columnPoss = columnPosition(headerPositions, columnNummer);
                                    }

                                    if (tmp3 == "Kanal 1")
                                    {
                                        List<int> chanelPositions = new List<int>();
                                        chanelPositions = getPositions(_pageContentReader);
                                    }

                                    if (tmp3 == "0,70,71,01,52,0")
                                    {
                                        List<int> valuePositions = new List<int>();
                                        valuePositions = getPositions(_pageContentReader);
                                    }

                                    



                                }
                            }
                        }
                    }
                }
                pageList.Add(stringsOnPage);
            }

            return pageList;

        }

        public List<int> getPositions(System.Xml.XmlReader pageContentReader)
        {
            string debug = pageContentReader.GetAttribute("Indices");
            List<string> Indices = debug.Split(';').ToList();
            int nCharacters = Indices.Count();
            string origX = pageContentReader.GetAttribute("OriginX");
            List<int> positions = new List<int>();
            double dPositon = double.Parse(origX.Replace('.', ','));
            int positon = Convert.ToInt32(dPositon);
            foreach (var item in Indices)
            {
                if (item.Split(',').Length > 1)
                {
                    string AdvanceWidthStr = item.Split(',')[1].Trim();
                    positon += int.Parse(AdvanceWidthStr);
                    positions.Add(positon);
                }
            }
            return positions;
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

        public int columnPosition(List<int> headerPositions, int columnNummer)
        {
            return headerPositions[24 + columnNummer * 5];
        }

        public int columnFromOrigX(string originX)
        {
            int startOrigin = 6849;
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
            else
            {
                return -1;
            }
        }
            public int columnOffetfromWidthSum(int widthSum)
        {
            int deltaSum = 561;
            if ((widthSum > 0 && (widthSum < 100)))
            {
                return 0;
            }
            else if ((widthSum > 100) && (widthSum < (deltaSum * 1)))
            {
                return 1;
            }
            else if ((widthSum > (deltaSum * 1)) && (widthSum < (deltaSum * 2)))
            {
                return 2;
            }
            else if ((widthSum > (deltaSum * 2)) && (widthSum < (deltaSum * 3)))
            {
                return 3;
            }
            else if ((widthSum > (deltaSum * 3)) && (widthSum < (deltaSum * 4)))
            {
                return 4;
            }
            else if ((widthSum > (deltaSum * 4)) && (widthSum < (deltaSum * 5)))
            {
                return 5;
            }
            else if ((widthSum > (deltaSum * 5)) && (widthSum < (deltaSum * 6)))
            {
                return 6;
            }
            else if ((widthSum > (deltaSum * 6)) && (widthSum < (deltaSum * 7)))
            {
                return 7;
            }
            else if ((widthSum > (deltaSum * 7)) && (widthSum < (deltaSum * 8)))
            {
                return 8;
            }
            else if ((widthSum > (deltaSum * 8)) && (widthSum < (deltaSum * 9)))
            {
                return 9;
            }
            else if ((widthSum > (deltaSum * 9)) && (widthSum < (deltaSum * 10)))
            {
                return 10;
            }
            else if ((widthSum > (deltaSum * 10)) && (widthSum < (deltaSum * 11)))
            {
                return 11;
            }
            else if ((widthSum > (deltaSum * 11)) && (widthSum < (deltaSum * 12)))
            {
                return 12;
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

        public void setPositonTimePairsTmp(List<string>  catheterPositions, List<Tuple<int, double>> columnAndTimes)
        {
            List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
            foreach (var item in columnAndTimes)
            {
                string position = catheterPositions[item.Item1];
                double timeDouble = Math.Round(item.Item2, 1);
                string time = String.Format("{0:0.0}", Convert.ToDecimal(timeDouble));
                Tuple<string, string> tuple = new Tuple<string, string>(position, time);
                positonTimePairs.Add(tuple);
            }

        }

    }

   
}


/*
 
  
 
     */