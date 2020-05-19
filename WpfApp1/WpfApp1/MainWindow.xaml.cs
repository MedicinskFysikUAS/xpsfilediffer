﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
           
        }

        void buildResultDataGrid()
        {
            DataTable dataTable = new DataTable();
            string tmp = needleLengthText.Text;
            string tmp2 = probeDistanceText.Text;

            if (_treatmentPlanXpsFilePath != null /*&& _dvhXpsFilePath != "Inte vald"*/ && _tccPlanXpsFilePath != null)
            {
                PageReader treatmentPlanPageReader = new PageReader(_treatmentPlanXpsFilePath);
                List<List<string>> treatmentPlanPageList = treatmentPlanPageReader.getPages();
                TreatmentPlan treatmentPlan = new TreatmentPlan(treatmentPlanPageList);
                //PageReader dvhPageReader = new PageReader(_dvhXpsFilePath);
                //List<List<string>> dvhPageList = dvhPageReader.getPages();
                //dvh dvh = new dvh(treatmentPlanPageList);
                PageReader tccPlanPageReader = new PageReader(_tccPlanXpsFilePath);
                List<List<string>> tccPlanPageList = tccPlanPageReader.getPages();
                TccPlan tccPlan = new TccPlan(tccPlanPageList);
                Comparator comparator = new Comparator(treatmentPlan, tccPlan);
                _resultRows = comparator.resultRows();
            }
            DataColumn testCase= new DataColumn("Test", typeof(string));
            DataColumn testResult = new DataColumn("Result", typeof(string));
            DataColumn resultDescripton = new DataColumn("Beskriving", typeof(string));
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
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //this.buildResultDataGrid();
        //}



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