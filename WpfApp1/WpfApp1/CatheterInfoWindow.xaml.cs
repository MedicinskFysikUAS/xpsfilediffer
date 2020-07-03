using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for CatheterInfoWindow.xaml
    /// </summary>
    public partial class CatheterInfoWindow : Window
    {
        
        public CatheterInfoWindow()
        {
            InitializeComponent();
           
        }

        public void setTreatmentPlanDataGrid(System.Data.DataTable treatmentPlanDataTable)
        {
            treatmentPlanDataGrid.ItemsSource = treatmentPlanDataTable.DefaultView;
        }

        public void setTccPlanDataGrid(System.Data.DataTable tccPlanDataTable)
        {
            tccPlanDataGrid.ItemsSource = tccPlanDataTable.DefaultView;
        }

        private void CatheterInfoExit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
