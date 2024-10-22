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
using static SAA_EquipmentMonitor_VST100_Lib.SAA_DatabaseEnum;

namespace SAA_EquipmentMonitor_VST100.UI
{
    /// <summary>
    /// ucSaaEquipmentMonitorHome.xaml 的互動邏輯
    /// </summary>
    public partial class ucSaaEquipmentMonitorHome : UserControl
    {
        public delegate void DelDataHome(App.BtnName home);
        public event DelDataHome? OnDataHome;

        public ucSaaEquipmentMonitorHome()
        {
            InitializeComponent();
        }

        private void SaaLog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OnDataHome?.Invoke(App.BtnName.BtnLog);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}-{ex.StackTrace}", App.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaaSend_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OnDataHome?.Invoke(App.BtnName.BtnSend);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}-{ex.StackTrace}", App.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaaSetUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OnDataHome?.Invoke(App.BtnName.BtnSetUp);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}-{ex.StackTrace}", App.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
