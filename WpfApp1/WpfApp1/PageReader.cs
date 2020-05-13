using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                StringBuilder _currentText = new StringBuilder();
                System.Xml.XmlReader _pageContentReader = _page.XmlReader;
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

    }

   
}
