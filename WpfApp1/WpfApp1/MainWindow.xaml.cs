using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string xpsFilePath = @"C:\work\git\xpsFileDiff\treatmentPlan.xps";
            PageReader pageReader = new PageReader(xpsFilePath);
            List<List<string>> pageList = pageReader.getPages();
            TreatmentPlan treatmentPlan = new TreatmentPlan(pageList);
            //string patientFirstName = treatmentPlan.patientFirstName();
            //string patientLastName = treatmentPlan.patientLastName();
            TccPlan tccPlan = new TccPlan(pageList);
            //string tccPatientFirstName = tccPlan.patientFirstName();
            //string tccPatientLastName = tccPlan.patientLastName();
            Comparator comparator = new Comparator(treatmentPlan, tccPlan);
            if (comparator.hasSamePatientName())
            {
                Console.WriteLine("Same name!");
            }
            else
            {
                Console.WriteLine("Different name!");

            }




            //// Orig:
            //XpsDocument _xpsDocument = new XpsDocument(@"C:\work\git\xpsFileDiff\treatmentPlan.xps", System.IO.FileAccess.Read);
            //IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            //IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            //FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            //string _fullPageText = "";

            //for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
            //{
            //    IXpsFixedPageReader _page = _document.FixedPages[pageCount];
            //    StringBuilder _currentText = new StringBuilder();
            //    System.Xml.XmlReader _pageContentReader = _page.XmlReader;

            //    if (_pageContentReader != null)
            //    {
            //        while (_pageContentReader.Read())
            //        {
            //            string tmp = _pageContentReader.Name;
            //            string tmp2 = _pageContentReader.Value;
            //            if (_pageContentReader.Name == "Glyphs")
            //            {
            //                if (_pageContentReader.HasAttributes)
            //                {

            //                    if (_pageContentReader.GetAttribute("UnicodeString") != null)
            //                    {
            //                        _currentText.
            //                          Append(_pageContentReader.
            //                          GetAttribute("UnicodeString"));
            //                        _currentText.Append(Environment.NewLine.ToString());
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    _fullPageText += _currentText.ToString();
            //    string[] textStrings = _currentText.ToString().Split(Environment.NewLine.ToCharArray());
            //}
            // Orig end



            //XpsDocument _xpsDocument = new XpsDocument(@"C:\work\git\xpsFileDiff\exempelXpsDokument.xps", System.IO.FileAccess.Read);
            //IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            //IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            //FixedDocumentSequence sequence = _xpsDocument.GetFixedDocumentSequence();
            //string _fullPageText = "";

            //for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
            //{
            //    IXpsFixedPageReader _page = _document.FixedPages[pageCount];
            //    StringBuilder _currentText = new StringBuilder();
            //    System.Xml.XmlReader _pageContentReader = _page.XmlReader;

            //    if (_pageContentReader != null)
            //    {
            //        while (_pageContentReader.Read())
            //        {
            //            if (_pageContentReader.Name == "Glyphs")
            //            {
            //                if (_pageContentReader.HasAttributes)
            //                {
            //                    if (_pageContentReader.GetAttribute("UnicodeString") != null)
            //                    {
            //                        _currentText.
            //                          Append(_pageContentReader.
            //                          GetAttribute("UnicodeString"));
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    _fullPageText += _currentText.ToString();
            //}


            //XpsDocument _xpsDocument = new XpsDocument("C:/work/git/xpsFileDiff/exempelXpsDokument.xps", System.IO.FileAccess.Read);
            //        IXpsFixedDocumentSequenceReader fixedDocSeqReader = _xpsDocument.FixedDocumentSequenceReader;
            //        //IXpsFixedDocumentSequenceReader fixedDocSeqReader
            //        //    = _xpsDocument.FixedDocumentSequenceReader;
            //        IXpsFixedDocumentReader _document = fixedDocSeqReader.FixedDocuments[0];
            //        IXpsFixedPageReader _page = _document.FixedPages[0];
            //        //IXpsFixedPageReader _page
            //        //    = _document.FixedPages[documentViewerElement.MasterPageNumber];
            //        StringBuilder _currentText = new StringBuilder();
            //        System.Xml.XmlReader _pageContentReader = _page.XmlReader;
            //        if (_pageContentReader != null)
            //        {
            //            while (_pageContentReader.Read())
            //            {
            //                //Console.WriteLine(_pageContentReader.Name);
            //                Console.WriteLine("------------------- ersa ---------------");
            //                if (_pageContentReader.Name == "Glyphs")
            //                //if (_pageContentReader.Name == "xml")
            //                {
            //                    if (_pageContentReader.HasAttributes)
            //                    {
            //                        if (_pageContentReader.GetAttribute("UnicodeString") != null)
            //                        {
            //                            _currentText.
            //                              Append(_pageContentReader.
            //                              GetAttribute("UnicodeString"));
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        string _fullPageText = _currentText.ToString();


        }
    }
}

//Text exists in Glyphs -> UnicodeString string attribute. You have to use XMLReader for fixed page.
// https://stackoverflow.com/questions/12262197/extract-text-from-a-xps-document