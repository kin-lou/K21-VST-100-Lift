using SAA_EquipmentMonitor_VST100_Lib;
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

namespace SAA_EquipmentMonitor_VST100.UI.SaaEquipmentLog
{
    /// <summary>
    /// ucSaaEquipmentMonitorLog.xaml 的互動邏輯
    /// </summary>
    public partial class ucSaaEquipmentMonitorLog : UserControl
    {
        public ucSaaEquipmentMonitorLog()
        {
            InitializeComponent();

            SAA_Database.OnLogMessage += SAA_Database_OnLogMessage;
        }

        private void SAA_Database_OnLogMessage(string message, SAA_DatabaseEnum.LogType logtype)
        {
            App.UpdateUi(() =>
            {
                if (logtype == SAA_DatabaseEnum.LogType.Error)
                {
                    LogMessage.Children.Insert(0, new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.Red,
                        Style = (Style)Application.Current.TryFindResource("SelectRecord")
                    });
                }
                else if (logtype == SAA_DatabaseEnum.LogType.Warnning)
                {
                    LogMessage.Children.Insert(0, new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.Pink,
                        Style = (Style)Application.Current.TryFindResource("SelectRecord")
                    });
                }
                else
                {
                    LogMessage.Children.Insert(0, new TextBlock
                    {
                        Text = message,
                        Style = (Style)Application.Current.TryFindResource("SelectRecord")
                    });
                }

                if (LogMessage.Children.Count > App.DisplayLogLineCount)
                    LogMessage.Children.RemoveAt(LogMessage.Children.Count - 1);
            });
        }
    }
}
