using SAA_EquipmentMonitor_VST100.UI.SaaEquipmentCommandSend;
using SAA_EquipmentMonitor_VST100.UI.SaaEquipmentLog;
using SAA_EquipmentMonitor_VST100_Lib;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
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

namespace SAA_EquipmentMonitor_VST100.UI
{
    /// <summary>
    /// ucSaaEquipmentMonitorElectric.xaml 的互動邏輯
    /// </summary>
    public partial class ucSaaEquipmentMonitorElectric : UserControl
    {
        private UserControl control = new UserControl();
        private readonly ucSaaEquipmentMonitorHome _mSaaEquipmentMonitorHome = new ucSaaEquipmentMonitorHome();
        private readonly ucSaaEquipmentMonitorLog _mSaaEquipmentMonitorLog = new ucSaaEquipmentMonitorLog();
        private readonly ucSaaEquipmentCommandSend _mSaaEquipmentCommandSend = new ucSaaEquipmentCommandSend();

        public ucSaaEquipmentMonitorElectric()
        {
            InitializeComponent();

            foreach (UIElement child in SplButton.Children)
            {
                child.IsEnabled = true;
            }
            BtnMenu.IsEnabled = false;
            GetGrid(_mSaaEquipmentMonitorHome);
        }
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                UIInfo(button.Name);
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void UIInfo(string uiname)
        {
            try
            {
                foreach (UIElement child in SplButton.Children)
                {
                    child.IsEnabled = true;
                }

                switch ((App.BtnName)Enum.Parse(typeof(App.BtnName), uiname))
                {
                    case App.BtnName.BtnMenu:
                        GetGrid(_mSaaEquipmentMonitorHome);
                        BtnMenu.IsEnabled = false;
                        break;
                    case App.BtnName.BtnLog:
                        GetGrid(_mSaaEquipmentMonitorLog);
                        BtnLog.IsEnabled = false;
                        break;
                    case App.BtnName.BtnSend:
                        GetGrid(_mSaaEquipmentCommandSend);
                        BtnSend.IsEnabled = false;
                        break;
                    case App.BtnName.BtnSetUp:
                        BtnSetUp.IsEnabled = false;
                        //SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                        //{
                        //    TASKDATETIME = SAA_Database.ReadTime(),
                        //    SETNO = 3,
                        //    MODEL_NAME = "K21",
                        //    STATION_NAEM = "SAA",
                        //    CARRIERID = "SAA1",
                        //    REPORE_DATATRACK = "1",
                        //    REPORE_DATAREMOTE = "TEST1",
                        //    REPORE_DATALOCAL = "TEST1",
                        //};
                        //SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                        break;

                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
                MessageBox.Show($"{ex.Message}-{ex.StackTrace}", App.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetGrid(UserControl user)
        {
            GdContent.Children.Clear();
            control.Visibility = Visibility.Hidden;
            user.Visibility = Visibility.Visible;
            GdContent.Children.Add(user);
        }
    }
}
