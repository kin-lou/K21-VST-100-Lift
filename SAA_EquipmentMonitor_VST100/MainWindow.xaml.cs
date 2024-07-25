using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using NLog;
using SAA_EquipmentMonitor_VST100.UI;
using SAA_EquipmentMonitor_VST100_Lib;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentLib;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;
using SAA_EquipmentMonitor_VST100_Lib.ReceivLiftAttributes;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SAA_EquipmentMonitor_VST100
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SAA_Config SaaConfig = new SAA_Config();
        private ucSaaEquipmentMonitorElectric _mSaaEquipmentMonitorElectric = new ucSaaEquipmentMonitorElectric();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SAA_EquipmentMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SaaConfig.SaaReadConfig();
                GdContent.Children.Add(_mSaaEquipmentMonitorElectric);
                if (SAA_Database.SaaSql == null)
                    SAA_Database.SaaSql = new SAA_EquipmentMonitor_VST100_Lib.MsSqlData.SqlData();
                if (SAA_Database.SaaLog == null)
                    SAA_Database.SaaLog = LogManager.Setup().LoadConfigurationFromFile(SAA_Database.configattributes.SaaNLogName).GetCurrentClassLogger();
                SAA_Database.ReadSaaOffset(SAA_Database.configattributes.SaaSetNo, SAA_Database.configattributes.SaaModelName);
                SaaConfig.ReadScCommon(SAA_Database.configattributes.SaaSetNo, SAA_Database.configattributes.SaaModelName);
                TexEquipment.Text = SAA_Database.configattributes.SaaModelName;
                //SAA_Database.webapiserver.StartApiService();
                //SAA_Database.LogMessage($"設備Web Api Server已啟動，Server IP位置:{SAA_Database.configattributes.WebApiServerIP}:7075");
                //SAA_Database.LogMessage("設備資料傳送系統準備開始");
                var devicedata = SAA_Database.SaaSql.GetScDevice(SAA_Database.configattributes.SaaSetNo, SAA_Database.configattributes.SaaModelName);
                SAA_EquipmentMonitor.Title = $"SAA_VST100 LIFT設備監控系統 版本{App.GetEdition()} 更新日期:{new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime:yyyy-MM-dd HH:mm:ss}";
                if (devicedata != null)
                {
                    if (devicedata.Rows.Count != 0)
                    {
                        foreach (DataRow dr in devicedata.Rows)
                        {
                            SaaEquipmentPlcAttributes SaaEquipmentPlc = new SaaEquipmentPlcAttributes
                            {
                                SaaSetNo = int.Parse(dr["SETNO"].ToString()!),
                                SaaModelName = dr["MODEL_NAME"].ToString()!,
                                EquipmentStation = dr["STATION_NAME"].ToString()!,
                                ActHostAddress = dr["LIFTIP"].ToString()!,
                                ActPort = int.Parse(dr["LIFTPORT"].ToString()!),
                                ConnTimeout = SAA_Database.configattributes.ConnTimeout,
                                SendTimeout = SAA_Database.configattributes.SendTimeout,
                                IsOctalXY = SAA_Database.configattributes.IsOctalXY,
                                RecvTimeout = SAA_Database.configattributes.RecvTimeout,
                            };
                            SaaEquipmentLib saaequipmentlib = new SaaEquipmentLib();
                            saaequipmentlib.SaaEquipmentMonitorStart(SaaEquipmentPlc);
                            SAA_Database.LogMessage($"【監控程式】【{SaaEquipmentPlc.EquipmentStation}】程式啟動開始連線");
                            SAA_Database.LogMessage($"【監控程式】【{SaaEquipmentPlc.EquipmentStation}】開始讀取相關點位");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        #region [===關閉程式===]
        private void SAA_EquipmentMonitor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否要關閉程式", "關閉程式", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    //SAA_Database.webapiserver.StopApiService();
                    //SAA_Database.LogMessage("【監控程式】Web Api Server關閉並關閉連線");
                    SAA_Database.LogMessage("【監控程式】程式關閉並關閉連線");
                    e.Cancel = false;
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}-{ex.StackTrace}", App.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region [===切換背景顏色===]
        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyTheme(theme => theme.SetBaseTheme(DarkModeToggleButton.IsChecked == true ? Theme.Dark : Theme.Light));
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }
        #endregion

        private void BtnLogIn_OnClick(object sender, RoutedEventArgs e)
        {
            //SaaLiftReceive saaLift = new SaaLiftReceive
            //{
            //    Message = "123",
            //};
            //Dictionary<string, object> dicstatusb = new Dictionary<string, object>
            //            {
            //                { "Message",  saaLift.Message},
            //            };
            //string commandcontent = JsonConvert.SerializeObject(dicstatusb);
            //string test= SAA_Database.SaaSendCommandSystems(commandcontent, "GetLiftMessage");
            //SAA_Database.LogMessage($"【測試】【{test}】");
        }
    }
}