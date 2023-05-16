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
using System.IO;

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

        string _treatmentPlanXpsFilePathIntraUterine;
        string _tccPlanXpsFilePathIntraUterine;


        string _treatmentPlanXpsFilePathEsofagus;
        string _tccPlanXpsFilePathEsofagus;
        string _treatmentPlanXpsFilePathEsofagusFractionX;
        string _tccPlanXpsFilePathEsofagusFractionX;

        private TabType _tabType;
        private List<int> _comboboxDiameters;
        private List<int> _comboboxApplicatorDiameters;
        private List<int> _comboboxApplicatorDiametersNr2;
        private bool _isSameSource;
        private bool _sameSourceSet;
        List<LiveCatheter> _treatmentPlanLiveCatheters = new List<LiveCatheter>();
        List<LiveCatheter> _treatmentPlanLiveCathetersCorr = new List<LiveCatheter>();
        List<LiveCatheter> _tccPlanLiveCatheters = new List<LiveCatheter>();
        Specifications _specifications;
        UserInputIntrauterine _userInputIntrauterine;
        UserInputEsofagus _userInputEsofagus;


        public MainWindow()
        {
            InitializeComponent();
            setProstateCalculationsVisable(false);
            setCylinderCalculationsVisable(false);
            catheterInfoButton.Visibility = Visibility.Hidden;
            resultSummaryLabel.Content = "";
            _specifications = new Specifications();
            initiateCylinderTypeComboBox();
            initiateApplicatorTypeComboBox();
            _comboboxDiameters = new List<int>();
            _comboboxApplicatorDiameters = new List<int>();
            _comboboxApplicatorDiametersNr2 = new List<int>();
            initiateSameSourceCombobox();
            initiateEsofagusTab();
            // debug
            //PageReader tccPlanPageReader = new PageReader("C:\\work\\git\\xpsfilediffer\\xpsFiles\\A92DTcc.xps");
            //List<List<string>> tccPlanPageList = tccPlanPageReader.getPages(true);
            //List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(TabType.PROSTATE);
            //// end
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

        public void initiateApplicatorTypeComboBox()
        {
            applicatorTypeComboBox.Items.Add("Ringapplikator");
            applicatorTypeComboBox.Items.Add("Venezia utan matris");
            applicatorTypeComboBox.Items.Add("Venezia med matris");
            applicatorTypeComboBox.Items.Add("MCVC");
            applicatorTypeComboBox.Items.Add("Vmix utan matris");
            applicatorTypeComboBox.Items.Add("Vmix med matris");
            applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Hidden;
        }

        public void initiateSameSourceCombobox()
        {
            sameSourceCombobox0.Items.Add("Ja");
            sameSourceCombobox0.Items.Add("Nej");
            sameSourceCombobox1.Items.Add("Ja");
            sameSourceCombobox1.Items.Add("Nej");
            sameSourceCombobox2.Items.Add("Ja");
            sameSourceCombobox2.Items.Add("Nej");
            sameSourceCombobox3.Items.Add("Ja");
            sameSourceCombobox3.Items.Add("Nej");
        }

        public void initiateEsofagusTab()
        {
            BtnOpenTPFile3.IsEnabled = true;
            BtnOpenTCCFile3.IsEnabled = true;
            BtnOpenTPFile4.IsEnabled = false;
            BtnOpenTCCFile4.IsEnabled = false;
            firstFractionCombobox.Items.Add("Ja");
            firstFractionCombobox.Items.Add("Nej");
            EsofagusBtnCheck.IsEnabled = false;
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
                if (planCodeText.Text.Length > 0)
                {
                    _specifications.PlanCode = planCodeText.Text;
                }
            }
            else if (_tabType == TabType.CYLINDER)
            {
                _specifications.ExpectedChannelLength = 1300.0m;
                if (prescriptionDoseIsSetCylinder())
                {
                    _specifications.PrescriptionDoseCylinder = stringExtractor.decimalStringToDecimal(cylindricPrescribedDoseText.Text);
                }
                if (planCodeTextCylinder.Text.Length > 0)
                {
                    _specifications.PlanCodeCylinder = planCodeTextCylinder.Text;
                }
            }
            else if (_tabType == TabType.INTRAUTERINE)
            {
                _specifications.ExpectedChannelLength = -1.0m;
                if (prescriptionDoseIsSetIntrauterine())
                {
                    _specifications.PrescriptionDoseIntrauterine = stringExtractor.decimalStringToDecimal(intrauterinePrescribedDoseText.Text);
                }
                if (planCodeTextIntrauterine.Text.Length > 0)
                {
                    _specifications.PlanCodeIntrauterine = planCodeTextIntrauterine.Text;
                }

                IntrauterineApplicatorType intrauterineApplicatorType = IntrauterineApplicatorType.UNKNOWN;
                if (applicatorTypeComboBox.SelectedIndex == 0)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.RINGAPPLIKATOR;
                }
                else if (applicatorTypeComboBox.SelectedIndex == 1)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.VENEZIA;
                }
                else if (applicatorTypeComboBox.SelectedIndex == 2)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.VENEZIA_M_MATRIS;
                }
                else if (applicatorTypeComboBox.SelectedIndex == 3)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.MCVC;
                }
                else if (applicatorTypeComboBox.SelectedIndex == 4)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.VMIX;
                }
                else if (applicatorTypeComboBox.SelectedIndex == 5)
                {
                    intrauterineApplicatorType = IntrauterineApplicatorType.VMIX_M_MATRIS;
                }
                _specifications.IntrauterineApplicatorType = intrauterineApplicatorType;

                int applicatorDiameter = -1;
                if (applicatorDiameterComboBox.SelectedIndex != -1 &&
                    applicatorDiameterComboBox.SelectedIndex < _comboboxApplicatorDiameters.Count)
                {
                    applicatorDiameter = _comboboxApplicatorDiameters[applicatorDiameterComboBox.SelectedIndex];
                }
                _specifications.ApplicatorDiameter = applicatorDiameter;
                int applicatorDiameterNr2 = -1;
                if (applicatorDiameterComboBoxNr2.SelectedIndex != -1 &&
                    applicatorDiameterComboBoxNr2.SelectedIndex < _comboboxApplicatorDiametersNr2.Count)
                {
                    applicatorDiameterNr2 = _comboboxApplicatorDiametersNr2[applicatorDiameterComboBoxNr2.SelectedIndex];
                }
                _specifications.ApplicatorDiameterNr2 = applicatorDiameterNr2;
            }
            else if (_tabType == TabType.ESOFAGUS)
            {
                _specifications.LengthOfCathetersUsedForEsofagus = 1000.0m;
                _specifications.MaxChannelLengthEsofagus = 14000.0m;
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

        public bool planCodeIsSet()
        {
            return (planCodeText.Text.Length > 0);
        }

        public bool planCodeIsSetCylinder()
        {
            return (planCodeTextCylinder.Text.Length > 0);
        }

        public bool planCodeIsSetIntrauterine()
        {
            return (planCodeTextIntrauterine.Text.Length > 0);
        }

        public bool prescriptionDoseIsSetCylinder()
        {
            return (cylindricPrescribedDoseText.Text.Length > 0);
        }

        public bool prescriptionDoseIsSetIntrauterine()
        {
            return (intrauterinePrescribedDoseText.Text.Length > 0);
        }

        public bool cylinderTypeDiamterLengthAndDoseIsSet()
        {
            bool typIsSet = cylinderTypeComboBox.SelectedIndex != -1;
            bool diameterIsSet = cylinderDiameterComboBox.SelectedIndex != -1;
            return (typIsSet && diameterIsSet && treatmentLengthText.Text.Length > 0 && cylindricPrescribedDoseText.Text.Length > 0);
        }

        public bool intrauterineTypeDiamterAndDoseIsSet()
        {
            bool typIsSet = applicatorTypeComboBox.SelectedIndex != -1;
            bool diameterIsSet = applicatorDiameterComboBox.SelectedIndex != -1;
            return (typIsSet && diameterIsSet && intrauterinePrescribedDoseText.Text.Length > 0);
        }

        public bool intrauterineTypeDiamterNr2AndDoseIsSet()
        {
            bool typIsSet = applicatorTypeComboBox.SelectedIndex != -1;
            bool diameterIsSet = applicatorDiameterComboBoxNr2.SelectedIndex != -1;
            return (typIsSet && diameterIsSet && intrauterinePrescribedDoseText.Text.Length > 0);
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
                _treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathIntraUterine;
                _dvhXpsFilePath = "";
                _tccPlanXpsFilePath = _tccPlanXpsFilePathIntraUterine;
            }
            else if (EsofagusTab.IsSelected)
            {
                _treatmentPlanXpsFilePath = _treatmentPlanXpsFilePathEsofagus;
                _dvhXpsFilePath = "";
                _tccPlanXpsFilePath = _tccPlanXpsFilePathEsofagus;
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

        private bool sameSourceIntrauterine()
        {
            return sameSourceCombobox2.SelectedIndex == 0;
        }

        private bool sameSourceSetIntrauterine()
        {
            return sameSourceCombobox2.SelectedIndex != -1;
        }

        private bool applicatorTypeIsSet()
        {
            return applicatorTypeComboBox.SelectedIndex != -1;
        }

        private bool applicatorDiameterIsSet()
        {
            return applicatorDiameterComboBox.SelectedIndex != -1;
        }

        private bool applicatorDiameterNr2IsSet()
        {
            return applicatorDiameterComboBoxNr2.SelectedIndex != -1;
        }

        private bool fractionDoseIsSet()
        {
            return intrauterinePrescribedDoseText.Text.Length > 0;
        }

        private bool uterinePlanCodeIsSet()
        {
            return planCodeTextIntrauterine.Text.Length > 0;
        }

        private bool sameSourceIsSet()
        {
            return sameSourceCombobox2.SelectedIndex != -1;
        }

        // TODO: Rename this to something that describes that it also updates user input 
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
                _isSameSource = sameSourceIntrauterine();
                _sameSourceSet = sameSourceSetIntrauterine();
                _userInputIntrauterine = new UserInputIntrauterine();
                _userInputIntrauterine.ApplicatorTypeIsSet = applicatorTypeIsSet();
                _userInputIntrauterine.ApplicatorDiameterIsSet = applicatorDiameterIsSet();
                _userInputIntrauterine.ApplicatorDiameterNr2IsSet = applicatorDiameterNr2IsSet();
                _userInputIntrauterine.FractionDoseIsSet = fractionDoseIsSet();
                _userInputIntrauterine.PlanCodeIsSet = planCodeIsSetIntrauterine();
                _userInputIntrauterine.SameSourceIsSet = sameSourceIsSet();
            }
        }

        private void updateEsofagusUserInput()
        {
            _userInputEsofagus = new UserInputEsofagus();
            _userInputEsofagus.ActiveLengthString = activeLengthText.Text;
            _userInputEsofagus.InactiveLengthString = inactiveLengthText.Text;
            _userInputEsofagus.IndexerLengthString = indexLengthText.Text;
            _userInputEsofagus.PlanCode = planCodeEsofagusText.Text;
            _userInputEsofagus.IsSameSource = sameSourceCombobox3.SelectedIndex == 0;
            _userInputEsofagus.IsFirstFraction = firstFractionCombobox.SelectedIndex == 0;
            _userInputEsofagus.PrescribedDoseString = esofagusPrescribedDoseText.Text;
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

        private bool correctIntrauterineFileType()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_treatmentPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ONCENTRA_INTRAUTERINE_TREATMENT_PLAN))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig Oncentra Brachy fil."));
                    return false;
                }
            }

            if (_tccPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_tccPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.INTRAUTERINE_TCC))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig TCC fil."));
                    return false;
                }
            }
            return true;
        }

        private bool correctEsofagusFileType()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_treatmentPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ONCENTRA_ESOFAGUS_TREATMENT_PLAN))
                {
                    _resultRows.AddRange(comparator.informationResultRows("", "Inte OK", "Felaktig Oncentra Brachy fil."));
                    return false;
                }
            }

            if (_tccPlanXpsFilePath != null)
            {
                PageReader pageReader = new PageReader(_tccPlanXpsFilePath);
                if (!pageReader.isFileType(XpsFileType.ESOFAGUS_TCC))
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
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven ordinationsdos", "Ordinationsdosen är inte angiven."));
                }
                if (planCodeIsSet())
                {
                    _resultRows.AddRange(comparator.planCodeResultRows());
                }
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven plankod", "Plankoden är inte angiven."));
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
            if (_treatmentPlanXpsFilePath != null)
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
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven ordinationsdos", "Ordinationsdosen är inte angiven."));
                }
                if (planCodeIsSetCylinder())
                {
                    _resultRows.AddRange(comparator.planCodeResultRowsCylinder());
                }
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven plankod", "Plankoden är inte angiven."));
                }
            }
        }

        private void addIntrauterineResultRows()
        {
            Comparator comparator = new Comparator(_specifications);
            _resultRows.AddRange(comparator.intrauterineInfoRows(_userInputIntrauterine));
            IntrauterineApplicatorType intrauterineApplicatorType = selectedItrauterineApplicatorType();
            if (_treatmentPlanXpsFilePath != null && applicatorTypeIsSet())
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType, intrauterineApplicatorType); 
                comparator.treatmentPlan = treatmentPlan;
                _resultRows.AddRange(comparator.intrauterineTreatmentPlanResultRows());
            }

            if ((intrauterineTypeDiamterAndDoseIsSet() && _treatmentPlanXpsFilePath != null) ||
                    (intrauterineTypeDiamterNr2AndDoseIsSet() && _treatmentPlanXpsFilePath != null))
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType, intrauterineApplicatorType);
                comparator.treatmentPlan = treatmentPlan;
                // DataForTreatmentTimeEstimate dataForTreatmentTimeEstimate = getDataForTreatmentTimeEstimate();
                // TBD: I think we could skip this test as we use SunCheck
                //_resultRows.AddRange(comparator.cylinderTreatmentPlanAndCylinderSettingsResultRows(dataForTreatmentTimeEstimate));
                _resultRows.AddRange(comparator.intrauterineTreatmentPlanAndIntrauterineSettingsResultRows());
            }

            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType, intrauterineApplicatorType);
                comparator.treatmentPlan = treatmentPlan;
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                List<LiveCatheter> tccLiveCatheters = tccPlanPageReader.tccLiveCatheters(_tabType);
                TccPlan tccPlan = new TccPlan(tccPlanPageList, tccLiveCatheters);
                comparator.tccPlan = tccPlan;
                bool skipApprovalTest = true;
                bool useRelativeEpsilon = false;
                bool useTimeEpsilonVeneziaAndRings = false;
                if (intrauterineApplicatorType  == IntrauterineApplicatorType.VENEZIA ||
                    intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS ||
                    intrauterineApplicatorType == IntrauterineApplicatorType.RINGAPPLIKATOR)
                {
                    useTimeEpsilonVeneziaAndRings = true;
                }
                _resultRows.AddRange(comparator.intrauterineTreatmentPlanAndTccPlanResultRows());
                _resultRows.AddRange(comparator.resultRows(skipApprovalTest, useRelativeEpsilon, useTimeEpsilonVeneziaAndRings));
                if (_sameSourceSet)
                {
                    _resultRows.AddRange(comparator.sourceComparisonResultRows(_isSameSource));
                }
                if (prescriptionDoseIsSetIntrauterine())
                {
                    _resultRows.AddRange(comparator.prescriptionDoseResultRowsIntrauterine());
                }
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven ordinationsdos", "Ordinationsdosen är inte angiven."));
                }
                if (planCodeIsSetIntrauterine())
                {
                    _resultRows.AddRange(comparator.planCodeResultRowsIntrauterine());
                }
                else
                {
                    _resultRows.AddRange(comparator.errorResultRows("Angiven plankod", "Plankoden är inte angiven."));
                }
            }
        }

        public void addEsofagusResultRows()
        {
            Comparator comparator = new Comparator(_specifications);
            if (_treatmentPlanXpsFilePathEsofagus != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePathEsofagus);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);
                comparator.treatmentPlan = treatmentPlan;
                _resultRows.AddRange(comparator.esofagusInfoRows(_userInputEsofagus));
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
                _resultRows.AddRange(comparator.resultRows(skipApprovalTest));
                _resultRows.AddRange(comparator.esofagusTreatmentLengthResultRows(_userInputEsofagus));
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
            else if (_tabType == TabType.INTRAUTERINE)
            {
                if (correctIntrauterineFileType())
                {
                    correctFileType = true;
                    addIntrauterineResultRows();
                }
            }
            else if (_tabType == TabType.ESOFAGUS)
            {
                if (correctEsofagusFileType())
                {
                    correctFileType = true;
                    addEsofagusResultRows();
                }
            }

            DataColumn testCase = new DataColumn("Test", typeof(string));
            DataColumn testResult = new DataColumn("Result", typeof(string));
            DataColumn resultDescripton = new DataColumn("Beskrivning", typeof(string));
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
            IntrauterineApplicatorType intrauterineApplicatorType = selectedItrauterineApplicatorType();
            if (_treatmentPlanXpsFilePath != null && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType, intrauterineApplicatorType);
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
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType, intrauterineApplicatorType);
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

        bool addVarning(decimal timeEpsilon, string planTime, string tccTime)
        {
            StringExtractor stringExtractor = new StringExtractor();
            if (planTime != "" && tccTime == "" && Math.Abs(stringExtractor.decimalStringToDecimal(planTime)) > timeEpsilon)
            {
                return true;
            }

            if (planTime == "" || tccTime == "")
            { 
                return false;
            }
            else if ((Math.Abs(stringExtractor.decimalStringToDecimal(planTime) -
            stringExtractor.decimalStringToDecimal(tccTime)) > timeEpsilon))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool useTimeEpsilonVenezia()
        {
            bool useTimeEpsilonVenezia = false;
            if (_tabType == TabType.INTRAUTERINE)
            {
                IntrauterineApplicatorType intrauterineApplicatorType = selectedItrauterineApplicatorType();
                if (intrauterineApplicatorType == IntrauterineApplicatorType.RINGAPPLIKATOR ||
                    intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA ||
                    intrauterineApplicatorType == IntrauterineApplicatorType.VENEZIA_M_MATRIS)
                {
                    useTimeEpsilonVenezia = true;
                }

            }
            return useTimeEpsilonVenezia;
        }

        public DataTable treatmentAndTccPlanDataTable()
        {
            DataColumn testCase = new DataColumn("Kanal", typeof(string));
            DataColumn testResult = new DataColumn("Position", typeof(string));
            DataColumn resultDescriptonPlan = new DataColumn("Tid plan (s)", typeof(string));
            DataColumn resultDescriptonTcc = new DataColumn("Tid TCC (s)", typeof(string));
            DataColumn note = new DataColumn("_", typeof(string));
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(testCase);
            dataTable.Columns.Add(testResult);
            dataTable.Columns.Add(resultDescriptonPlan);
            dataTable.Columns.Add(resultDescriptonTcc);
            dataTable.Columns.Add(note);
            int counter = 0 ;
            Specifications specifications = new Specifications();
            List<LiveCatheter> tccPlanLiveCatheters = _tccPlanLiveCatheters;
            foreach (var item in _treatmentPlanLiveCathetersCorr)
            {
                StringExtractor stringExtractor = new StringExtractor();
                foreach (var subItem in item.positonTimePairs())
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = item.catheterNumber();
                    dataRow[1] = subItem.Item1;
                    dataRow[2] = subItem.Item2;
                    dataRow[3] = "";
                    dataRow[4] = "";
                    if (tccPlanLiveCatheters.Count > counter)
                    {
                        string tccTime = "";
                        for (int i = 0; i < tccPlanLiveCatheters[counter].positonTimePairs().Count; i++)
                        {
                            if ((stringExtractor.decimalStringToDecimal(tccPlanLiveCatheters[counter].positonTimePairs()[i].Item1) -
                                stringExtractor.decimalStringToDecimal(subItem.Item1)) == 0.0m)
                            {
                                tccTime = tccPlanLiveCatheters[counter].positonTimePairs()[i].Item2;
                                break;
                            }
                        }
                        dataRow[3] = tccTime;
                    }
                    else
                    {
                        dataRow[3] = "";
                    }
                    decimal timeEpsilon = useTimeEpsilonVenezia() ? specifications.TimeEpsilonVenezia : specifications.TimeEpsilon;
                    if (addVarning(timeEpsilon, dataRow[2].ToString(), dataRow[3].ToString()))
                    {
                        dataRow[4] = "?";
                    }
                    dataTable.Rows.Add(dataRow);
                }
                if (item.positonTimePairs().Count > 0)
                {
                    ++counter;
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
                    TPXpsPathLabel2.Content = selectedFile;
                    _treatmentPlanXpsFilePathIntraUterine = selectedFile;
                    _tabType = TabType.INTRAUTERINE;
                }
                else if (EsofagusTab.IsSelected)
                {
                    TPXpsPathLabel3.Content = selectedFile;
                    _treatmentPlanXpsFilePathEsofagus = selectedFile;
                    _tabType = TabType.ESOFAGUS;
                }
            }
        }

        private void BtnOpenTPFile2_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xps files (*.xps)|*.xps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFile = openFileDialog.FileName;
                if (EsofagusTab.IsSelected)
                {
                    TPXpsPathLabel4.Content = selectedFile;
                    _treatmentPlanXpsFilePathEsofagus = selectedFile;
                    _tabType = TabType.ESOFAGUS;
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
                else if (EsofagusTab.IsSelected)
                {
                    TCCXpsPathLabel3.Content = selectedFile;
                    _tccPlanXpsFilePathEsofagus = selectedFile;
                }
            }
        }

        private void BtnOpenTCCFile2_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xps files (*.xps)|*.xps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFile = openFileDialog.FileName;
                if (EsofagusTab.IsSelected)
                {
                    TCCXpsPathLabel4.Content = selectedFile;
                    _treatmentPlanXpsFilePathEsofagusFractionX = selectedFile;
                    _tabType = TabType.ESOFAGUS;
                }
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                updateSpecifications();
                setProstateCalculationsVisable(false);
                calculateLengthAndFreeLength();
                setCylinderCalculationsVisable(false);
                // TODO:
                //setIntrauterineCalculationsVisable(false)
                updateInputFilePaths();
                updateSameSourceSelected();
                if (_tabType == TabType.ESOFAGUS)
                {
                    updateEsofagusUserInput();
                }
                if (buildResultDataGrid())
                {
                    updateCatheters();
                }
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Ett fel uppstod som programmet inte kunde hantera.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void moveFileToArchive(string soureFileNamePath, string destinationDirectory, string destinationFileName)
        {
            string sourceDirectory = System.IO.Path.GetDirectoryName(soureFileNamePath);
            if (!Directory.Exists(sourceDirectory))
            {
                throw new Exception("'sourceDirectory' does not exist: " + sourceDirectory);
            }

            string archveDirectory = System.IO.Path.Combine(sourceDirectory, destinationDirectory);
            if (!Directory.Exists(archveDirectory))
            {
                System.IO.Directory.CreateDirectory(archveDirectory);
            }

            if (System.IO.Path.GetFileName(destinationFileName) != destinationFileName)
            {
                throw new Exception("'destinationFileName' is invalid: " + destinationFileName);
            }
            string destinationFilePath = System.IO.Path.Combine(archveDirectory, destinationFileName);


            string err; int result;
            //// Try to disconnect from network
            //result = NetworkHelper.Disconnect(@"\\195.252.26.54", true, out err);
            //if (result != 0)
            //{
            //    Console.WriteLine(err);
            //    return;
            //}

            // Try to connect to network
            result = NetworkHelper.Connect(@"\\195.252.26.54", @"asfcon", @"Asfcon018", false, out err);
            if (result != 0)
            {
                Console.WriteLine(err);
                return;
            }

            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }
            File.Move(soureFileNamePath, destinationFilePath);
        }


        private List<List<string>> getPageList(string xpsFilePath)
        {
            PageReader pageReader = new PageReader(xpsFilePath);
            return pageReader.getPages();
        }

        public string getTreatmentPlanCode(string xpsFilePath, TabType tabType)
        {  
            TreatmentPlan treatmentPlan = new TreatmentPlan(getPageList(xpsFilePath), tabType);
            return treatmentPlan.planCode();
        }
        public string getTccPlanCode(string xpsFilePath)
        {
            List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
            TccPlan tccPlan = new TccPlan(getPageList(xpsFilePath), liveCatheters);
            return tccPlan.planCode();
        }
        private void moveFilesToArchive()
        {
            try
            {
                if (_treatmentPlanXpsFilePath != null && File.Exists(_treatmentPlanXpsFilePath) &&
                    _tccPlanXpsFilePath != null && File.Exists(_tccPlanXpsFilePath) &&
                    ((_dvhXpsFilePath != null && File.Exists(_dvhXpsFilePath) && ProstateTab.IsSelected) ||
                        CylinderTab.IsSelected))
                {
                    if (MessageBox.Show("Skall xps-filerna flyttas till arkiv-mappen?", "Fråga", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }

                    PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                    List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                    TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList, _tabType);

                    if (ProstateTab.IsSelected)
                    {
                        string planCode = getTreatmentPlanCode(_treatmentPlanXpsFilePath, TabType.PROSTATE);
                        string archiveDirName = "Prostata_xps_filer_arkiv";
                        moveFileToArchive(_treatmentPlanXpsFilePath,archiveDirName, planCode + "_prost_plan.xps");
                        moveFileToArchive(_dvhXpsFilePath,archiveDirName, planCode + "_prost_dvh.xps");
                        planCode = getTccPlanCode(_treatmentPlanXpsFilePath);
                        moveFileToArchive(_tccPlanXpsFilePath,archiveDirName, planCode + "_prost_tcc.xps");
                    }
                    else if (CylinderTab.IsSelected)
                    {
                        string planCode = getTreatmentPlanCode(_treatmentPlanXpsFilePath, TabType.CYLINDER);
                        string archiveDirName = "Cylinder_xps_filer_arkiv";
                        moveFileToArchive(_treatmentPlanXpsFilePath, archiveDirName, planCode + "_cyl_plan.xps");
                        planCode = getTccPlanCode(_treatmentPlanXpsFilePath);
                        moveFileToArchive(_tccPlanXpsFilePath, archiveDirName, planCode + "_cyl_tcc.xps");
                    }
                }
            }
            catch (Exception e)
            {
                string messageStr = "Det gick inte att flytta xps-filer till arkiv-mappen. Fel: " + e.ToString();
                MessageBox.Show(messageStr, "Fel inträffade", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void archiveFileReminder()
        {
            if (_treatmentPlanXpsFilePath != null && File.Exists(_treatmentPlanXpsFilePath) &&
                    _tccPlanXpsFilePath != null && File.Exists(_tccPlanXpsFilePath) &&
                    ((_dvhXpsFilePath != null && File.Exists(_dvhXpsFilePath) && ProstateTab.IsSelected) ||
                        CylinderTab.IsSelected))
            {
                MessageBox.Show("Glöm inte att arkivera xps-filerna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            archiveFileReminder();
            System.Windows.Application.Current.Shutdown();
        }

        private void clearLabelsAndResultRows()
        {
            setProstateCalculationsVisable(false);
            setCylinderCalculationsVisable(false);
            _resultRows.Clear();
            resultSummaryLabel.Content = "";
            resultSummaryLabel.Visibility = Visibility.Hidden;
            DataTable dataTable = new DataTable();
            ResultDataGrid.ItemsSource = dataTable.DefaultView;
            catheterInfoButton.Visibility = Visibility.Hidden;
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
            else if (EsofagusTab.IsSelected)
            {
                _tabType = TabType.ESOFAGUS;
            }

            clearLabelsAndResultRows();
        }

        private void cylinderTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            cylinderDiameterComboBox.Items.Clear();
            _comboboxDiameters.Clear();

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

        private void firstFractionCombobox_DropDownClosed(object sender, EventArgs e)
        {
            if (firstFractionCombobox.SelectedIndex == 0)
            {
                BtnOpenTPFile4.IsEnabled = false;
                BtnOpenTCCFile4.IsEnabled = false;
            }
            else if (firstFractionCombobox.SelectedIndex == 1)
            {
                BtnOpenTPFile4.IsEnabled = true;
                BtnOpenTCCFile4.IsEnabled = true;
            }
            EsofagusBtnCheck.IsEnabled = true;
        }

        private void applicatorTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            applicatorDiameterComboBox.Items.Clear();
            applicatorDiameterComboBoxNr2.Items.Clear();
            _comboboxApplicatorDiameters.Clear();
            // "Ringapplikator 0
            // "Venezia utan matris" 1
            // "Venezia med matris" 2
            // "MCVC" 3
            // "VMIX utan" 
            // "VMIX med" 
            if (applicatorTypeComboBox.SelectedIndex == 0)
            {
                applicatorDiameterComboBox.Items.Add("26");
                _comboboxApplicatorDiameters.Add(26);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);
                applicatorDiameterComboBox.Items.Add("34");
                _comboboxApplicatorDiameters.Add(34);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Hidden;
            }
            if (applicatorTypeComboBox.SelectedIndex == 1)
            {
                applicatorDiameterComboBox.Items.Add("22");
                _comboboxApplicatorDiameters.Add(22);
                applicatorDiameterComboBox.Items.Add("26");
                _comboboxApplicatorDiameters.Add(26);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Hidden;
            }
            if (applicatorTypeComboBox.SelectedIndex == 2)
            {
                applicatorDiameterComboBox.Items.Add("22");
                _comboboxApplicatorDiameters.Add(22);
                applicatorDiameterComboBox.Items.Add("26");
                _comboboxApplicatorDiameters.Add(26);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Hidden;
            }
            if (applicatorTypeComboBox.SelectedIndex == 3)
            {
                applicatorDiameterComboBox.Items.Add("25");
                _comboboxApplicatorDiameters.Add(25);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);
                applicatorDiameterComboBox.Items.Add("35");
                _comboboxApplicatorDiameters.Add(35);
                cylinderDiameterComboBox.Items.Add("40");
                _comboboxApplicatorDiameters.Add(40);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Hidden;
            }
            if (applicatorTypeComboBox.SelectedIndex == 4)
            {
                applicatorDiameterComboBox.Items.Add("22");
                _comboboxApplicatorDiameters.Add(22);
                applicatorDiameterComboBox.Items.Add("26");
                _comboboxApplicatorDiameters.Add(26);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);

                applicatorDiameterComboBoxNr2.Items.Add("22");
                _comboboxApplicatorDiametersNr2.Add(22);
                applicatorDiameterComboBoxNr2.Items.Add("26");
                _comboboxApplicatorDiametersNr2.Add(26);
                applicatorDiameterComboBoxNr2.Items.Add("30");
                _comboboxApplicatorDiametersNr2.Add(30);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Visible;
            }
            if (applicatorTypeComboBox.SelectedIndex == 5)
            {
                applicatorDiameterComboBox.Items.Add("22");
                _comboboxApplicatorDiameters.Add(22);
                applicatorDiameterComboBox.Items.Add("26");
                _comboboxApplicatorDiameters.Add(26);
                applicatorDiameterComboBox.Items.Add("30");
                _comboboxApplicatorDiameters.Add(30);

                applicatorDiameterComboBoxNr2.Items.Add("22");
                _comboboxApplicatorDiametersNr2.Add(22);
                applicatorDiameterComboBoxNr2.Items.Add("26");
                _comboboxApplicatorDiametersNr2.Add(26);
                applicatorDiameterComboBoxNr2.Items.Add("30");
                _comboboxApplicatorDiametersNr2.Add(30);
                applicatorDiameterComboBoxNr2.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private IntrauterineApplicatorType selectedItrauterineApplicatorType()
        {
            if (applicatorTypeComboBox.SelectedIndex == 0)
            {
                return IntrauterineApplicatorType.RINGAPPLIKATOR;
            }
            else if (applicatorTypeComboBox.SelectedIndex == 1)
            {
                return IntrauterineApplicatorType.VENEZIA;
            }
            else if (applicatorTypeComboBox.SelectedIndex == 2)
            {
                return IntrauterineApplicatorType.VENEZIA_M_MATRIS;
            }
            else if (applicatorTypeComboBox.SelectedIndex == 3)
            {
                return IntrauterineApplicatorType.MCVC;
            }
            else if (applicatorTypeComboBox.SelectedIndex == 4)
            {
                return IntrauterineApplicatorType.VMIX;
            }
            else if (applicatorTypeComboBox.SelectedIndex == 5)
            {
                return IntrauterineApplicatorType.VMIX_M_MATRIS;
            }
            else 
            {
                return IntrauterineApplicatorType.UNKNOWN;
            }
        }

        private void catheterInfo_Click(object sender, RoutedEventArgs e)
        {
            CatheterInfoWindow catheterInfoWindow = new CatheterInfoWindow();
            catheterInfoWindow.setTreatmentPlanDataGrid(treatmentPlanDataTable());
            catheterInfoWindow.setTreatmentPlanDataGridCorr(treatmentAndTccPlanDataTable());
            catheterInfoWindow.setTccPlanDataGrid(tccPlanDataTable());
            catheterInfoWindow.ShowDialog(); // use ShowDialog to make it modal. use show will make non modal
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            clearLabelsAndResultRows();
        }
    }
}

//Text exists in Glyphs -> UnicodeString string attribute. You have to use XMLReader for fixed page.
// https://stackoverflow.com/questions/12262197/extract-text-from-a-xps-document