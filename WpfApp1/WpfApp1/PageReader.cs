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
                                    string debug = _pageContentReader.GetAttribute("Indices");
                                    List<string> Indices = debug.Split(';').ToList();
                                    int nCharacters = Indices.Count();
                                    string origX = _pageContentReader.GetAttribute("OriginX");
                                    List<int> positions = new List<int>();
                                    int positon = 0;
                                    foreach (var item in Indices)
                                    {
                                        if (item.Split(',').Length > 1)
                                        {
                                            string AdvanceWidthStr = item.Split(',')[1].Trim();
                                            positon += int.Parse(AdvanceWidthStr);
                                            positions.Add(positon);
                                        }
                                    }

                                    stringsOnPage.Add(_pageContentReader.
                                      GetAttribute("UnicodeString"));

                                    string tmp3 = (_pageContentReader.
                                      GetAttribute("UnicodeString"));
                                    if (tmp3 == "Kanal 1")
                                    {
                                        string tmp4 = "hittad";
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

    }

   
}
