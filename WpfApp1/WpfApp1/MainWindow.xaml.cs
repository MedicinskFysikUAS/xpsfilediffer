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
using System.Configuration;

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

        string _treatmentPlanXpsFilePathProstate;
        string _dvhXpsFilePathProstate;
        string _tccPlanXpsFilePathProstate;

        string _treatmentPlanXpsFilePathCylinder;
        string _tccPlanXpsFilePathCylinder; 

        //string _treatmentPlanXpsFilePathIntraUterine;
        //string _dvhXpsFilePathIntraUterine;
        string _tccPlanXpsFilePathIntraUterine;
        private TabType _tabType;
        private List<int> _comboboxDiameters;
        private bool _isSameSource;
        private bool _sameSourceSet;


        List<LiveCatheter> _treatmentPlanLiveCatheters = new List<LiveCatheter>();
        List<LiveCatheter> _treatmentPlanLiveCathetersCorr = new List<LiveCatheter>();
        List<LiveCatheter> _tccPlanLiveCatheters = new List<LiveCatheter>();

        Specifications _specifications;
        public MainWindow()
        {
            InitializeComponent();
            setProstateCalculationsVisable(false);
            setCylinderCalculationsVisable(false);
            catheterInfoButton.Visibility = Visibility.Hidden;
            resultSummaryLabel.Content = "";
            _specifications = new Specifications();
            initiateCylinderTypeComboBox();
             _comboboxDiameters = new List<int>();
            initiateSameSourceCombobox();
            // debug
            //PageReader tccPlanPageReader = new PageReader("C:\\work\\git\\xpsfilediffer\\xpsFilerTcc1\\prostateTcc3.xps");
            //List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
            //List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(TabType.PROSTATE);
            // end
        }
    // https://stackoverflow.com/questions/23499105/c-sharp-app-config-with-array-or-list-like-data

    public void setProstateCalculationsVisable(bool setVisable)
        {
            if (setVisable)
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

        public void setCylinderCalculationsVisable(bool setVisable)
        {
            if (setVisable)
            {
                estimatedTreatmentTimeLabel.Visibility = Visibility.Visible;
                needleDepthText.Visibility = Visibility.Visible;
                calculatedLabel1.Visibility = Visibility.Visible;
            }
            else
            {
                estimatedTreatmentTimeLabel.Visibility = Visibility.Hidden;
                estimatedTreatmentTimeText.Visibility = Visibility.Hidden;
                calculatedLabel1.Visibility = Visibility.Hidden;
            }
        }

        public void initiateCylinderTypeComboBox()
        {
            cylinderTypeComboBox.Items.Add("VC");
            cylinderTypeComboBox.Items.Add("SVC/AVC");
        }

        public void initiateSameSourceCombobox()
        {
            sameSourceCombobox0.Items.Add("Ja");
            sameSourceCombobox0.Items.Add("Nej");
            sameSourceCombobox1.Items.Add("Ja");
            sameSourceCombobox1.Items.Add("Nej");
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
                setProstateCalculationsVisable(true);
            }
        }

        // Returns all data needed to estimate treatment time expect current source strength
        public DataForTreatmentTimeEstimate getDataForTreatmentTimeEstimate()
        {
            StringExtractor stringExtractor = new StringExtractor();
            CylinderType cylinderType = CylinderType.VC;
            if (cylinderTypeComboBox.SelectedIndex == 0)
            {
                cylinderType = CylinderType.VC; 
            }
            else if (cylinderTypeComboBox.SelectedIndex == 1)
            {
                cylinderType = CylinderType.SVC;
            }
            int cylinderDiameter = _comboboxDiameters[cylinderDiameterComboBox.SelectedIndex];
            decimal prescriptionDose = stringExtractor.decimalStringToDecimal(cylindricPrescribedDoseText.Text);
            decimal treatmentLength = stringExtractor.decimalStringToDecimal(treatmentLengthText.Text);
           
            DataForTreatmentTimeEstimate dataForTreatmentTimeEstimate = new DataForTreatmentTimeEstimate();
            dataForTreatmentTimeEstimate.CylinderType = cylinderType;
            dataForTreatmentTimeEstimate.CylinderDiameter = cylinderDiameter;
            dataForTreatmentTimeEstimate.PrescriptionDose = prescriptionDose;
            dataForTreatmentTimeEstimate.TreatmentLength = treatmentLength;
            return dataForTreatmentTimeEstimate;
        }

        public void updateSpecifications()
        {
            Calculator calculator = new Calculator();
            StringExtractor stringExtractor = new StringExtractor();
            if (_tabType == TabType.PROSTATE)
            {
                _specifications.ExpectedChannelLength = 1190.0m;
                if (needleLengthText.Text.Length > 0 && probeDistanceText.Text.Length > 0)
                {
                    calculator.NeedleLength = stringExtractor.decimalStringToDecimal(needleLengthText.Text);
                    calculator.ProbeDistance = stringExtractor.decimalStringToDecimal(probeDistanceText.Text);
                    _specifications.NeedleDepth = calculator.needleDepth();
                    _specifications.FreeLength = calculator.freeLength();
                }
                if (prescribedDoseText.Text.Length > 0)
                {
                    _specifications.PrescriptionDose = stringExtractor.decimalStringToDecimal(prescribedDoseText.Text);
                }
            }
            else if (_tabType == TabType.CYLINDER)
            {
                _specifications.ExpectedChannelLength = 1300.0m;
                if (prescriptionDoseIsSetCylinder())
                {
                    _specifications.PrescriptionDoseCylinder = stringExtractor.decimalStringToDecimal(cylindricPrescribedDoseText.Text);
                }
            }
        }

        public bool needleDepthAndFreeLengthIsSet()
        {
            return ((needleDepthText.Visibility == Visibility.Visible) && (freeLengthText.Visibility == Visibility.Visible));
        }

        public bool prescriptionDoseIsSet()
        {
            return (prescribedDoseText.Text.Length > 0);
        }

        public bool prescriptionDoseIsSetCylinder()
        {
            return (cylindricPrescribedDoseText.Text.Length > 0);
        }

        public bool cylinderTypeDiamterLengthAndDoseIsSet()
        {
            bool typIsSet = cylinderTypeComboBox.SelectedIndex != -1;
            bool diameterIsSet = cylinderDiameterComboBox.SelectedIndex != -1;
            return (typIsSet && diameterIsSet && treatmentLengthText.Text.Length > 0 && cylindricPrescribedDoseText.Text.Length > 0);
        }

        private void updateInputFilePaths()
        {
            if (ProstateTab.IsSelected)
            {
                _treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathProstate;
                _dvhXpsFilePath = _dvhXpsFilePathProstate;
                _tccPlanXpsFilePath = _tccPlanXpsFilePathProstate;
            }
            else if (CylinderTab.IsSelected)
            {
                _treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathCylinder;
                _dvhXpsFilePath = "";
                _tccPlanXpsFilePath = _tccPlanXpsFilePathCylinder;
            }
            else if (IntraUterineTab.IsSelected)
            {
                //_treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathIntraUterine;
                //_dvhXpsFilePath = _dvhXpsFilePathIntraUterine;
                //_tccPlanXpsFilePath = _tccPlanXpsFilePathIntraUterine;
            }
        }

        private bool sameSourceProstate()
        {
            return sameSourceCombobox0.SelectedIndex == 0;
        }

        private bool sameSourceSetProstate()
        {
            return sameSourceCombobox0.SelectedIndex != -1;
        }

        private bool sameSourceSetCylinder()
        {
            return sameSourceCombobox1.SelectedIndex != -1;
        }

        private bool sameSourceCylinder()
        {
            return sameSourceCombobox1.SelectedIndex == 0;
        }

        private void updateSameSourceSelected()
        {
            if (ProstateTab.IsSelected)
            {
                _isSameSource = sameSourceProstate();
                _sameSourceSet = sameSourceSetProstate();
            }
            else if (CylinderTab.IsSelected)
            {
                _isSameSource = sameSourceCylinder();
                _sameSourceSet = sameSourceSetCylinder();
            }
            else if (IntraUterineTab.IsSelected)
            {
                //_treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathIntraUterine;
                //_dvhXpsFilePath = _dvhXpsFilePathIntraUterine;
                //_tccPlanXpsFilePath = _tccPlanXpsFilePathIntraUterine;
            }
        }

        private bool correctProstateFileType()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_treatmentPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_TREATMENT_PLAN))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig Oncentra Prostate fil."));
                    return false;
                }
            }
            if (_dvhXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_dvhXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_DVH))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig Oncentra DVH fil."));
                    return false;
                }
            }
            if (_tccPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_tccPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.PROSTATE_TCC))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig TCC fil."));
                    return false;
                }
            }
            return true;
        }
        private bool correctCylinderFileType()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_treatmentPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ONCENTRA_CYLINDER_TREATMENT_PLAN))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig Oncentra Brachy fil."));
                    return false;
                }
            }
            
            if (_tccPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_tccPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.CYLINDER_TCC))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig TCC fil."));
                    return false;
                }
            }
            return true;
        }

            private void addProstateResultRows()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null && needleDepthAndFreeLengthIsSet())
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                _resultRows.AddRange(comparator.prostateTreatmentPlanResultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _dvhXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                PageReader treatmentDvhPageReader = new PageReader(_dvhXpsFilePath);
                List<List<string>> treatmentPlanDvhPageList = treatmentDvhPageReader.getPages();
                TreatmentDvh treatmentDvh = new TreatmentDvh(treatmentPlanDvhPageList);
                comparator.treatmentPlan = treatmentPlan;
                comparator.treatmentDvh = treatmentDvh;
                _resultRows.AddRange(comparator.treatmentPlanAndDvhResultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                _resultRows.AddRange(comparator.resultRows());
                if (_sameSourceSet)
                {
                    _resultRows.AddRange(comparator.sourceComparisonResultRows(_isSameSource));
                }
                if (prescriptionDoseIsSet())
                {
                    _resultRows.AddRange(comparator.prescriptionDoseResultRows());
                }
            }

                if (_treatmentPlanXpsFilePath != null && _dvhXpsFilePath != null && _tccPlanXpsFilePath != null && prescriptionDoseIsSet())
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                PageReader treatmentDvhPageReader = new PageReader(_dvhXpsFilePath);
                List<List<string>> treatmentPlanDvhPageList = treatmentDvhPageReader.getPages();
                TreatmentDvh treatmentDvh = new TreatmentDvh(treatmentPlanDvhPageList);
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath); 
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.treatmentPlan = treatmentPlan;
                comparator.treatmentDvh = treatmentDvh;
                comparator.tccPlan = tccPlan;
                _treatmentPlanLiveCatheters = comparator.treatmentPlanLiveCatheters();
                _treatmentPlanLiveCathetersCorr = comparator.treatmentPlanLiveCathetersDecayCorrected();
                _tccPlanLiveCatheters = comparator.tccPlanLiveCatheters();
                _resultRows.AddRange(comparator.allXpsResultRows());
            }

        }

        private void addCylinderResultRows()
        {
            Comparator comparator = new Comparator(_specifications);
            if ( _treatmentPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                _resultRows.AddRange(comparator.cylinderTreatmentPlanResultRows());
            }

            if (cylinderTypeDiamterLengthAndDoseIsSet() && _treatmentPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                DataForTreatmentTimeEstimate dataForTreatmentTimeEstimate = getDataForTreatmentTimeEstimate();
                _resultRows.AddRange(comparator.cylinderTreatmentPlanAndCylinderSettingsResultRows(dataForTreatmentTimeEstimate));
            }

            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                bool skipApprovalTest = true;
                bool useRelativeEpsilon = true;
                _resultRows.AddRange(comparator.resultRows(skipApprovalTest, useRelativeEpsilon));
                if (_sameSourceSet)
                {
                    _resultRows.AddRange(comparator.sourceComparisonResultRows(_isSameSource));
                }
                if (prescriptionDoseIsSetCylinder())
                {
                    _resultRows.AddRange(comparator.prescriptionDoseResultRowsCylinder());
                }

            }
        }

        bool buildResultDataGrid()
        {
            _resultRows.Clear();
            resultSummaryLabel.Visibility = Visibility.Hidden;
            bool correctFileType = false;
            if (_tabType == TabType.PROSTATE)
            {
                if (correctProstateFileType())
                {
                    correctFileType = true;
                    addProstateResultRows();
                }
            }
            else if (_tabType == TabType.CYLINDER)
            {
                if (correctCylinderFileType())
                {
                    correctFileType = true;
                    addCylinderResultRows();
                }
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
            updateResultSummaryLabel();
            return correctFileType;
        }

        private void updateCatheters()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                _treatmentPlanLiveCatheters = comparator.treatmentPlanLiveCatheters();
                catheterInfoButton.Visibility = Visibility.Visible;

                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                _tccPlanLiveCatheters = comparator.tccPlanLiveCatheters();
                catheterInfoButton.Visibility = Visibility.Visible;

                // Both plan and tcc file must be loaded to be able to calculate decay correction.
                _treatmentPlanLiveCathetersCorr = comparator.treatmentPlanLiveCathetersDecayCorrected();
            }
            else if (_treatmentPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                _treatmentPlanLiveCatheters = comparator.treatmentPlanLiveCatheters();
                catheterInfoButton.Visibility = Visibility.Visible;
            }
            else if (_tccPlanXpsFilePath != null)
            {
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                _tccPlanLiveCatheters = comparator.tccPlanLiveCatheters();
                catheterInfoButton.Visibility = Visibility.Visible;
            }

        }

        public DataTable treatmentPlanDataTable()
        {
            DataColumn testCase = new DataColumn("Kanal", typeof(string));
            DataColumn testResult = new DataColumn("Position", typeof(string));
            DataColumn resultDescripton = new DataColumn("Tid (s)", typeof(string));
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(testCase);
            dataTable.Columns.Add(testResult);
            dataTable.Columns.Add(resultDescripton);
            foreach (var item in _treatmentPlanLiveCatheters)
            {
                foreach (var subItem in item.positonTimePairs())
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = item.catheterNumber();
                    dataRow[1] = subItem.Item1;
                    dataRow[2] = subItem.Item2;
                    dataTable.Rows.Add(dataRow);
                }

            }
            return dataTable;
        }

        public DataTable treatmentPlanDataTableCorr()
        {
            DataColumn testCase = new DataColumn("Kanal", typeof(string));
            DataColumn testResult = new DataColumn("Position", typeof(string));
            DataColumn resultDescripton = new DataColumn("Tid (s)", typeof(string));
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(testCase);
            dataTable.Columns.Add(testResult);
            dataTable.Columns.Add(resultDescripton);
            foreach (var item in _treatmentPlanLiveCathetersCorr)
            {
                foreach (var subItem in item.positonTimePairs())
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = item.catheterNumber();
                    dataRow[1] = subItem.Item1;
                    dataRow[2] = subItem.Item2;
                    dataTable.Rows.Add(dataRow);
                }

            }
            return dataTable;
        }

        public DataTable tccPlanDataTable()
        {
            DataColumn testCase = new DataColumn("Kanal", typeof(string));
            DataColumn testResult = new DataColumn("Position", typeof(string));
            DataColumn resultDescripton = new DataColumn("Tid (s)", typeof(string));
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(testCase);
            dataTable.Columns.Add(testResult);
            dataTable.Columns.Add(resultDescripton);
            foreach (var item in _tccPlanLiveCatheters)
            {
                foreach (var subItem in item.positonTimePairs())
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = item.catheterNumber();
                    dataRow[1] = subItem.Item1;
                    dataRow[2] = subItem.Item2;
                    dataTable.Rows.Add(dataRow);
                }

            }
            return dataTable;
        }

        private void updateResultSummaryLabel()
        {
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
                resultSummaryLabel.Visibility = Visibility.Visible;
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
            openFileDialog.Filter = "xps files (*.xps)|*.xps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFile = openFileDialog.FileName;
                if (ProstateTab.IsSelected)
                {
                    TPXpsPathLabel0.Content = selectedFile;
                    _treatmentPlanXpsFilePathProstate = selectedFile;
                    _tabType = TabType.PROSTATE;
                }
                else if (CylinderTab.IsSelected)
                {
                    TPXpsPathLabel1.Content = selectedFile;
                    _treatmentPlanXpsFilePathCylinder = selectedFile;
                    _tabType = TabType.CYLINDER;
                }
                else if (IntraUterineTab.IsSelected)
                {
                    //TPXpsPathLabel2.Content = selectedFile;
                    //_treatmentPlanXpsFilePathIntraUterine = selectedFile;
                    //_tabType = TabType.INTRAUTERINE;
                }
            }
        }

        private void BtnOpenDVHFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xps files (*.xps)|*.xps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFile = openFileDialog.FileName;
                if (ProstateTab.IsSelected)
                {
                    DVHXpsPathLabel0.Content = selectedFile;
                    _dvhXpsFilePathProstate = selectedFile;
                }
                else if (IntraUterineTab.IsSelected)
                {
                    //DVHXpsPathLabel2.Content = selectedFile;
                    //_dvhXpsFilePathIntraUterine = selectedFile;
                }
            }
        }

        private void BtnOpenTCCFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xps files (*.xps)|*.xps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFile = openFileDialog.FileName;
                if (ProstateTab.IsSelected)
                {
                    TCCXpsPathLabel0.Content = selectedFile;
                    _tccPlanXpsFilePathProstate = selectedFile;
                }
                else if (CylinderTab.IsSelected)
                {
                    TCCXpsPathLabel1.Content = selectedFile;
                    _tccPlanXpsFilePathCylinder = selectedFile;
                }
                else if (IntraUterineTab.IsSelected)
                {
                    TCCXpsPathLabel2.Content = selectedFile;
                    _tccPlanXpsFilePathIntraUterine = selectedFile;
                }
            }
        }


        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            updateSpecifications();
            setProstateCalculationsVisable(false);
            calculateLengthAndFreeLength();
            setCylinderCalculationsVisable(false);
            updateInputFilePaths();
            updateSameSourceSelected();
            if (buildResultDataGrid())
            {
                updateCatheters();
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProstateTab.IsSelected)
            {
                _tabType = TabType.PROSTATE;
            }
            else if (CylinderTab.IsSelected)
            {
                _tabType = TabType.CYLINDER;
            }
            else if (IntraUterineTab.IsSelected)
            {
                _tabType = TabType.INTRAUTERINE;
            }

            setProstateCalculationsVisable(false);
            setCylinderCalculationsVisable(false);
            _resultRows.Clear();
            resultSummaryLabel.Content = "";
            resultSummaryLabel.Visibility = Visibility.Hidden;
            DataTable dataTable = new DataTable();
            ResultDataGrid.ItemsSource = dataTable.DefaultView;
            catheterInfoButton.Visibility = Visibility.Hidden;
        }

        private void cylinderTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            cylinderDiameterComboBox.Items.Clear();

            if (cylinderTypeComboBox.SelectedIndex == 0) //sel ind already updated
            {
                cylinderDiameterComboBox.Items.Add("20");
                _comboboxDiameters.Add(20);
                cylinderDiameterComboBox.Items.Add("25");
                _comboboxDiameters.Add(25);
                cylinderDiameterComboBox.Items.Add("30");
                _comboboxDiameters.Add(30);
                cylinderDiameterComboBox.Items.Add("35");
                _comboboxDiameters.Add(35);
                cylinderDiameterComboBox.Items.Add("40");
                _comboboxDiameters.Add(40);
            }
            if (cylinderTypeComboBox.SelectedIndex == 1)
            {
                cylinderDiameterComboBox.Items.Add("25");
                _comboboxDiameters.Add(25);
                cylinderDiameterComboBox.Items.Add("30");
                _comboboxDiameters.Add(30);
                cylinderDiameterComboBox.Items.Add("35");
                _comboboxDiameters.Add(35);
                cylinderDiameterComboBox.Items.Add("40");
                _comboboxDiameters.Add(40);
            }
        }
        private void catheterInfo_Click(object sender, RoutedEventArgs e)
        {
            CatheterInfoWindow catheterInfoWindow = new CatheterInfoWindow();
            catheterInfoWindow.setTreatmentPlanDataGrid(treatmentPlanDataTable());
            catheterInfoWindow.setTreatmentPlanDataGridCorr(treatmentPlanDataTableCorr());
            catheterInfoWindow.setTccPlanDataGrid(tccPlanDataTable());
            catheterInfoWindow.ShowDialog(); // use ShowDialog to make it modal. use show will make non modal
        }

    }
}

//Text exists in Glyphs -> UnicodeString string attribute. You have to use XMLReader for fixed page.
// https://stackoverflow.com/questions/12262197/extract-text-from-a-xps-document