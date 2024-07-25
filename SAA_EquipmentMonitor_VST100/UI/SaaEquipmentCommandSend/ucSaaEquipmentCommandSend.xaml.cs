using SAA_EquipmentMonitor_VST100_Lib;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace SAA_EquipmentMonitor_VST100.UI.SaaEquipmentCommandSend
{
    /// <summary>
    /// ucSaaEquipmentCommandSend.xaml 的互動邏輯
    /// </summary>
    public partial class ucSaaEquipmentCommandSend : UserControl
    {
        public ucSaaEquipmentCommandSend()
        {
            InitializeComponent();
        }

        private void CmdTake_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                string cmdstation = CmdStation.Text;
                if (cmdstation != string.Empty)
                {
                    CmdTake.Items.Clear();
                    var locationsettingdata = SAA_Database.SaaSql?.GetScLocationsetting(SAA_Database.configattributes.SaaSetNo, SAA_Database.configattributes.SaaModelName, cmdstation);
                    if (locationsettingdata != null)
                    {
                        foreach (DataRow dr in locationsettingdata!.Rows)
                        {
                            CmdTake.Items.Add(dr["LOCATIONID"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void CmdTake_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string cmdstation = CmdStation.Text;
                string cmdput = CmdTake.Text;
                if (cmdput != string.Empty)
                {
                    var dara = SAA_Database.SaaSql?.GetScLocationsettingLocationidStation(SAA_Database.configattributes.SaaSetNo.ToString(), cmdstation, cmdput);
                    if (dara?.Rows.Count != 0)
                    {
                        App.UpdateUi(() =>
                        {
                            TexLotId.Text = dara?.Rows[0]["CARRIERID"].ToString();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void CmdPut_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                CmdPut.Items.Clear();
                CmdPut.Items.Add("PGV-OUT");
                CmdPut.Items.Add("DK-IN");
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string cmdtake = CmdTake.Text;
                string cmdput = CmdPut.Text;
                string texlotid = TexLotId.Text;
                if (cmdtake != string.Empty && cmdput != string.Empty && texlotid != string.Empty)
                {
                    if (CmdPut.Text == "PGV-OUT")
                    {
                        var dara = SAA_Database.SaaSql?.GetScLocationsettingLocationid(SAA_Database.configattributes.SaaSetNo.ToString(), SAA_Database.configattributes.SaaModelName, cmdtake);
                        string partno = dara?.Rows.Count != 0 ? dara!.Rows[0]["PARTNO"].ToString()! : "NA";
                        SaaEquipmentCarrierInfo equipmentcarrierinfo = new SaaEquipmentCarrierInfo
                        {
                            SETNO = SAA_Database.configattributes.SaaSetNo,
                            MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                            STATIOM_NAME = CmdStation.Text,
                            CARRIERID = texlotid,
                            PARTNO = partno,
                            CARRIERTYOE = "Normal",
                            OPER = "NA",
                            ROTFLAG = "0",
                            FLIPFLAG = "0",
                            REJECT_CODE = "RS0001",
                            REJECT_MESSAGE = "SAA_reject",
                        };
                        SAA_Database.SaaSql?.SetScEquipmentCarrierInfo(equipmentcarrierinfo);


                        SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                        {
                            TASKDATETIME = SAA_Database.ReadTime(),
                            SETNO = SAA_Database.configattributes.SaaSetNo,
                            MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                            STATION_NAEM = CmdStation.Text,
                            CARRIERID = texlotid,
                            REPORE_DATATRACK = "405",
                            REPORE_DATAREMOTE = cmdtake,
                            REPORE_DATALOCAL = "PGV-OUT",
                        };

                        SaaEquipmentCarrierInfo EquipmentCarrierInfo = new SaaEquipmentCarrierInfo
                        {
                            STATIOM_NAME = EquipmentReport.STATION_NAEM,
                            CARRIERID = EquipmentReport.CARRIERID,
                            REJECT_CODE = "RS0001",
                            REJECT_MESSAGE = "SAA_reject",
                        };
                        SAA_Database.SaaSql?.UpdScEquipmentCarrierInfo(EquipmentCarrierInfo);

                        SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                    }

                    if (CmdPut.Text == "DK-IN")
                    {
                        SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                        {
                            TASKDATETIME = SAA_Database.ReadTime(),
                            SETNO = SAA_Database.configattributes.SaaSetNo,
                            MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                            STATION_NAEM = CmdStation.Text,
                            CARRIERID = texlotid,
                            REPORE_DATATRACK = "105",
                            REPORE_DATAREMOTE = cmdtake,
                            REPORE_DATALOCAL = "DK-IN",
                        };
                        SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                    }

                    SaaScCommandTask CommandTask = new SaaScCommandTask
                    {
                        TASKDATETIME = SAA_Database.ReadTime(),
                        SETNO = SAA_Database.configattributes.SaaSetNo,
                        MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                        STATION_NAME = CmdStation.Text,
                        CARRIERID = texlotid,
                        COMMANDID = $"{DateTime.Now:fff}",
                        LOCATIONPUT = cmdput,
                        LOCATIONTAKE = cmdtake,
                    };
                    SAA_Database.SaaSql?.SetScCommandTask(CommandTask);
                    MessageBox.Show("新增需求完成");
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void CmdStation_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                CmdStation.Items.Clear();
                var locationsettingdata = SAA_Database.SaaSql?.GetScLocationsetting();
                if (locationsettingdata != null)
                {
                    foreach (DataRow dr in locationsettingdata!.Rows)
                    {
                        CmdStation.Items.Add(dr["STATIOM_NAME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void BtnAddReject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cmdstationreject = CmdStationReject.Text;
                if (cmdstationreject != string.Empty)
                {
                    var carrieridnotnull = SAA_Database.SaaSql?.GetScLocationSettingCarrierIdNotNull(cmdstationreject);
                    foreach (DataRow dr in carrieridnotnull!.Rows)
                    {
                        var data = SAA_Database.SaaSql?.GetScLiftCarrierInfoReject(cmdstationreject, dr["CARRIERID"].ToString()!);
                        if (data?.Rows.Count == 0)
                        {
                            SaaScLiftCarrierInfoReject CarrierInfoReject = new SaaScLiftCarrierInfoReject
                            {
                                TASKDATETIME = SAA_Database.ReadTime(),
                                SETNO = SAA_Database.configattributes.SaaSetNo,
                                MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                                CARRIERID = dr["CARRIERID"].ToString()!,
                                STATION_NAME = cmdstationreject,
                            };
                            if (SAA_Database.SaaSql?.GetScCommandTask(CarrierInfoReject.STATION_NAME, CarrierInfoReject.CARRIERID).Rows.Count == 0)
                            {
                                SaaEquipmentCarrierInfo EquipmentCarrierInfo = new SaaEquipmentCarrierInfo
                                {
                                    STATIOM_NAME = cmdstationreject,
                                    CARRIERID = CarrierInfoReject.CARRIERID,
                                    REJECT_CODE = "RS0001",
                                    REJECT_MESSAGE = "SAA_reject",
                                };
                                SAA_Database.SaaSql?.UpdScEquipmentCarrierInfo(EquipmentCarrierInfo);
                                SAA_Database.SaaSql?.SetScLiftCarrierInfoReject(CarrierInfoReject);
                            }
                        }
                    }
                    MessageBox.Show("一鍵退盒完成");
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        private void CmdStationReject_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                CmdStationReject.Items.Clear();
                var locationsettingdata = SAA_Database.SaaSql?.GetScLocationsetting();
                if (locationsettingdata != null)
                {
                    foreach (DataRow dr in locationsettingdata!.Rows)
                    {
                        CmdStationReject.Items.Add(dr["STATIOM_NAME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
    }
}
