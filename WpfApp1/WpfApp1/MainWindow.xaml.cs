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
using System.Data;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<List<string>> _resultRows = new List<List<string>>();
        string _treatmentPlanXpsFilePath;
        string _dvhXpsFilePath;
        string _tccPlanXpsFilePath;
        Specifications _specifications;
        public MainWindow()
        {
            InitializeComponent();
            setLabelAndTextboxVisable(false);
            resultSummaryLabel.Content = "";
            _specifications = new Specifications();
        }

        public void setLabelAndTextboxVisable(bool setLabelAndTextboxVisable)
        {
            if (setLabelAndTextboxVisable)
            {
                needleDepthLabel.Visibility = Visibility.Visible;
                needleDepthText.Visibility = Visibility.Visible;
                freeLengthLabel.Visibility = Visibility.Visible;
                freeLengthText.Visibility = Visibility.Visible;
                needleLengthProbSumLabel.Visibility = Visibility.Visible;
                needleLengthProbSumText.Visibility = Visibility.Visible;
                calculatedLabel.Visibility = Visibility.Visible;
            }
            else
            {
                needleDepthLabel.Visibility = Visibility.Hidden;
                needleDepthText.Visibility = Visibility.Hidden;
                freeLengthLabel.Visibility = Visibility.Hidden;
                freeLengthText.Visibility = Visibility.Hidden;
                needleLengthProbSumLabel.Visibility = Visibility.Hidden;
                needleLengthProbSumText.Visibility = Visibility.Hidden;
                calculatedLabel.Visibility = Visibility.Hidden;
            }
        }

        public void calculateLengthAndFreeLength()
        {
            Calculator calculator = new Calculator();
            StringExtractor stringExtractor = new StringExtractor();
            if (needleLengthText.Text.Length > 0 && probeDistanceText.Text.Length > 0)
            {
                calculator.NeedleLength = stringExtractor.decimalStringToDecimal(needleLengthText.Text);
                calculator.ProbeDistance = stringExtractor.decimalStringToDecimal(probeDistanceText.Text);
                needleDepthText.Text = calculator.needleDepth().ToString();
                freeLengthText.Text = calculator.freeLength().ToString();
                needleLengthProbSumText.Text = calculator.needleLengthPlusProbeDistance().ToString();
                if (calculator.sufficientNeedleDepth())
                {
                    needleLengthProbSumText.Background = Brushes.Green;
                }
                else
                {
                    needleLengthProbSumText.Background = Brushes.Red;
                }
                setLabelAndTextboxVisable(true);
            }
        }

        public void updateSpecifications()
        {
            Calculator calculator = new Calculator();
            StringExtractor stringExtractor = new StringExtractor();
            if (needleLengthText.Text.Length > 0 && probeDistanceText.Text.Length > 0)
            {
                calculator.NeedleLength = stringExtractor.decimalStringToDecimal(needleLengthText.Text);
                calculator.ProbeDistance = stringExtractor.decimalStringToDecimal(probeDistanceText.Text);
                _specifications.NeedleDepth = calculator.needleDepth();
                _specifications.FreeLength = calculator.freeLength();
                _specifications.PrescriptionDose = stringExtractor.decimalStringToDecimal(prescribedDoseText.Text);
            }
        }

        public bool needleDepthAndFreeLengthIsSet()
        {
            return ((needleDepthText.Visibility == Visibility.Visible) && (freeLengthText.Visibility == Visibility.Visible));
        }

        void buildResultDataGrid()
        {
            _resultRows.Clear();
            resultSummaryLabel.Content = "";

            if (_treatmentPlanXpsFilePath != null && needleDepthAndFreeLengthIsSet())
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList);
                Comparator comparator = new Comparator(_specifications); 
                comparator.treatmentPlan = treatmentPlan;
                _resultRows.AddRange(comparator.treatmentPlanResultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _dvhXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList);
                PageReader treatmentDvhPageReader = new PageReader(_dvhXpsFilePath);
                List<List<string>> treatmentPlanDvhPageList = treatmentDvhPageReader.getPages();
                TreatmentDvh treatmentDvh = new TreatmentDvh(treatmentPlanDvhPageList);
                Comparator comparator = new Comparator(_specifications);
                comparator.treatmentPlan = treatmentPlan;
                comparator.treatmentDvh = treatmentDvh;
                _resultRows.AddRange(comparator.treatmentPlanAndDvhResultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList);
                Comparator comparator = new Comparator(_specifications);
                comparator.treatmentPlan = treatmentPlan;
                //PageReader dvhPageReader = new PageReader(_dvhXpsFilePath);
                //List<List<string>> dvhPageList = dvhPageReader.getPages();
                //dvh dvh = new dvh(treatmentPlanPageList);
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters();
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                //_resultRows = comparator.resultRows();
                _resultRows.AddRange(comparator.resultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _dvhXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList);
                PageReader treatmentDvhPageReader = new PageReader(_dvhXpsFilePath);
                List<List<string>> treatmentPlanDvhPageList = treatmentDvhPageReader.getPages();
                TreatmentDvh treatmentDvh = new TreatmentDvh(treatmentPlanDvhPageList);
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters();
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                Comparator comparator = new Comparator(_specifications);
                comparator.treatmentPlan = treatmentPlan;
                comparator.treatmentDvh = treatmentDvh;
                comparator.tccPlan = tccPlan;
                _resultRows.AddRange(comparator.allXpsResultRows());
            }
            DataColumn testCase= new DataColumn("Test", typeof(string));
            DataColumn testResult = new DataColumn("Result", typeof(string));
            DataColumn resultDescripton = new DataColumn("Beskriving", typeof(string));
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(testCase);
            dataTable.Columns.Add(testResult);
            dataTable.Columns.Add(resultDescripton);
            foreach (var resultRow in _resultRows)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow[0] = resultRow[0];
                dataRow[1] = resultRow[1];
                dataRow[2] = resultRow[2];
                dataTable.Rows.Add(dataRow);
            }
            ResultDataGrid.ItemsSource = dataTable.DefaultView;
            ResultDataGrid.UpdateLayout();

            int counter = 0;
            int nOk = 0;
            int nErrors = 0;
            foreach (var item in ResultDataGrid.ItemsSource)
            {
                DataGridRow row = (DataGridRow)ResultDataGrid.ItemContainerGenerator.ContainerFromIndex(counter);
                if (row != null)
                {
                    DataGridCell cellInSecondColumn = ResultDataGrid.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    string tmp = cellInSecondColumn.ToString();
                    if (cellInSecondColumn.ToString() == "System.Windows.Controls.DataGridCell: OK")
                    {
                        cellInSecondColumn.Background = Brushes.Green;
                        ++nOk;
                    }
                    else if (cellInSecondColumn.ToString() == "System.Windows.Controls.DataGridCell: Inte OK")
                    {
                        cellInSecondColumn.Background = Brushes.Red;
                        ++nErrors;
                    }
                }
                ++counter;
            }

            if (nOk + nErrors > 0)
            {
                if (nErrors == 0)
                {
                    resultSummaryLabel.Content = "Alla test var OK";
                    resultSummaryLabel.Background = Brushes.Green;
                }
                else
                {
                    resultSummaryLabel.Content = "Alla test var INTE OK";
                    resultSummaryLabel.Background = Brushes.Red;
                }
            }
        }




        private void BtnOpenTPFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _treatmentPlanXpsFilePath = openFileDialog.FileName;
                TPXpsPathLabel.Content = _treatmentPlanXpsFilePath;
            }
        }

        private void BtnOpenDVHFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _dvhXpsFilePath = openFileDialog.FileName;
                DVHXpsPathLabel.Content = _dvhXpsFilePath;
            }
        }

        private void BtnOpenTCCFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _tccPlanXpsFilePath = openFileDialog.FileName;
                TCCXpsPathLabel.Content = _tccPlanXpsFilePath;
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            updateSpecifications();
            calculateLengthAndFreeLength();
            this.buildResultDataGrid();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}

//Text exists in Glyphs -> UnicodeString string attribute. You have to use XMLReader for fixed page.
// https://stackoverflow.com/questions/12262197/extract-text-from-a-xps-document