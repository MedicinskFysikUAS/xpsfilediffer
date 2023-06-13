using System;
using System.Collections.Generic;
using System.IO;
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
using System.Configuration;

using WpfApp1;
using System.Diagnostics;
using System.Net;

namespace archiveXps
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public string networkPath = @"\\195.252.26.54\asfdoc";
        public string networkPath = @"\\10.194.4.157\BRACHY\xpsFiler_klinisk";
        //public string networkPath = @"\\10.194.4.157\BRACHY";
        NetworkCredential credentials = new NetworkCredential(@"hdr1\flexitron", "flexitron");
        //NetworkCredential credentials = new NetworkCredential(@"asfcon", "Asfcon018");
        public string myNetworkPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            string inputDirectory = ConfigurationManager.AppSettings["inputDirectory"];
            inputDirectoryLabel.Content = "Detta program arkiverar xsp-filer i katalogen:\n" + inputDirectory;
        }
        public void archiveXpsFiles()
        {
            PageReader treatmentPlanPageReader = new PageReader("");

        }
        

        private XpsFileInfo getXpsFileInfo(string filePath)
        {
            PageReader pageReader = new PageReader(filePath);
            XpsFileInfo xpsFileInfo = new XpsFileInfo();
            xpsFileInfo.OutputDirectoryName = "xps_filer_arkiv";
            try
            {
                if (pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_TREATMENT_PLAN))
                {
                    TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.PROSTATE);
                    xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_prost_plan.xps";
                }
                else if (pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_DVH))
                {
                    TreatmentDvh treatmentDvh = new TreatmentDvh(pageReader.getPages());
                    xpsFileInfo.PlanCode = treatmentDvh.planCode() + "_prost_dvh.xps";
                }
                else if (pageReader.isFileType(XpsFileType.ONCENTRA_CYLINDER_TREATMENT_PLAN))
                {
                    TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.CYLINDER);
                    xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_cyl_plan.xps";
                }
                else if (pageReader.isFileType(XpsFileType.ONCENTRA_INTRAUTERINE_TREATMENT_PLAN))
                {
                    TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.INTRAUTERINE);
                    xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_intraut_plan.xps";
                }
                else if (pageReader.isFileType(XpsFileType.ONCENTRA_ESOFAGUS_TREATMENT_PLAN))
                {
                    TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.ESOFAGUS);
                    xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_esofagus_plan.xps";
                }
                else if (pageReader.isFileType(XpsFileType.PROSTATE_TCC) || 
                    (pageReader.isFileType(XpsFileType.CYLINDER_TCC)) ||
                    pageReader.isFileType(XpsFileType.INTRAUTERINE_TCC) ||
                    pageReader.isFileType(XpsFileType.ESOFAGUS_TCC))
                {
                    List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
                    TccPlan tccPlan = new TccPlan(pageReader.getPages(), liveCatheters);
                    string fractionNumber = tccPlan.fractionNumber();
                    xpsFileInfo.PlanCode = tccPlan.planCode() + "_frakt_" + fractionNumber + "_tcc.xps";
                }
                else
                {
                    xpsFileInfo.PlanCode = "UNKNOWN";
                    xpsFileInfo.OutputDirectoryName = "UNKNOWN";
                }

            }
            catch (Exception)
            {
                xpsFileInfo.PlanCode = "UNKNOWN";
                xpsFileInfo.OutputDirectoryName = "UNKNOWN";
                MessageBox.Show("Ett fel uppstod som ArchiveXps inte kunde hantera.", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return xpsFileInfo;
        }

        private void moveFileToArchive(string soureFileNamePath, string destinationDirectory, string destinationFileName)
        {
            using (new ConnectToSharedFolder(networkPath, credentials))
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
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }
                File.Move(soureFileNamePath, destinationFilePath);
            }
        }

        private void archiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Skall xps-filerna flyttas till arkiv-mappen?", "Fråga", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            int counter = 0;
            int skipCounter = 0;
            try
            {
                // Code taken from https://www.c-sharpcorner.com/blogs/how-to-access-network-drive-using-c-sharp
                using (new ConnectToSharedFolder(networkPath, credentials))
                {
                    string inputDirectory = ConfigurationManager.AppSettings["inputDirectory"];
                    string[] filePaths = Directory.GetFiles(@inputDirectory, "*.xps");
                    foreach (var filePath in filePaths)
                    {
                        XpsFileInfo xpsFileInfo = getXpsFileInfo(filePath);
                        if (xpsFileInfo.PlanCode == "UNKNOWN")
                        {
                            ++skipCounter;
                            continue;
                        }
                        moveFileToArchive(filePath, xpsFileInfo.OutputDirectoryName, xpsFileInfo.PlanCode);
                        ++counter;
                    }
                }
            }
            catch (Exception exception)
            {
                string messageStr = "Det gick inte att flytta xps-filer till arkiv-mappen. Fel:\n\n" + exception.ToString();
                MessageBox.Show(messageStr, "Fel inträffade", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            string message = counter.ToString() + " filerna har arkiverats.\n";
            message += skipCounter.ToString() + " filer har skippats.";
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
           
            System.Windows.Application.Current.Shutdown();
        }
    }
}
