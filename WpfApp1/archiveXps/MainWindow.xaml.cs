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

namespace archiveXps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string inputDirectory = ConfigurationManager.AppSettings["inputDirectory"];
            inputDirectoryLabel.Content = "Detta program arkiverar filer i katalogen:\n" + inputDirectory;
        }
        public void archiveXpsFiles()
        {
            PageReader treatmentPlanPageReader = new PageReader("");

        }


        private XpsFileInfo getXpsFileInfo(string filePath)
        {
            PageReader pageReader = new PageReader(filePath);
            XpsFileInfo xpsFileInfo = new XpsFileInfo();
            if (pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_TREATMENT_PLAN))
            {
                TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.PROSTATE);
                xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_prost_plan.xps";
                xpsFileInfo.OutputDirectoryName = "Prostata_xps_filer_arkiv";
            }
            else if (pageReader.isFileType(XpsFileType.ONCENTRA_PROSTATE_DVH))
            {
                TreatmentDvh treatmentDvh = new TreatmentDvh(pageReader.getPages());
                xpsFileInfo.PlanCode = treatmentDvh.planCode() + "_prost_dvh.xps";
                xpsFileInfo.OutputDirectoryName = "Prostata_xps_filer_arkiv";
            }
            else if (pageReader.isFileType(XpsFileType.PROSTATE_TCC))
            {
                List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
                TccPlan tccPlan = new TccPlan(pageReader.getPages(), liveCatheters);
                xpsFileInfo.PlanCode = tccPlan.planCode() + "_prost_tcc.xps";
                xpsFileInfo.OutputDirectoryName = "Prostata_xps_filer_arkiv";
            }
            else if (pageReader.isFileType(XpsFileType.ONCENTRA_CYLINDER_TREATMENT_PLAN))
            {
                TreatmentPlan treatmentPlan = new TreatmentPlan(pageReader.getPages(), TabType.CYLINDER);
                xpsFileInfo.PlanCode = treatmentPlan.planCode() + "_cyl_plan.xps";
                xpsFileInfo.OutputDirectoryName = "Cylinder_xps_filer_arkiv";
            }
            else if (pageReader.isFileType(XpsFileType.CYLINDER_TCC))
            {
                List<LiveCatheter> liveCatheters = new List<LiveCatheter>();
                TccPlan tccPlan = new TccPlan(pageReader.getPages(), liveCatheters);
                xpsFileInfo.PlanCode = tccPlan.planCode() + "_cyl_tcc.xps";
                xpsFileInfo.OutputDirectoryName = "Cylinder_xps_filer_arkiv";
            }
            else
            {
                xpsFileInfo.PlanCode = "UNKNOWN";
                xpsFileInfo.OutputDirectoryName = "UNKNOWN";
            }
            return xpsFileInfo;
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
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }
            File.Move(soureFileNamePath, destinationFilePath);
        }

        private void archiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Skall xps-filerna flyttas till arkiv-mappen?", "Fråga", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                string inputDirectory = ConfigurationManager.AppSettings["inputDirectory"];
                string[] filePaths = Directory.GetFiles(@inputDirectory, "*.xps");
                foreach (var filePath in filePaths)
                {
                    XpsFileInfo xpsFileInfo = getXpsFileInfo(filePath);
                    if (xpsFileInfo.PlanCode == "UNKNOWN")
                    {
                        continue;
                    }
                    moveFileToArchive(filePath, xpsFileInfo.OutputDirectoryName, xpsFileInfo.PlanCode);
                }
            }
            catch (Exception exception)
            {
                string messageStr = "Det gick inte att flytta xps-filer till arkiv-mappen. Fel: " + exception.ToString();
                MessageBox.Show(messageStr, "Fel inträffade", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Filerna har arkiverats", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
           
            System.Windows.Application.Current.Shutdown();
        }
    }
}
