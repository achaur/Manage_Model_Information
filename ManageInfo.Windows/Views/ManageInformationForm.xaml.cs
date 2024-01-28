using System;

namespace ManageInfo_Windows
{
    public partial class ManageInformationForm : BaseView
    {
        public ManageInformationForm()
        {
            InitializeMaterialDesign();
            InitializeComponent();
        }

        private void HideCalculationColumns(object sender, System.Windows.RoutedEventArgs e)
        {
            CalculationColumn1.Visibility = System.Windows.Visibility.Collapsed;
            CalculationColumn2.Visibility = System.Windows.Visibility.Collapsed;
            CalculationColumn3.Visibility = System.Windows.Visibility.Collapsed;
            CalculationColumn4.Visibility = System.Windows.Visibility.Collapsed;
            CalculationColumn5.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UnHideCalculationColumns(object sender, System.Windows.RoutedEventArgs e)
        {
            CalculationColumn1.Visibility = System.Windows.Visibility.Visible;
            CalculationColumn2.Visibility = System.Windows.Visibility.Visible;
            CalculationColumn3.Visibility = System.Windows.Visibility.Visible;
            CalculationColumn4.Visibility = System.Windows.Visibility.Visible;
            CalculationColumn5.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
