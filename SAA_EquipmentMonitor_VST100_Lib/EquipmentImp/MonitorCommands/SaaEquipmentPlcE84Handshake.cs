using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SAA.Core.PLC.Models;
using SAA.Core.PLC.Slmp.Functions;
using SAA_EquipmentMonitor_VST100_Lib.Attributes;
using SAA_EquipmentMonitor_VST100_Lib.Controllers;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcImp;
using SAA_EquipmentMonitor_VST100_Lib.ReceivLiftAttributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentImp.MonitorCommands
{
    public class SaaEquipmentPlcE84Handshake
    {
        private int heartbit = 0;
        private int serialnumber;
        private bool PlcE84 = true;
        private bool PlcE84Confirm = false;
        private bool Reply = false;
        private int GroupCount;
        private bool MonitorStart = true;
        private int[] SaaDataTrack = new int[5];
        private int[] SaaDaataReply = new int[5];
        private int[] SaaDataLocal = new int[5];
        private int[] SaaDataRemote = new int[5];
        private int[] SaaDataCarrierId = new int[5];
        private bool SaaAlarm = true;
        public int SaaBit = 0;
        public int SaaBitAck = 0;
        //private int[] PlcE84Request = new int[16];
        public int DeviceStatus = 0;
        public int SaaDeviceStatus = 0;
        private EquipmentPlc? SaaEquipmentPlc;
        private SaaEquipmentAttributes SaaEquipmentgroup = new SaaEquipmentAttributes();
        private SaaEquipmentE84Plc EquipmentE84Plc = new SaaEquipmentE84Plc();
        private SaaEquipmentOffsetAttributes EquipmentOffset = new SaaEquipmentOffsetAttributes();
        private int plceqpoffset = 0;
        private string EquipmentStation = string.Empty;
        private string PlcE84InCarrierId = string.Empty;

        public SaaEquipmentPlcE84Handshake(string equipmentstation, EquipmentPlc saaequipmentplc)
        {
            SaaEquipmentPlc = saaequipmentplc;
            EquipmentStation = equipmentstation;

            WebApiController.OnLiftE84PcData += WebApiController_OnLiftE84PcData;
        }

        private void WebApiController_OnLiftE84PcData(SaaScLiftE84Pc LiftE84Pc)
        {
            if (EquipmentStation == LiftE84Pc.STATION_NAME)
            {
                SAA_Database.LogMessage($"E84站點【{LiftE84Pc.STATION_NAME}】開始寫入PLC E84訊號");
                if (PlcE84InCarrierId != LiftE84Pc.CARRIERID)
                {
                    SaaEquipmentPlc?.WriteStringArray("W", "310", LiftE84Pc.CARRIERID);
                    SAA_Database.LogMessage($"【{LiftE84Pc.STATION_NAME}】【E84監控】【W310】寫入卡匣ID==>CARRIERID:{LiftE84Pc.CARRIERID}");
                    PlcE84InCarrierId = LiftE84Pc.CARRIERID;
                }
                int[] e84pc = { LiftE84Pc.HOA_VBL, LiftE84Pc.ES, LiftE84Pc.L_REQ, LiftE84Pc.U_REQ, LiftE84Pc.READY, 0, LiftE84Pc.VA, LiftE84Pc.VS_0, LiftE84Pc.VS_1 };
                SaaEquipmentPlc?.WriteIntArray("B", "300", e84pc);
                SAA_Database.LogMessage($"【{LiftE84Pc.STATION_NAME}】【E84監控】【B300】寫入PLC資料:HO_AVBL:{e84pc[0]}，ES:{e84pc[1]}，L_REQ:{e84pc[2]}，U_REQ:{e84pc[3]}，Ready:{e84pc[4]}，VA:{e84pc[6]}，VS_0:{e84pc[7]}，VS_1:{e84pc[8]}");
                SAA_Database.LogMessage($"【{LiftE84Pc.STATION_NAME}】【E84監控】結束寫入PLC E84訊號");
            }
        }

        #region [===更新時間心跳包===]
        public async void SaaEquipmentUpdDateTimeLifebitClock(SaaEquipmentPlcOffsetAttributes SaaEquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        SaaUpdDateTimeLifebitClock(SaaEquipmentPlcOffset);
                        Thread.Sleep(1000);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void SaaUpdDateTimeLifebitClock(SaaEquipmentPlcOffsetAttributes SaaEquipmentPlcOffset)
        {
            try
            {
                DateTime datetime = DateTime.Now;
                int[] date = new int[6];
                date[0] = short.Parse(datetime.Year.ToString());
                date[1] = short.Parse(datetime.Month.ToString());
                date[2] = short.Parse(datetime.Day.ToString());
                date[3] = short.Parse(datetime.Hour.ToString());
                date[4] = short.Parse(datetime.Minute.ToString());
                date[5] = short.Parse(datetime.Second.ToString());
                SaaEquipmentPlc?.WriteIntArray(SAA_Database.saaequipmentoffset.SaaDB_DateTime, SAA_Database.saaequipmentoffset.SaaValue_DateTime, date);
                heartbit = heartbit == 0 ? 1 : 0;
                SaaEquipmentPlc?.WriteInt(SAA_Database.saaequipmentoffset.SaaDB_HeartBit, SAA_Database.saaequipmentoffset.SaaValue_HeartBit, heartbit);
                SaaDeviceStatus = SaaEquipmentPlc!.ReadInt(SAA_Database.saaequipmentbitoffset.SaaDB_DeviceStatus, SAA_Database.saaequipmentbitoffset.SaaValue_DeviceStatus);
                if (SaaDeviceStatus != DeviceStatus)
                {
                    SaaScDevice ScDevice = new SaaScDevice
                    {
                        SETNO = int.Parse(SaaEquipmentPlcOffset.SETNO),
                        MODEL_NAME = SaaEquipmentPlcOffset.MODEL_NAME,
                        STATION_NAME = SaaEquipmentPlcOffset.STATION_NAME,
                        DEVICESTATUS = SaaDeviceStatus == 1 || SaaDeviceStatus == 5 ? SAA_DatabaseEnum.DeviceStatus.Y.ToString() : SAA_DatabaseEnum.DeviceStatus.N.ToString(),
                    };
                    SAA_Database.SaaSql?.UpdScDeviceModeStatus(ScDevice);
                    SAA_Database.LogMessage($"【{ScDevice.STATION_NAME}】【設備狀態變更】設備狀態為:{SaaDeviceStatus}，狀態更新為:{ScDevice.DEVICESTATUS}(Y:自動，N:手動)");
                    SaaLiftReceive saaLift = new SaaLiftReceive
                    {
                        Statiom_Name = ScDevice.STATION_NAME,
                        CommandName = SAA_DatabaseEnum.SaaLiftCommandName.EquipmentStatus.ToString(),
                    };
                    Dictionary<string, object> dicstatusb = new Dictionary<string, object>
                           {
                               { SAA_DatabaseEnum.EquipmentStatusCommand.Statiom_Name.ToString(),  saaLift.Statiom_Name},
                               { SAA_DatabaseEnum.EquipmentStatusCommand.CommandName.ToString(), saaLift.CommandName}
                           };
                    string commandcontent = JsonConvert.SerializeObject(saaLift);
                    string ReportMessage = SAA_Database.SaaSendCommandSystems(commandcontent, SAA_DatabaseEnum.SendWebApiCommandName.GetLiftMessage.ToString());
                    SAA_Database.LogMessage($"【{saaLift.Statiom_Name}】【傳送設備】【EQ-LCS至轉譯程式】接收結果:{ReportMessage}");
                    DeviceStatus = SaaDeviceStatus;
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        #region [===讀取PLC E84交握===]
        public async void SaaEquipmentMonitorE84PlcSendStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        EquipmentOffset.PlcE84Request = SaaEquipmentPlc?.ReadIntArray(EquipmentPlcOffset.E84PLC_DB, EquipmentPlcOffset.E84PLC_VALUE, 864)!;
                        if (EquipmentOffset.PlcE84Request[1] == 1)
                        {
                            if (PlcE84)
                            {
                                var liftdata = SAA_Database.SaaSql?.GetScLiftE84PcStatsus(EquipmentPlcOffset.STATION_NAME)!;
                                if (liftdata != null)
                                {
                                    if (liftdata.Rows.Count != 0)
                                    {
                                        SaaEquipmentE84PlcAttributes SaaEquipmentE84Plc = new SaaEquipmentE84PlcAttributes
                                        {
                                            TASKDATETIME = SAA_Database.ReadTime(),
                                            STATION_NAME = liftdata.Rows[0]["STATION_NAME"].ToString()!,
                                            SHUTTLEID = liftdata.Rows[0]["SHUTTLEID"].ToString()!,
                                            COMMANDID = liftdata.Rows[0]["COMMANDID"].ToString()!,
                                            CARRIERID = liftdata.Rows[0]["CARRIERID"].ToString()!,
                                            AM_AVBL = EquipmentOffset.PlcE84Request[768],
                                            CS_0 = EquipmentOffset.PlcE84Request[769],
                                            Valid = EquipmentOffset.PlcE84Request[770],
                                            TR_REQ = EquipmentOffset.PlcE84Request[771],
                                            Busy = EquipmentOffset.PlcE84Request[772],
                                            Complete = EquipmentOffset.PlcE84Request[773],
                                            Continue = EquipmentOffset.PlcE84Request[774],
                                            SELECT = EquipmentOffset.PlcE84Request[775],
                                        };
                                        SAA_Database.SaaSql?.SetScLifte84Plc(SaaEquipmentE84Plc);
                                        SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【程式初始化】【E84監控】新增資料庫資料上報讀取PLC資訊寫入資料庫完成。");
                                    }
                                }
                                EquipmentOffset.PcE84Request = EquipmentOffset.PlcE84Request!;
                                PlcE84 = false;
                            }

                            for (int i = 768; i < 776; i++)
                            {
                                if (EquipmentOffset.PcE84Request?[i] != EquipmentOffset.PlcE84Request?[i])
                                {
                                    if (EquipmentOffset.PlcE84Request != null)
                                    {
                                        serialnumber = i;
                                        switch (i)
                                        {
                                            case 768:
                                                EquipmentE84Plc.AM_AVBL = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【AM_AVBL】【設備可以使用】AM_AVBL:{EquipmentE84Plc.AM_AVBL}");
                                                break;
                                            case 769:
                                                EquipmentE84Plc.CS_0 = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【CS_0】【指定傳輸Port1】CS_0:{EquipmentE84Plc.CS_0}");
                                                break;
                                            case 770:
                                                EquipmentE84Plc.Valid = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Valid】【是否可交握】Valid-:{EquipmentE84Plc.Valid}");
                                                break;
                                            case 771:
                                                EquipmentE84Plc.TR_REQ = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【TR_REQ】【傳輸請求】TR_REQ:{EquipmentE84Plc.TR_REQ}");
                                                break;
                                            case 772:
                                                EquipmentE84Plc.Busy = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Busy】【正在進行傳輸】Busy:{EquipmentE84Plc.Busy}");
                                                break;
                                            case 773:
                                                EquipmentE84Plc.Complete = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Complete】【完成傳輸】Complete:{EquipmentE84Plc.Complete}");
                                                break;
                                            case 774:
                                                EquipmentE84Plc.Continue = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Continue】【繼續傳輸】Continue:{EquipmentE84Plc.Continue}");
                                                break;
                                            case 775:
                                                EquipmentE84Plc.SELECT = EquipmentOffset.PlcE84Request[i];
                                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【SELECT】【選擇】SELECT:{EquipmentE84Plc.SELECT}");
                                                break;
                                            default:
                                                break;
                                        }
                                        switch (i)
                                        {
                                            case 768:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【AM_AVBL】【設備可以使用】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 769:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【CS_0】【指定傳輸Port1】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 770:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Valid】【是否可交握】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 771:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【TR_REQ】【傳輸請求】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 772:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Busy】【正在進行傳輸】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 773:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Complete】【完成傳輸】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 774:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Continue】【繼續傳輸】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            case 775:
                                                SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【SELECT】【選擇】PC紀錄值:{EquipmentOffset.PcE84Request?[i]}，PLC紀錄值:{EquipmentOffset.PlcE84Request?[i]}");
                                                break;
                                            default:
                                                break;
                                        }
                                        PlcE84Confirm = true;
                                    }
                                }
                            }

                            if (PlcE84Confirm)
                            {
                                while (true)
                                {
                                    var liftdata = SAA_Database.SaaSql?.GetScLiftE84PcStatsus(EquipmentPlcOffset.STATION_NAME)!;
                                    if (liftdata != null)
                                    {
                                        if (liftdata.Rows.Count != 0)
                                        {
                                            SaaEquipmentE84PlcAttributes SaaEquipmentE84Plc = new SaaEquipmentE84PlcAttributes
                                            {
                                                TASKDATETIME = SAA_Database.ReadTime(),
                                                STATION_NAME = liftdata.Rows[0]["STATION_NAME"].ToString()!,
                                                SHUTTLEID = liftdata.Rows[0]["SHUTTLEID"].ToString()!,
                                                COMMANDID = liftdata.Rows[0]["COMMANDID"].ToString()!,
                                                CARRIERID = liftdata.Rows[0]["CARRIERID"].ToString()!,
                                                CS_0 = EquipmentE84Plc.CS_0,
                                                Valid = EquipmentE84Plc.Valid,
                                                TR_REQ = EquipmentE84Plc.TR_REQ,
                                                Busy = EquipmentE84Plc.Busy,
                                                Complete = EquipmentE84Plc.Complete,
                                                Continue = EquipmentE84Plc.Continue,
                                                SELECT = EquipmentE84Plc.SELECT,
                                                AM_AVBL = EquipmentE84Plc.AM_AVBL,
                                            };
                                            SaaScLiftE84iLisPlc LiftE84iLisPlc = new SaaScLiftE84iLisPlc
                                            {
                                                TASKDATETIME = SaaEquipmentE84Plc.TASKDATETIME,
                                                STATION_NAME = SaaEquipmentE84Plc.STATION_NAME,
                                                SHUTTLEID = SaaEquipmentE84Plc.SHUTTLEID,
                                                COMMANDID = SaaEquipmentE84Plc.COMMANDID,
                                                CARRIERID = SaaEquipmentE84Plc.CARRIERID,
                                                CS_0 = EquipmentE84Plc.CS_0,
                                                VALID = EquipmentE84Plc.Valid,
                                                TR_REQ = EquipmentE84Plc.TR_REQ,
                                                BUSY = EquipmentE84Plc.Busy,
                                                COMPT = EquipmentE84Plc.Complete,
                                                CONT = EquipmentE84Plc.Continue,
                                                SELECT = EquipmentE84Plc.SELECT,
                                                AM_AVBL = EquipmentE84Plc.AM_AVBL,
                                                CS_1 = 0,
                                                READY = 0,
                                                ES = 0,
                                                HOA_VBL = 0,
                                                L_REQ = 0,
                                                U_REQ = 0,
                                                VA = 0,
                                                VS_0 = 0,
                                                VS_1 = 0,
                                            };
                                            Dictionary<string, object> dicstatusb = new Dictionary<string, object>
                                       {
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.TASKDATETIME.ToString(), LiftE84iLisPlc.TASKDATETIME},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.STATION_NAME.ToString(), LiftE84iLisPlc.STATION_NAME},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.SHUTTLEID.ToString(), LiftE84iLisPlc.SHUTTLEID},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.CARRIERID.ToString(), LiftE84iLisPlc.CARRIERID},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.COMMANDID.ToString(), LiftE84iLisPlc.COMMANDID},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.Mode.ToString(), LiftE84iLisPlc.Mode},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.VALID.ToString(), LiftE84iLisPlc.VALID},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.CS_0.ToString(), LiftE84iLisPlc.CS_0},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.CS_1.ToString(), LiftE84iLisPlc.CS_1},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.TR_REQ.ToString(), LiftE84iLisPlc.TR_REQ},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.L_REQ.ToString(), LiftE84iLisPlc.L_REQ},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.U_REQ.ToString(), LiftE84iLisPlc.U_REQ},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.READY.ToString(), LiftE84iLisPlc.READY},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.BUSY.ToString(), LiftE84iLisPlc.BUSY},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.COMPT.ToString(), LiftE84iLisPlc.COMPT},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.CONT.ToString(), LiftE84iLisPlc.CONT},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.HOA_VBL.ToString(), LiftE84iLisPlc.HOA_VBL},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.ES.ToString(), LiftE84iLisPlc.ES},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.VA.ToString(), LiftE84iLisPlc.VA},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.AM_AVBL.ToString(), LiftE84iLisPlc.AM_AVBL},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.VS_0.ToString(), LiftE84iLisPlc.VS_0},
                                           { SAA_DatabaseEnum.SaaScLiftE84iLisPlc.VS_1.ToString(), LiftE84iLisPlc.VS_1},
                                       };
                                            string commandcontent = JsonConvert.SerializeObject(dicstatusb);
                                            SAA_Database.LogMessage($"【{LiftE84iLisPlc.STATION_NAME}】【{SAA_DatabaseEnum.SendWebApiCommandName.SaaEquipmentMonitorE84PlcSendStart}】【傳送E84訊號】LIFT傳送E84訊號至轉譯程式，內容:{commandcontent}");
                                            string result = SAA_Database.SaaSendCommandSystems(commandcontent, SAA_DatabaseEnum.SendWebApiCommandName.SaaEquipmentMonitorE84PlcSendStart.ToString());
                                            SAA_Database.LogMessage($"【{LiftE84iLisPlc.STATION_NAME}】【{SAA_DatabaseEnum.SendWebApiCommandName.SaaEquipmentMonitorE84PlcSendStart}】轉譯程式回傳結果:{result}");

                                            //SAA_Database.SaaSql?.SetScLifte84Plc(SaaEquipmentE84Plc);
                                            //SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】新增資料庫資料上報讀取PLC資訊寫入資料庫完成。");
                                            EquipmentOffset.PcE84Request = EquipmentOffset.PlcE84Request;
                                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】E84資訊PC已變更。");
                                            SAA_Database.SaaSql?.UpdScLifte84PlcStatus(SaaEquipmentE84Plc);
                                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】E84資訊PLC狀態已變更。");

                                            PlcE84Confirm = false;
                                            #region [===暫時不用===]
                                            //for (int i = 0; i < EquipmentOffset.PcE84Request?.Length; i++)
                                            //{
                                            //    serialnumber = i + 1;
                                            //    switch (i)
                                            //    {
                                            //        case 768:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【AM_AVBL】【設備可以使用】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 769:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【CS_0】【指定傳輸Port1】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 770:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Valid】【是否可交握】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 771:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【TR_REQ】【傳輸請求】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 772:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Busy】【正在進行傳輸】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 773:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Complete】【完成傳輸】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 774:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【Continue】【繼續傳輸】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        case 775:
                                            //            SAA_Database.LogMessage($"【測試監控】【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【第{serialnumber}個】【SELECT】【選擇】變更PC紀錄值:{EquipmentOffset.PcE84Request?[i]}");
                                            //            break;
                                            //        default:
                                            //            break;
                                            //    }
                                            //} 
                                            #endregion
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】查詢資料庫為Null無法新增資料。", SAA_DatabaseEnum.LogType.Error);
                                    }
                                }
                                Thread.Sleep(100);
                            }
                        }
                        else
                        {
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B401】讀取PLC為{EquipmentOffset.PlcE84Request[1]}非1");
                        }
                        Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        #region [===寫入PLC E84交握===]
        public async void SaaEquipmentMonitorE84PcSendStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        var liftdata = SAA_Database.SaaSql?.GetScLifte84PC(EquipmentPlcOffset.STATION_NAME)!;
                        if (liftdata != null)
                        {
                            if (liftdata.Rows.Count != 0)
                            {
                                SaaScLiftE84Pc ScLiftE84Pc = new SaaScLiftE84Pc
                                {
                                    TASKDATETIME = liftdata.Rows[0]["TASKDATETIME"].ToString()!,
                                    STATION_NAME = liftdata.Rows[0]["STATION_NAME"].ToString()!,
                                    SHUTTLEID = liftdata.Rows[0]["SHUTTLEID"].ToString()!,
                                    COMMANDID = liftdata.Rows[0]["COMMANDID"].ToString()!,
                                    CARRIERID = liftdata.Rows[0]["CARRIERID"].ToString()!,
                                    Mode = liftdata.Rows[0]["Mode"].ToString()!,
                                    VALID = liftdata.Rows[0]["VALID"].ToString() == "True" ? 1 : 0,
                                    CS_0 = liftdata.Rows[0]["CS_0"].ToString() == "True" ? 1 : 0,
                                    CS_1 = liftdata.Rows[0]["CS_1"].ToString() == "True" ? 1 : 0,
                                    TR_REQ = liftdata.Rows[0]["TR_REQ"].ToString() == "True" ? 1 : 0,
                                    L_REQ = liftdata.Rows[0]["L_REQ"].ToString() == "True" ? 1 : 0,
                                    U_REQ = liftdata.Rows[0]["U_REQ"].ToString() == "True" ? 1 : 0,
                                    READY = liftdata.Rows[0]["READY"].ToString() == "True" ? 1 : 0,
                                    BUSY = liftdata.Rows[0]["BUSY"].ToString() == "True" ? 1 : 0,
                                    COMPT = liftdata.Rows[0]["COMPT"].ToString() == "True" ? 1 : 0!,
                                    CONT = liftdata.Rows[0]["CONT"].ToString() == "True" ? 1 : 0,
                                    HOA_VBL = liftdata.Rows[0]["HO_AVBL"].ToString() == "True" ? 1 : 0,
                                    ES = liftdata.Rows[0]["ES"].ToString() == "True" ? 1 : 0,
                                    VA = liftdata.Rows[0]["VA"].ToString() == "True" ? 1 : 0,
                                    AM_AVBL = liftdata.Rows[0]["AM_AVBL"].ToString() == "True" ? 1 : 0,
                                    VS_0 = liftdata.Rows[0]["VS_0"].ToString() == "True" ? 1 : 0,
                                    VS_1 = liftdata.Rows[0]["VS_1"].ToString() == "True" ? 1 : 0,
                                    RESULT = liftdata.Rows[0]["RESULT"].ToString()!,
                                };
                                SaaEquipmentPlc?.WriteStringArray(EquipmentPlcOffset.E84PC_CARRRIERID_DB, EquipmentPlcOffset.E84PC_CARRRIERID_VALUE, ScLiftE84Pc.CARRIERID);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【{EquipmentPlcOffset.E84PC_CARRRIERID_DB}{EquipmentPlcOffset.E84PC_CARRRIERID_VALUE}】寫入卡匣ID==>CARRIERID:{ScLiftE84Pc.CARRIERID}");
                                int[] e84pc = { ScLiftE84Pc.HOA_VBL, ScLiftE84Pc.ES, ScLiftE84Pc.L_REQ, ScLiftE84Pc.U_REQ, ScLiftE84Pc.READY, 0, ScLiftE84Pc.VA, ScLiftE84Pc.VS_0, ScLiftE84Pc.VS_1 };
                                SaaEquipmentPlc?.WriteIntArray(EquipmentPlcOffset.E84PC_DB, EquipmentPlcOffset.E84PC_VALUE, e84pc);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【E84監控】【{EquipmentPlcOffset.E84PC_DB}{EquipmentPlcOffset.E84PC_VALUE}】寫入PLC資料:HO_AVBL:{e84pc[0]}，ES:{e84pc[1]}，L_REQ:{e84pc[2]}，U_REQ:{e84pc[3]}，READY:{e84pc[4]}，VA:{e84pc[6]}，VS_0:{e84pc[7]}，VS_1:{e84pc[8]}");
                                ScLiftE84Pc.RESULT = SAA_DatabaseEnum.SendFlag.Y.ToString();
                                SAA_Database.SaaSql?.UpdateScLiftE84Pc(ScLiftE84Pc);
                            }
                        }
                        Thread.Sleep(500);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        #region [===讀取監控上報點位===]
        public async void ReadPlcActionOffset()
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        //EquipmentOffset.PlcRequest = SaaEquipmentPlc?.ReadIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_Track, SAA_Database.saaequipmentbitoffset.SaaValue_Track, 7)!;
                        PlcActionOffset();
                        Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void PlcActionOffset()
        {
            try
            {
                EquipmentOffset.PlcRequest = SaaEquipmentPlc?.ReadIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_Track, SAA_Database.saaequipmentbitoffset.SaaValue_Track, 7)!;
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        #region [===監控在籍上報===]
        public async void SaaEquipmentMonitorStart(int setno, string model_name, string station_name)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        SaaEquipmentStart(setno, model_name, station_name);
                        Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void SaaEquipmentStart(int setno, string model_name, string station_naem)
        {
            if (MonitorStart)
            {
                int[] plcmodel = { 0, 1, 0, 0 };
                SAA_Database.LogMessage($"【{station_naem}】【模式切換】【{SAA_Database.saaequipmentbitoffset.SaaDB_SynchronizeAck}{SAA_Database.saaequipmentbitoffset.SaaValue_SynchronizeAck}】資料同步模式開始");
                SAA_Database.LogMessage($"【{station_naem}】【模式切換】【{SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck}{SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck}】物料追蹤模式停止");
                ReadOffset();
                SaaEquipmentPlc?.WriteIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck, SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck, plcmodel);
                MonitorStart = false;
                SaaEquipmentgroup.DataTrack = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataTrack.ToString()];
                SaaEquipmentgroup.DataReply = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataReply.ToString()];
                SaaEquipmentgroup.DataLocal = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataLocal.ToString()];
                SaaEquipmentgroup.DataRemote = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataRemote.ToString()];
                SaaEquipmentgroup.DataEnrollmentInfo = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataEnrollmentInfo.ToString()];
                SaaEquipmentgroup.DataCarrierId = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataCarrierId.ToString()];
                SaaEquipmentgroup.DataTrackAck = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataTrackAck.ToString()];
                SaaEquipmentgroup.DataReplyAck = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataReplyAck.ToString()];
                SaaEquipmentgroup.DataLocalAck = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataLocalAck.ToString()];
                SaaEquipmentgroup.DataRemoteAck = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataRemoteAck.ToString()];
                SaaEquipmentgroup.DataCarrrierIdAck = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataCarrrierIdAck.ToString()];
                SaaEquipmentgroup.DataReplyInfo = SAA_Database.DictionaryOffset1[SAA_DatabaseEnum.SaaOffsetGroup.DataReplyInfo.ToString()];
                UpdOffsetOff();
            }
            if (EquipmentOffset.PlcRequest != null)
            {
                if (EquipmentOffset.PlcRequest[0] == 0 && EquipmentOffset.PlcRequest[1] == 0)
                {
                    EquipmentOffset.PcRequest = SaaEquipmentPlc?.ReadIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck, SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck, 7)!;
                    if (EquipmentOffset.PcRequest[1] == 0)
                    {
                        int[] plcmodel = { 0, 1, 0, 0 };
                        SaaEquipmentPlc?.WriteIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck, SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck, plcmodel);
                        SAA_Database.LogMessage($"【{station_naem}】【模式切換】【重新連線】【{SAA_Database.saaequipmentbitoffset.SaaDB_SynchronizeAck}{SAA_Database.saaequipmentbitoffset.SaaValue_SynchronizeAck}】資料同步模式開始");
                        SAA_Database.LogMessage($"【{station_naem}】【模式切換】【重新連線】【{SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck}{SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck}】物料追蹤模式停止");
                        UpdOffsetOff();
                    }
                }

                if (EquipmentOffset.PlcRequest[2] == 1)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    SAA_Database.LogMessage($"【{station_naem}】【監控上報】【{SAA_Database.saaequipmentbitoffset.SaaDB_Request}{SAA_Database.saaequipmentbitoffset.SaaValue_Request}】接收到PLC訊號為{EquipmentOffset.PlcRequest[2]}");
                    int[] ReadPlc = SaaEquipmentPlc?.ReadIntArray(SAA_Database.configattributes.SaaOffsetWord, SAA_Database.saaequipmentbitoffset.SaaValue_DataCount, 107)!;
                    SaaEquipmentPlc?.WriteInt(SAA_Database.saaequipmentbitoffset.SaaDB_RequestAck, SAA_Database.saaequipmentbitoffset.SaaValue_RequestAck, 1);
                    SAA_Database.LogMessage($"【{station_naem}】【監控上報】【{SAA_Database.saaequipmentbitoffset.SaaDB_RequestAck}{SAA_Database.saaequipmentbitoffset.SaaValue_RequestAck}】寫入PC訊號為True");
                    while (true)
                    {
                        Thread.Sleep(50);
                        if (EquipmentOffset.PlcRequest[2] == 1) continue;
                        SaaEquipmentPlc?.WriteInt(SAA_Database.saaequipmentbitoffset.SaaDB_RequestAck, SAA_Database.saaequipmentbitoffset.SaaValue_RequestAck, 0);
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【{SAA_Database.saaequipmentbitoffset.SaaDB_Request}{SAA_Database.saaequipmentbitoffset.SaaValue_Request}】接收到PLC訊號為{EquipmentOffset.PlcRequest[2]}");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【{SAA_Database.saaequipmentbitoffset.SaaDB_RequestAck}{SAA_Database.saaequipmentbitoffset.SaaValue_RequestAck}】PC寫入訊號為False");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】交握完成，總共花費 {stopwatch.ElapsedMilliseconds} ms");
                        stopwatch.Stop();
                        break;
                    }
                    GroupCount = ReadPlc![SAA_Database.EquipmentCommon.GroupCount];
                    for (int i = 0; i < GroupCount; i++)
                    {
                        int[] datalocal = new int[10];
                        int[] dataremote = new int[10];
                        int[] datacarrierid = new int[10];
                        int No = 1 + i;
                        for (int j = 0; j < datalocal.Length; j++)
                        {
                            datalocal[j] = ReadPlc[SaaDataLocal[i] + j];
                            dataremote[j] = ReadPlc[SaaDataRemote[i] + j];
                            datacarrierid[j] = ReadPlc[SaaDataCarrierId[i] + j];
                        }

                        SaaEquipmentAttributes SaaEquipmentgroup = new SaaEquipmentAttributes
                        {
                            DataTrack = ReadPlc[SaaDataTrack[i]].ToString(),
                            DataReply = ReadPlc[SaaDaataReply[i]].ToString(),
                            DataLocal = ReadSringArray(datalocal),
                            DataRemote = ReadSringArray(dataremote),
                            DataCarrierId = ReadSringArray(datacarrierid),
                        };

                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】讀取資料類別:{SaaEquipmentgroup.DataTrack}(1=物料追蹤-搬移, 2=物料追蹤-更新, 4=物料追蹤-建立, 8=物料追蹤-清除, 16=物料追蹤-詢問, 32=物料追蹤-答覆, 64=資料同步-有帳, 128=資料同步-無帳)");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】取詢問類型:{SaaEquipmentgroup.DataReply}");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】讀取位置-來源:{SaaEquipmentgroup.DataRemote}");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】讀取位置-本地:{SaaEquipmentgroup.DataLocal}");
                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】取卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                        if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Move)
                        {
                            var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(station_naem, SaaEquipmentgroup.DataCarrierId);
                            SaaScLocationSetting locationsettinglocal = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataLocal,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                PARTNO = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.PARTNO.ToString()].ToString()! : string.Empty,
                                INVENTORYFULL = 1,
                                PUTTIME = SAA_Database.ReadTime(),
                                OPER = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.OPER.ToString()].ToString()! : string.Empty,
                                CARRIERSTATE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.CARRIERSTATE.ToString()].ToString()! : string.Empty,
                                DESTINATIONTYPE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString()! : string.Empty,
                                LOCATIONTYPE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString() == SAA_DatabaseEnum.LOCATIONTYPE.Buffer_Global.ToString() ? SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global.ToString() : SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString() : SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString(),
                            };

                            SaaScLocationSetting locationsettingremote = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataRemote,
                                CARRIERID = string.Empty,
                                PARTNO = string.Empty,
                                INVENTORYFULL = 0,
                                PUTTIME = string.Empty,
                                OPER = string.Empty,
                                CARRIERSTATE = string.Empty,
                                DESTINATIONTYPE = string.Empty,
                                CARRIERID1 = SaaEquipmentgroup.DataCarrierId,
                                LOCATIONTYPE = SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()
                            };
                            #region [===建立帳料===]
                            //建立帳料
                            var locationsettinglocaldata = SAA_Database.SaaSql?.GetScLocationSetting(locationsettinglocal);
                            if (locationsettinglocaldata != null)
                            {
                                if (locationsettinglocaldata.Rows.Count != 0)
                                {
                                    string locationtype = locationsettinglocaldata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()!;
                                    locationsettinglocal.HOSTID = locationsettinglocaldata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.HOSTID.ToString()].ToString()!;
                                    if (locationtype.Contains(SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()))
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSettingLocationType(locationsettinglocal);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新儲格 儲格位置: {locationsettinglocal.LOCATIONID}，卡匣ID:{locationsettinglocal.CARRIERID}，HOSTID儲格名稱:{locationsettinglocal.HOSTID}，變更儲格屬性:{locationsettinglocal.LOCATIONTYPE}");
                                    }
                                    else
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSetting(locationsettinglocal);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新位置 機構位置: {locationsettinglocal.LOCATIONID}，卡匣ID:{locationsettinglocal.CARRIERID}，HOSTID位置名稱:{locationsettinglocal.HOSTID}");
                                    }
                                }
                            }
                            #endregion

                            #region [===清除帳料===]
                            //清除帳料
                            var locationsettingremotedata = SAA_Database.SaaSql?.GetScLocationSetting(locationsettingremote);
                            if (locationsettingremotedata != null)
                            {
                                if (locationsettingremotedata.Rows.Count != 0)
                                {
                                    string locationtype = locationsettingremotedata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()!;
                                    locationsettingremote.HOSTID = locationsettingremotedata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.HOSTID.ToString()].ToString()!;
                                    if (locationtype.Contains(SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()))
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSettingLocationType(locationsettingremote);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除儲格 儲格位置: {locationsettingremote.LOCATIONID}，儲格屬性:{locationsettingremote.LOCATIONTYPE}，HOSTID位置名稱:{locationsettingremote.HOSTID}");
                                    }
                                    else
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSetting(locationsettingremote);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除位置資料 機構位置: {locationsettingremote.LOCATIONID}，屬性:{locationtype}，HOSTID位置名稱:{locationsettingremote.HOSTID}");
                                    }
                                }
                            } 
                            #endregion

                            SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                            {
                                TASKDATETIME = SAA_Database.ReadTime(),
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATION_NAEM = station_naem,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,//.Contains(SAA_Database.EquipmentCommon.ReaderError) ? "ERROR" : SaaEquipmentgroup.DataCarrierId,
                                REPORE_DATATRACK = SaaEquipmentgroup.DataTrack,
                                REPORE_DATAREMOTE = SaaEquipmentgroup.DataRemote,
                                REPORE_DATALOCAL = SaaEquipmentgroup.DataLocal,
                            };
                            SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                            SAA_Database.LogMessage($"【{station_naem}】【監控上報】【第{No}組】設備上報資訊=>MAG ({SaaEquipmentgroup.DataCarrierId}) 從 {SaaEquipmentgroup.DataRemote} 抓取到 {SaaEquipmentgroup.DataLocal}");
                            if (SaaEquipmentgroup.DataRemote == "DK-OUT" && SaaEquipmentgroup.DataLocal == "CRANE")
                            {
                                var devicedata = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                                if (devicedata != null)
                                {
                                    if (devicedata?.Rows.Count != 0)
                                    {
                                        string devicetype = devicedata?.Rows[0]["DEVICETYPE"].ToString()!;
                                        if (devicetype == "1")//1:LD 2:ULD
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "2",//1:實貨2:空盒,
                                                REMOTE = "DK-OUT",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);
                                            }
                                            SaaEquipmentCarrierInfo saaequipmentcarrierinfo = new SaaEquipmentCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATIOM_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                PARTNO = "NA",
                                                CARRIERTYOE = SAA_DatabaseEnum.CarrierTyoe.Normal.ToString(),
                                                CARRIERSTATE = SAA_DatabaseEnum.CarrierState.Empty.ToString(),
                                                DESTINATIONTYPE = SAA_DatabaseEnum.DestinationType.EQP.ToString(),
                                                OPER = "NA",
                                                ROTFLAG = "0",
                                                FLIPFLAG = "0",
                                                REJECT_CODE = string.Empty,
                                                REJECT_MESSAGE = string.Empty,
                                            };
                                            if (SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(saaequipmentcarrierinfo.STATIOM_NAME, saaequipmentcarrierinfo.CARRIERID).Rows.Count != 0)
                                            {
                                                SAA_Database.SaaSql?.DelScEquipmentCarrierInfo(saaequipmentcarrierinfo.STATIOM_NAME, saaequipmentcarrierinfo.CARRIERID);
                                                SAA_Database.LogMessage($"【{station_naem}】已刪除卡匣資訊卡匣資訊卡匣ID:{saaequipmentcarrierinfo.CARRIERID}");
                                            }
                                            SAA_Database.SaaSql?.SetScEquipmentCarrierInfo(saaequipmentcarrierinfo);
                                            SAA_Database.LogMessage($"【{station_naem}】新增卡匣資訊卡匣資訊卡匣ID:{saaequipmentcarrierinfo.CARRIERID}，卡匣屬性:{saaequipmentcarrierinfo.CARRIERSTATE}，卡匣目的位置:{saaequipmentcarrierinfo.DESTINATIONTYPE}");
                                        }
                                        else
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "1",//1:實貨2:空盒
                                                REMOTE = "DK-OUT",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                    SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】已刪除卡匣資訊LIFT卡匣資訊卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);

                                            }
                                        }
                                    }
                                }
                            }
                            if (SaaEquipmentgroup.DataRemote == "CRANE" && SaaEquipmentgroup.DataLocal == "RGV")
                            {
                                SaaScLiftCarrierAmount LiftCarrierAmount = new SaaScLiftCarrierAmount
                                {
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAME = station_naem,
                                    TASKDATETIME = SAA_Database.ReadTime(),
                                    ACTION = "L",
                                    CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                };
                                SAA_Database.SaaSql?.SetScLiftAmount(LiftCarrierAmount);

                                SAA_Database.SaaSql?.DelScTransportrequirement(LiftCarrierAmount.STATION_NAME, LiftCarrierAmount.CARRIERID);
                                SAA_Database.LogMessage($"刪除ScTransportrequirement資料(叫車資料表)，站點:{LiftCarrierAmount.STATION_NAME}，卡匣ID:{LiftCarrierAmount.CARRIERID}");
                            }
                            else if (SaaEquipmentgroup.DataRemote == "RGV" && SaaEquipmentgroup.DataLocal == "CRANE")
                            {
                                var devicedata = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                                if (devicedata != null)
                                {
                                    if (devicedata?.Rows.Count != 0)
                                    {
                                        string devicetype = devicedata?.Rows[0]["DEVICETYPE"].ToString()!;
                                        if (devicetype == "1")//1:LD 2:ULD
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "1",//1:實貨2:空盒
                                                REMOTE = "RGV",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);
                                            }
                                        }
                                        else
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "2",//1:實貨2:空盒
                                                REMOTE = "RGV",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);
                                            }
                                        }
                                    }
                                }

                                SaaScLiftCarrierAmount LiftCarrierAmount = new SaaScLiftCarrierAmount
                                {
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAME = station_naem,
                                    TASKDATETIME = SAA_Database.ReadTime(),
                                    ACTION = "U",
                                    CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                };
                                SAA_Database.SaaSql?.SetScLiftAmount(LiftCarrierAmount);

                                //var carrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(EquipmentReport.STATION_NAEM, EquipmentReport.CARRIERID);
                                //if (carrierinfo?.Rows.Count != 0)
                                //{
                                //string destinationtype = carrierinfo?.Rows[0]["DESTINATIONTYPE"].ToString()!;
                                //SAA_Database.LogMessage($"【{EquipmentReport.STATION_NAEM}】卡匣DESTINATIONTYPE:{destinationtype}，卡匣ID:{EquipmentReport.CARRIERID}");
                                //if (destinationtype == SAA_DatabaseEnum.DestinationType.EQP.ToString())
                                //{
                                if (!EquipmentReport.CARRIERID.Contains(SAA_Database.configattributes.PARTICLE))
                                {
                                    var data = SAA_Database.SaaSql?.GetScCommandTask(station_naem);
                                    if (data?.Rows.Count == 0)
                                    {
                                        var ScCommandTask = SAA_Database.SaaSql?.GetScCommandTaskResult(setno.ToString(), model_name, station_naem);
                                        if (ScCommandTask?.Rows.Count == 0)
                                        {
                                            var liftcarrierinfoempty = SAA_Database.SaaSql?.GetScLiftCarrierInfoMaterial(setno.ToString(), model_name, station_naem);
                                            if (liftcarrierinfoempty?.Rows.Count == 0)
                                            {
                                                int[] EquipmentSiteOffset = SaaEquipmentPlc?.ReadIntArray("B", "6C0", 32)!;
                                                //進製程
                                                if (EquipmentSiteOffset?[19] == 1)
                                                {
                                                    SaaScCommandTask CommandTask = new SaaScCommandTask
                                                    {
                                                        TASKDATETIME = SAA_Database.ReadTime(),
                                                        SETNO = setno,
                                                        MODEL_NAME = model_name,
                                                        STATION_NAME = station_naem,
                                                        CARRIERID = EquipmentReport.CARRIERID,
                                                        COMMANDID = $"{DateTime.Now:fff}",
                                                        LOCATIONPUT = "DK-IN",
                                                        LOCATIONTAKE = "CRANE",

                                                    };
                                                    SAA_Database.SaaSql?.SetScCommandTask(CommandTask);
                                                    SAA_Database.LogMessage($"【{CommandTask.STATION_NAME}】新增SC_COMMAND_TASK資料卡匣ID:{CommandTask.CARRIERID}");
                                                    SAA_Database.SaaSql?.SetScCommandTaskHistory(CommandTask);
                                                }
                                            }
                                        }
                                    }
                                }
                                //}
                                //}
                                //else
                                //{
                                //    SAA_Database.LogMessage($"【{station_naem}】【監控上報】查無此卡匣資訊(ScEquipmentCarrierInfo) 卡匣ID:{EquipmentReport.CARRIERID}");
                                //}
                            }
                            else if (SaaEquipmentgroup.DataRemote == "PGV-IN" && SaaEquipmentgroup.DataLocal == "CRANE")
                            {
                                var devicedata = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                                if (devicedata != null)
                                {
                                    if (devicedata?.Rows.Count != 0)
                                    {
                                        string devicetype = devicedata?.Rows[0]["DEVICETYPE"].ToString()!;
                                        if (devicetype == "1")//1:LD 2:ULD
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "1",//1:實貨2:空盒,
                                                REMOTE = "PGV-IN",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);
                                            }
                                        }
                                        else
                                        {
                                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                            {
                                                SETNO = setno,
                                                MODEL_NAME = model_name,
                                                STATION_NAME = station_naem,
                                                CARRIERID = EquipmentReport.CARRIERID,
                                                CARRIERTYPE = "2",//1:實貨2:空盒
                                                REMOTE = "PGV-IN",
                                            };
                                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                            if (liftcarrierdata != null)
                                            {
                                                if (liftcarrierdata.Rows.Count != 0)
                                                {
                                                    SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                                    SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】已刪除卡匣資訊LIFT卡匣資訊卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                                }
                                                SAA_Database.SaaSql?.SetScLiftCarrierInfo(LiftCarrierInfo);

                                            }
                                        }
                                    }
                                }
                            }
                            if (SaaEquipmentgroup.DataLocal == "DK-IN")
                            {
                                SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                {
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAME = station_naem,
                                    CARRIERID = EquipmentReport.CARRIERID,
                                };
                                var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                                if (liftcarrierdata != null)
                                {
                                    if (liftcarrierdata.Rows.Count != 0)
                                    {
                                        SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                        SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】已刪除卡匣資訊LIFT卡匣資訊卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    }
                                    else
                                    {
                                        SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】查無卡匣資訊LIFT卡匣資訊卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    }
                                }
                            }

                            if (SaaEquipmentgroup.DataLocal == "DK-IN" || SaaEquipmentgroup.DataLocal == "PGV-OUT")
                            {
                                var commandtask = SAA_Database.SaaSql?.GetScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                if (commandtask?.Rows.Count != 0)
                                {
                                    SAA_Database.SaaSql?.DelScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除SC_COMMAND_TASK資料，卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                                }
                                else
                                {
                                    SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】查無卡匣資訊SC_COMMAND_TASK匣資訊卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                                    SAA_Database.SaaSql?.DelScCommandTask(SaaEquipmentgroup.DataCarrierId);
                                    SAA_Database.LogMessage($"【{station_naem}】【{SaaEquipmentgroup.DataLocal}】刪除SC_COMMAND_TASK匣資訊卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                                }
                            }

                            var LocationidShelfdata = SAA_Database.SaaSql?.GetScLocationsettingLocationidShelf(station_naem, SaaEquipmentgroup.DataLocal);
                            if (LocationidShelfdata != null)
                            {
                                if (LocationidShelfdata?.Rows.Count != 0)
                                {
                                    if (LocationidShelfdata?.Rows[0]["LOCATIONTYPE"].ToString() == "Shelf")
                                    {
                                        var commandtaskdata = SAA_Database.SaaSql?.GetScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                        if (commandtaskdata != null)
                                        {
                                            if (commandtaskdata.Rows.Count != 0)
                                            {
                                                SAA_Database.SaaSql?.DelScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                                SAA_Database.LogMessage($"【{station_naem}】【刪除資料】重新放回儲格 卡匣ID:{SaaEquipmentgroup.DataCarrierId} 儲格位置: {SaaEquipmentgroup.DataLocal}，刪除搬運資料SC_COMMAND_TASK(站點:{station_naem}卡匣ID:{SaaEquipmentgroup.DataCarrierId})");
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #region [===建帳與有帳===]
                        if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Establish || SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Have)
                        {
                            var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(station_naem, SaaEquipmentgroup.DataCarrierId);
                            SaaScLocationSetting locationsettinglocal = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataLocal,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                PARTNO = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.PARTNO.ToString()].ToString()! : string.Empty,
                                INVENTORYFULL = 1,
                                PUTTIME = SAA_Database.ReadTime(),
                                OPER = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.OPER.ToString()].ToString()! : string.Empty,
                                CARRIERSTATE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.CARRIERSTATE.ToString()].ToString()! : string.Empty,
                                DESTINATIONTYPE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString()! : string.Empty,
                                LOCATIONTYPE = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString() == SAA_DatabaseEnum.LOCATIONTYPE.Buffer_Global.ToString() ? SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global.ToString() : SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString() : string.Empty,
                            };

                            SaaScLocationSetting locationsettingremote = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataRemote,
                                CARRIERID = string.Empty,
                                PARTNO = string.Empty,
                                INVENTORYFULL = 0,
                                PUTTIME = string.Empty,
                                OPER = string.Empty,
                                CARRIERSTATE = string.Empty,
                                DESTINATIONTYPE = string.Empty,
                                CARRIERID1 = SaaEquipmentgroup.DataCarrierId,
                                LOCATIONTYPE = SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()
                            };

                            #region [===更新設備儲格狀態===]
                            //建立帳料
                            var locationsettinglocaldata = SAA_Database.SaaSql?.GetScLocationSetting(locationsettinglocal);
                            if (locationsettinglocaldata != null)
                            {
                                if (locationsettinglocaldata.Rows.Count != 0)
                                {
                                    string locationtype = locationsettinglocaldata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()!;
                                    locationsettinglocal.HOSTID = locationsettinglocaldata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.HOSTID.ToString()].ToString()!;
                                    if (locationtype.Contains(SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()))
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSettingLocationType(locationsettinglocal);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新儲格 儲格位置: {locationsettinglocal.LOCATIONID}，卡匣ID:{locationsettinglocal.CARRIERID}，HOSTID儲格名稱:{locationsettinglocal.HOSTID}，變更儲格屬性:{locationsettinglocal.LOCATIONTYPE}");
                                    }
                                    else
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSetting(locationsettinglocal);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新位置 機構位置: {locationsettinglocal.LOCATIONID}，卡匣ID:{locationsettinglocal.CARRIERID}，HOSTID位置名稱:{locationsettinglocal.HOSTID}");
                                    }
                                }
                            }

                            //清除帳料
                            var locationsettingremotedata = SAA_Database.SaaSql?.GetScLocationSetting(locationsettingremote);
                            if (locationsettingremotedata != null)
                            {
                                if (locationsettingremotedata.Rows.Count != 0)
                                {
                                    string locationtype = locationsettingremotedata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()!;
                                    locationsettingremote.HOSTID = locationsettingremotedata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.HOSTID.ToString()].ToString()!;
                                    if (locationtype.Contains(SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()))
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSettingLocationType(locationsettingremote);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除儲格 儲格位置: {locationsettingremote.LOCATIONID}，儲格屬性:{locationsettingremote.LOCATIONTYPE}，HOSTID位置名稱:{locationsettingremote.HOSTID}");
                                    }
                                    else
                                    {
                                        SAA_Database.SaaSql?.UpdScLocationSetting(locationsettingremote);
                                        SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除位置資料 機構位置: {locationsettingremote.LOCATIONID}，屬性:{locationtype}，HOSTID位置名稱:{locationsettingremote.HOSTID}");
                                    }
                                }
                            }
                            #endregion

                            #region [===更新iLIS儲格狀態(未用到)===]
                            //var tmp = SAA_Database.SaaSql?.GetScLocationSetting(locationsettinglocal);
                            //更新iLIS儲格狀態
                            //var shelfglobaldata = SAA_Database.SaaSql?.GetScLocationSettingLocationType(station_naem, SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global.ToString());
                            //var shelfdata = SAA_Database.SaaSql?.GetScLocationSettingLocationType(station_naem, SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString());
                            //var devicedata = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                            //if (shelfglobaldata != null && shelfdata != null && devicedata != null && tmp?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString() == SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString())
                            //{
                            //    string deviceoper = devicedata.Rows.Count != 0 ? devicedata.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.OPER.ToString()].ToString()! : string.Empty;
                            //    string carrierstate = equipmentcarrierinfo!.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.CARRIERSTATE.ToString()].ToString()! : string.Empty;
                            //    string destinationtype = equipmentcarrierinfo!.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString()! : string.Empty;
                            //    var locationilisdata = SAA_Database.SaaSql?.GetScLocationSettingiLIS(locationsettingremote);
                            //    string shelfilis = locationilisdata!.Rows.Count != 0 ? locationilisdata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()! : string.Empty;
                            //    if (shelfilis == SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString())
                            //    {
                            //        SAA_Database.SaaSql?.UpdScLocationSettingiLISClear(locationsettingremote);
                            //        SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除儲格 儲格位置: {locationsettingremote.LOCATIONID}，儲格屬性:{shelfilis}");
                            //    }
                            //    else
                            //    {
                            //        var locationtypecelldata = SAA_Database.SaaSql?.GetScLocationSettingLocationTypeCell(station_naem, SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global.ToString());
                            //        if (locationtypecelldata?.Rows.Count > 4)
                            //        {
                            //            SAA_Database.SaaSql?.UpdScLocationSettingiLISClearLocationType(locationsettingremote);
                            //            SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除儲格 儲格位置: {locationsettingremote.LOCATIONID}，儲格屬性:{SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global}");
                            //        }
                            //        else
                            //        {
                            //            SAA_Database.SaaSql?.UpdScLocationSettingiLISClear(locationsettingremote);
                            //            SAA_Database.LogMessage($"【{station_naem}】【更新資料】清除儲格 儲格位置: {locationsettingremote.LOCATIONID}，變更儲格屬性:{locationsettingremote.DESTINATIONTYPE}");
                            //        }
                            //    }

                            //    if (destinationtype == SAA_DatabaseEnum.DestinationType.Shelf_Global.ToString())
                            //    {
                            //        if (shelfglobaldata.Rows.Count != 0)
                            //        {
                            //            SaaScLocationSetting locationsettinglocalilis = new SaaScLocationSetting
                            //            {
                            //                SETNO = setno,
                            //                MODEL_NAME = model_name,
                            //                STATIOM_NAME = station_naem,
                            //                LOCATIONID = shelfglobaldata.Rows.Count != 0 ? shelfglobaldata.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONID.ToString()].ToString()! : string.Empty,
                            //                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                            //                PARTNO = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.PARTNO.ToString()].ToString()! : string.Empty,
                            //                INVENTORYFULL = 1,
                            //                HOSTID = shelfglobaldata.Rows.Count != 0 ? shelfglobaldata.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.HOSTID.ToString()].ToString()! : string.Empty,
                            //                PUTTIME = SAA_Database.ReadTime(),
                            //                OPER = deviceoper,
                            //                CARRIERSTATE = carrierstate,
                            //            };
                            //            SAA_Database.SaaSql?.UpdScLocationSettingiLIS(locationsettinglocalilis);
                            //            SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新儲格 儲格位置: {locationsettingremote.LOCATIONID}，卡匣ID:{locationsettingremote.CARRIERID}，儲格名稱:{locationsettingremote.HOSTID}，儲格屬性:{SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global}");
                            //        }
                            //        else
                            //        {
                            //            SaaScLocationSetting locationsettinglocalilis = new SaaScLocationSetting
                            //            {
                            //                SETNO = setno,
                            //                MODEL_NAME = model_name,
                            //                STATIOM_NAME = station_naem,
                            //                LOCATIONID = shelfdata.Rows.Count != 0 ? shelfdata.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONID.ToString()].ToString()! : string.Empty,
                            //                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                            //                PARTNO = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.PARTNO.ToString()].ToString()! : string.Empty,
                            //                INVENTORYFULL = 1,
                            //                PUTTIME = SAA_Database.ReadTime(),
                            //                OPER = deviceoper,
                            //                CARRIERSTATE = carrierstate,
                            //                LOCATIONTYPE = SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global.ToString(),
                            //            };
                            //            SAA_Database.SaaSql?.UpdScLocationSettingiLISLocationType(locationsettinglocalilis);
                            //            SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新儲格 儲格位置: {locationsettingremote.LOCATIONID}，卡匣ID:{locationsettingremote.CARRIERID}，儲格名稱:{locationsettingremote.HOSTID}，變更儲格屬性:{SAA_DatabaseEnum.LOCATIONTYPE.Shelf_Global}");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        SaaScLocationSetting locationsettinglocalilis = new SaaScLocationSetting
                            //        {
                            //            SETNO = setno,
                            //            MODEL_NAME = model_name,
                            //            STATIOM_NAME = station_naem,
                            //            LOCATIONID = shelfdata.Rows.Count != 0 ? shelfdata.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONID.ToString()].ToString()! : string.Empty,
                            //            CARRIERID = SaaEquipmentgroup.DataCarrierId,
                            //            PARTNO = equipmentcarrierinfo?.Rows.Count != 0 ? equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.PARTNO.ToString()].ToString()! : string.Empty,
                            //            INVENTORYFULL = 1,
                            //            PUTTIME = SAA_Database.ReadTime(),
                            //            OPER = deviceoper,
                            //            CARRIERSTATE = carrierstate,
                            //        };
                            //        SAA_Database.SaaSql?.UpdScLocationSettingiLIS(locationsettinglocalilis);
                            //        SAA_Database.LogMessage($"【{station_naem}】【更新資料】更新儲格 儲格位置: {locationsettingremote.LOCATIONID}，卡匣ID:{locationsettingremote.CARRIERID}，儲格名稱:{locationsettingremote.HOSTID}，屬性:{SAA_DatabaseEnum.LOCATIONTYPE.Shelf}");
                            //    }
                            //} 
                            #endregion

                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATION_NAME = station_naem,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                            };

                            var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfo(LiftCarrierInfo);
                            if (SAA_Database.SaaSql?.GetScLocationSetting(locationsettinglocal).Rows.Count != 0)
                            {

                                if (liftcarrierdata!.Rows != null)
                                {
                                    if (liftcarrierdata.Rows.Count != 0)
                                    {
                                        locationsettinglocal.LOCATIONMODE = liftcarrierdata.Rows[0]["CARRIERTYPE"].ToString()!;
                                        SAA_Database.SaaSql?.UpdScLocationSettingMode(locationsettinglocal);
                                    }
                                }
                            }
                            else
                            {
                                SAA_Database.SaaSql?.SetScLocationSetting(locationsettinglocal);
                            }

                            string pattern = "[^a-zA-Z0-9]";
                            string clean = Regex.Replace(SaaEquipmentgroup.DataLocal, pattern, "");
                            if (Enum.IsDefined(typeof(SAA_DatabaseEnum.EquipmentType), clean))
                            {
                                string db = string.Empty;
                                string offset = string.Empty;
                                switch ((SAA_DatabaseEnum.EquipmentType)Enum.Parse(typeof(SAA_DatabaseEnum.EquipmentType), clean))
                                {
                                    case SAA_DatabaseEnum.EquipmentType.PGVOUT:
                                        db = SAA_Database.saaequipmentbitoffset.SaaDB_PUGVOUT;
                                        offset = SAA_Database.saaequipmentbitoffset.SaaValue_PUGVOUT;
                                        break;
                                    case SAA_DatabaseEnum.EquipmentType.DKIN:
                                        db = SAA_Database.saaequipmentbitoffset.SaaDB_DKIN;
                                        offset = SAA_Database.saaequipmentbitoffset.SaaValue_DKIN;
                                        break;
                                }
                                SaaEquipmentPlc?.WriteInt(db, offset, 0);
                                SAA_Database.LogMessage($"【{station_naem}】【手臂搬運】【{db}{offset}】【卡匣傳送至{SaaEquipmentgroup.DataLocal}】PC已傳送命令給PLC，{clean}訊號為 false");
                            }
                        } 
                        #endregion

                        #region [===無帳或清除資料===]
                        if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Clear || SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.None)
                        {
                            if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Clear)
                            {
                                var commandtask = SAA_Database.SaaSql?.GetScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                if (commandtask?.Rows.Count != 0)
                                {
                                    string locationput = commandtask?.Rows[0]["LOCATIONPUT"].ToString()!;
                                    if (locationput == "PGV-OUT")
                                    {
                                        SaaEquipmentPlc?.WriteBool("B", "2C0", false);
                                        SAA_Database.LogMessage($"【{station_naem}】【手臂搬運】【卡匣傳送至PGV-OUT】PC已傳送命令給PLC，W_FICS為 False");
                                    }
                                    else if (locationput == "DK-IN")
                                    {
                                        SaaEquipmentPlc?.WriteBool("B", "2B0", false);
                                        SAA_Database.LogMessage($"【{station_naem}】【手臂搬運】【卡匣傳送至DK-IN】PC已傳送命令給PLC，W_FICS為 False");
                                    }
                                    SAA_Database.SaaSql?.DelScCommandTask(station_naem, SaaEquipmentgroup.DataCarrierId);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除SC_COMMAND_TASK資料，卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                                }

                                SaaScLocationSetting locationsettingremote = new SaaScLocationSetting
                                {
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATIOM_NAME = station_naem,
                                    LOCATIONID = SaaEquipmentgroup.DataLocal,
                                    CARRIERID = string.Empty,
                                    PARTNO = string.Empty,
                                    INVENTORYFULL = 0,
                                    PUTTIME = string.Empty,
                                    OPER = string.Empty,
                                    CARRIERSTATE = string.Empty,
                                    DESTINATIONTYPE = string.Empty,
                                    LOCATIONTYPE = SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()
                                };

                                var locationilisdata = SAA_Database.SaaSql?.GetScLocationSetting(locationsettingremote);
                                string shelfilis = locationilisdata!.Rows.Count != 0 ? locationilisdata?.Rows[0][SAA_DatabaseEnum.SC_LOCATIONSETTING.LOCATIONTYPE.ToString()].ToString()! : string.Empty;
                                if (shelfilis.Contains(SAA_DatabaseEnum.LOCATIONTYPE.Shelf.ToString()))
                                {
                                    SAA_Database.SaaSql?.UpdScLocationSettingLocationType(locationsettingremote);
                                }

                                SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                {
                                    TASKDATETIME = SAA_Database.ReadTime(),
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAEM = station_naem,
                                    CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                    REPORE_DATATRACK = SaaEquipmentgroup.DataTrack,
                                    REPORE_DATAREMOTE = SaaEquipmentgroup.DataRemote,
                                    REPORE_DATALOCAL = SaaEquipmentgroup.DataLocal,
                                };
                                SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);

                                SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                                {
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAME = station_naem,
                                    CARRIERID = EquipmentReport.CARRIERID,
                                };
                                SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                SAA_Database.LogMessage($"【{station_naem}】刪除LiftCarrierInfo資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                SAA_Database.SaaSql?.DelScTransportrequirement(LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                SAA_Database.LogMessage($"【{station_naem}】刪除ScTransportrequirement資料(叫車資料表)，站點:{LiftCarrierInfo.STATION_NAME}，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                if (SaaEquipmentgroup.DataLocal != "RGV")
                                {
                                    SAA_Database.SaaSql?.DelScLiftCarrierInfoEmpty(LiftCarrierInfo.SETNO, LiftCarrierInfo.MODEL_NAME, LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除LiftCarrierInfoEmpty資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    SAA_Database.SaaSql?.DelScLiftCarrierInfoMaterial(LiftCarrierInfo.SETNO, LiftCarrierInfo.MODEL_NAME, LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除LiftCarrierInfoMaterial資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    SAA_Database.SaaSql?.DelScLiftCarrierInfoReject(LiftCarrierInfo.SETNO, LiftCarrierInfo.MODEL_NAME, LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除LiftCarrierInfoReject資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    //SAA_Database.SaaSql?.DelScEquipmentCarrierInfo(LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                    //SAA_Database.LogMessage($"【{station_naem}】刪除ScEquipmentCarrierInfo資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                    SAA_Database.SaaSql?.DelScCommandTask(LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                                    SAA_Database.LogMessage($"【{station_naem}】刪除SC_COMMAND_TASK資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                                }
                            }

                            SaaScLocationSetting locationsetting = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataLocal,
                                INVENTORYFULL = 0,
                            };
                            var locationsettingdata = SAA_Database.SaaSql?.GetScLocationSetting(locationsetting);
                            if (locationsettingdata != null)
                            {
                                if (locationsettingdata.Rows.Count != 0)
                                    SAA_Database.SaaSql?.UpdScLocationSetting(locationsetting);
                                else
                                    SAA_Database.SaaSql?.SetScLocationSetting(locationsetting);
                            }
                        }
                        #endregion

                        #region [===更新資料===]
                        if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Update)
                        {
                            SaaScLocationSetting locationsetting = new SaaScLocationSetting
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATIOM_NAME = station_naem,
                                LOCATIONID = SaaEquipmentgroup.DataLocal,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                            };
                            SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                            {
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATION_NAME = station_naem,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                CARRIERID_UPDATE = "Y",
                            };
                            string carrierid = string.Empty;
                            var locationsettingdata = SAA_Database.SaaSql?.GetScLocationSetting(locationsetting);
                            if (locationsettingdata != null)
                            {
                                if (locationsettingdata.Rows.Count != 0)
                                {
                                    carrierid = locationsettingdata.Rows[0]["CARRIERID"].ToString()!;
                                    SAA_Database.LogMessage($"【{station_naem}】【監控上報】【新增查詢ID】卡匣ID:{carrierid}");
                                    SAA_Database.SaaSql?.UpdScLiftCarrierInfoCarrierid(LiftCarrierInfo, carrierid);
                                }
                            }
                            SAA_Database.SaaSql?.UpdScLocationSettingCarrierid(locationsetting);
                            if (SaaEquipmentgroup.DataCarrierId.Contains(SAA_Database.EquipmentCommon.ReaderError))
                            {
                                SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                {
                                    TASKDATETIME = SAA_Database.ReadTime(),
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAEM = station_naem,
                                    CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                    REPORE_DATATRACK = SaaEquipmentgroup.DataTrack,
                                    REPORE_DATAREMOTE = SaaEquipmentgroup.DataRemote,
                                    REPORE_DATALOCAL = SaaEquipmentgroup.DataLocal,
                                };
                                SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                                SAA_Database.LogMessage($"【{station_naem}】【監控上報】新增ERROR上報事件 MAG ({SaaEquipmentgroup.DataCarrierId}) ", SAA_DatabaseEnum.LogType.Error);

                                var data = SAA_Database.SaaSql?.GetScLiftCarrierInfoReject(station_naem, SaaEquipmentgroup.DataCarrierId);
                                if (data?.Rows.Count == 0)
                                {
                                    SaaScLiftCarrierInfoReject CarrierInfoReject = new SaaScLiftCarrierInfoReject
                                    {
                                        TASKDATETIME = SAA_Database.ReadTime(),
                                        SETNO = SAA_Database.configattributes.SaaSetNo,
                                        MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                                        CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                        STATION_NAME = station_naem,
                                    };
                                    SAA_Database.SaaSql?.SetScLiftCarrierInfoReject(CarrierInfoReject);
                                    SAA_Database.LogMessage($"【{station_naem}】【監控上報】新增退盒資料表SC_LIFT_CARRIER_INFO_REJECT MAG ({SaaEquipmentgroup.DataCarrierId}) ", SAA_DatabaseEnum.LogType.Error);
                                }
                            }

                            if (SaaEquipmentgroup.DataLocal == "CRANE")
                            {
                                int ReadPlc1 = SaaEquipmentPlc!.ReadInt(SAA_Database.configattributes.SaaOffsetWord, "533");
                                SAA_Database.LogMessage($"【{station_naem}】【監控上報】讀取空框檢知:{ReadPlc1} MAG ({SaaEquipmentgroup.DataCarrierId}) ");
                                if (ReadPlc1 == 99 || ReadPlc1 == 98)
                                {
                                    var liftcarrierdata = SAA_Database.SaaSql?.GetLiftCarrierInfoCarrier(LiftCarrierInfo);
                                    if (liftcarrierdata != null)
                                    {
                                        if (liftcarrierdata.Rows.Count != 0)
                                        {
                                            SAA_Database.SaaSql?.UpdScLiftCarrierInfoCarrieridReadPlc(LiftCarrierInfo, ReadPlc1.ToString());
                                            SAA_Database.LogMessage($"【{station_naem}】【監控上報】更新空框檢知:{ReadPlc1} MAG ({SaaEquipmentgroup.DataCarrierId}) ");
                                        }
                                        else
                                        {
                                            LiftCarrierInfo.READPLC = ReadPlc1.ToString();
                                            SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                                            SAA_Database.SaaSql?.SetScLiftCarrierInfoReadPlc(LiftCarrierInfo);
                                            SAA_Database.LogMessage($"【{station_naem}】【監控上報】新增空框檢知:{ReadPlc1} MAG ({SaaEquipmentgroup.DataCarrierId}) ");
                                        }
                                    }

                                    var data = SAA_Database.SaaSql?.GetScLiftCarrierInfoReject(station_naem, SaaEquipmentgroup.DataCarrierId);
                                    if (data?.Rows.Count == 0)
                                    {
                                        SaaScLiftCarrierInfoReject CarrierInfoReject = new SaaScLiftCarrierInfoReject
                                        {
                                            TASKDATETIME = SAA_Database.ReadTime(),
                                            SETNO = SAA_Database.configattributes.SaaSetNo,
                                            MODEL_NAME = SAA_Database.configattributes.SaaModelName,
                                            CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                            STATION_NAME = station_naem,
                                        };
                                        SAA_Database.SaaSql?.SetScLiftCarrierInfoReject(CarrierInfoReject);
                                    }

                                    if (ReadPlc1 == 98)
                                    {
                                        var equirementmaterial = SAA_Database.SaaSql?.GetScTransportrEquirementMaterialCarrierId(SaaEquipmentgroup.DataCarrierId);
                                        if (equirementmaterial?.Rows.Count != 0)
                                        {
                                            SAA_Database.SaaSql?.DelScTransportrEquirementMaterial(SaaEquipmentgroup.DataCarrierId);
                                            SAA_Database.LogMessage($"【{station_naem}】【監控上報】空框檢知:{ReadPlc1} MAG ({SaaEquipmentgroup.DataCarrierId}刪除叫車任務，資料表:SC_TRANSPORTR_EQUIREMENT_MATERIAL) ");
                                        }
                                    }
                                    else if (ReadPlc1 == 99)
                                    {
                                        var equirementmaterial = SAA_Database.SaaSql?.GetScTransportrEquirementMaterialCarrierId(SaaEquipmentgroup.DataCarrierId);
                                        if (equirementmaterial?.Rows.Count != 0)
                                        {
                                            SAA_Database.SaaSql?.DelScTransportrequirement(SaaEquipmentgroup.DataCarrierId);
                                            SAA_Database.LogMessage($"【{station_naem}】【監控上報】空框檢知:{ReadPlc1} MAG ({SaaEquipmentgroup.DataCarrierId}刪除叫車任務，資料表:SC_TRANSPORTR_EQUIREMENT) ");
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        if (SaaEquipmentgroup.DataTrack == SAA_Database.EquipmentCommon.Ask)
                        {
                            SaaScPurchase purchasedata = new SaaScPurchase
                            {
                                TASKDATETIME = SAA_Database.ReadTime(),
                                SETNO = setno,
                                MODEL_NAME = model_name,
                                STATION_NAME = station_naem,
                                REPLY = SaaEquipmentgroup.DataReply,
                                LOCAL = SaaEquipmentgroup.DataLocal,
                                CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                COMMANDID = SaaEquipmentgroup.DataTrack,
                            };
                            var purchase = SAA_Database.SaaSql?.GetScPurchase(purchasedata);
                            if (purchase?.Rows.Count == 0)
                            {
                                SAA_Database.SaaSql?.SetScPurchase(purchasedata);
                                SAA_Database.LogMessage($"【{station_naem}】【監控上報】【新增詢問】新增卡匣詢問需求:卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                            }
                            else
                            {
                                SAA_Database.LogMessage($"【{station_naem}】【監控上報】【新增詢問】已有卡匣詢問資訊不予新增卡匣ID:{SaaEquipmentgroup.DataCarrierId}");
                            }

                            var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(station_naem, SaaEquipmentgroup.DataCarrierId);
                            if (equipmentcarrierinfo?.Rows.Count == 0)
                            {
                                SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                {
                                    TASKDATETIME = SAA_Database.ReadTime(),
                                    SETNO = setno,
                                    MODEL_NAME = model_name,
                                    STATION_NAEM = station_naem,
                                    CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                    REPORE_DATATRACK = SaaEquipmentgroup.DataTrack,
                                    REPORE_DATAREMOTE = SaaEquipmentgroup.DataRemote,
                                    REPORE_DATALOCAL = SaaEquipmentgroup.DataLocal,
                                };
                                if (SaaEquipmentgroup.DataLocal != "PGV-OUT" && SaaEquipmentgroup.DataLocal != "Stage-In")
                                    SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                            }
                            else
                            {
                                string carrierflag = !string.IsNullOrEmpty(equipmentcarrierinfo?.Rows[0]["CARRIERFLAG"].ToString()) ? equipmentcarrierinfo?.Rows[0]["CARRIERFLAG"].ToString()! : string.Empty;
                                if (string.IsNullOrEmpty(carrierflag))
                                {
                                    SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                    {
                                        TASKDATETIME = SAA_Database.ReadTime(),
                                        SETNO = setno,
                                        MODEL_NAME = model_name,
                                        STATION_NAEM = station_naem,
                                        CARRIERID = SaaEquipmentgroup.DataCarrierId,
                                        REPORE_DATATRACK = SaaEquipmentgroup.DataTrack,
                                        REPORE_DATAREMOTE = SaaEquipmentgroup.DataRemote,
                                        REPORE_DATALOCAL = SaaEquipmentgroup.DataLocal,
                                    };
                                    if (SaaEquipmentgroup.DataLocal != "PGV-OUT" && SaaEquipmentgroup.DataLocal != "Stage-In")
                                        SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                                }
                                else
                                {
                                    SAA_Database.SaaSql?.UpdScScPurchase(setno, station_naem, SaaEquipmentgroup.DataCarrierId, carrierflag);
                                    SAA_Database.LogMessage($"【{station_naem}】【監控上報】【新增詢問】已詢問卡匣詢需求不詢問ASE:卡匣ID:{SaaEquipmentgroup.DataCarrierId}，結果:{carrierflag}(Y:進卡匣，N:退REJECT)");
                                }
                            }
                            Reply = true;
                        }
                    }
                }

                #region [===監控答覆===]
                if (Reply)
                {
                    if (SaaEquipmentgroup != null)
                    {
                        if (EquipmentOffset.PlcRequest[0] == 1)
                        {
                            var purchasedata = SAA_Database.SaaSql?.GetScPurchase(station_naem);
                            if (purchasedata!.Rows.Count != 0)
                            {
                                string carrierid = purchasedata.Rows[0]["CARRIERID"].ToString()!;
                                string commandid = SAA_Database.EquipmentCommon.Reply;
                                string reply = purchasedata.Rows[0]["REPLY"].ToString()!;
                                string local = purchasedata.Rows[0]["LOCAL"].ToString()!;
                                var data = local == "PGV-OUT" ? SAA_Database.SaaSql?.GetPurchasePgvOut(carrierid) : SAA_Database.SaaSql?.GetPurchase(carrierid);
                                SAA_Database.LogMessage($"【{station_naem}】【監控資料】詢問時的位置 :{local}");
                                if (data!.Rows.Count != 0)
                                {
                                    string replyresult = string.Empty;
                                    int writereply = 0;
                                    if (local == "PGV-OUT")
                                    {
                                        var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(station_naem, SaaEquipmentgroup.DataCarrierId);
                                        if (equipmentcarrierinfo?.Rows.Count != 0)
                                        {
                                            SaaEquipmentCarrierInfo carrierinfo = new SaaEquipmentCarrierInfo
                                            {
                                                REJECT_CODE = equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.REJECT_CODE.ToString()].ToString()!,
                                                REJECT_MESSAGE = equipmentcarrierinfo?.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.REJECT_MESSAGE.ToString()].ToString()!,
                                            };
                                            var rejectlistplcdata = SAA_Database.SaaSql?.GetScRejectListPlc(carrierinfo.REJECT_CODE);
                                            if (rejectlistplcdata != null)
                                            {
                                                if (rejectlistplcdata.Rows.Count != 0)
                                                {
                                                    writereply = int.Parse(rejectlistplcdata.Rows[0][SAA_DatabaseEnum.SC_REJECT_LIST_PLC.PLC_REJECT_CODE.ToString()].ToString()!);
                                                    SAA_Database.LogMessage($"【{station_naem}】【監控答覆】PLC 退盒編號:{writereply}，卡匣ID:{carrierid}，退盒原因:{rejectlistplcdata.Rows[0][SAA_DatabaseEnum.SC_REJECT_LIST_PLC.REMOTE_REJECT_MSG.ToString()]!}");
                                                }
                                            }
                                        }
                                        if (writereply == 0)
                                        {
                                            writereply = 1;
                                            SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【查無此資料】PLC 退盒編號:{writereply}，卡匣ID:{carrierid}");
                                        }
                                        replyresult = SAA_DatabaseEnum.SendFlag.Y.ToString();
                                    }
                                    else if (local == "Stage-In")
                                    {
                                        var device = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                                        string simulation = device?.Rows.Count != 0 ? device!.Rows[0]["SIMULATION"].ToString()! : SAA_DatabaseEnum.SendFlag.Y.ToString();
                                        SAA_Database.LogMessage($"【{station_naem}】【監控資料】模擬回覆模式 SIMULATION :{simulation}");
                                        if (simulation == SAA_DatabaseEnum.SendFlag.Y.ToString())//Y:上報ASE N:模擬
                                        {
                                            var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(station_naem, SaaEquipmentgroup.DataCarrierId);
                                            if (equipmentcarrierinfo != null)
                                            {
                                                if (equipmentcarrierinfo.Rows.Count != 0)
                                                {
                                                    string destinationtype = equipmentcarrierinfo.Rows[0][SAA_DatabaseEnum.SC_EQUIPMENT_CARRIER_INFO.DESTINATIONTYPE.ToString()].ToString()!;
                                                    writereply = destinationtype == SAA_DatabaseEnum.DestinationType.EQP.ToString() ? 1 : destinationtype.Contains(SAA_DatabaseEnum.DestinationType.Buffer.ToString()) ? 2 : 4;
                                                    replyresult = destinationtype == SAA_DatabaseEnum.DestinationType.EQP.ToString() || destinationtype.Contains(SAA_DatabaseEnum.DestinationType.Buffer.ToString()) ? SAA_DatabaseEnum.SendFlag.Y.ToString() : SAA_DatabaseEnum.SendFlag.N.ToString();
                                                    SAA_Database.LogMessage($"【{station_naem}】【監控資料】卡匣ID:{carrierid}，DESTINATIONTYPE:{destinationtype}，讀取狀態:{writereply}");
                                                }
                                                else
                                                {
                                                    writereply = 4;
                                                    replyresult = SAA_DatabaseEnum.SendFlag.N.ToString();
                                                    SAA_Database.LogMessage($"【{station_naem}】【監控資料】卡匣ID:{carrierid}，查無此卡匣資訊，讀取狀態:{writereply}");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int simulationreplyresult = int.Parse(device!.Rows[0]["SIMULATION_REPLYRESULT"].ToString()!);//要更新欄位
                                            replyresult = simulationreplyresult == 1 || simulationreplyresult == 2 ? SAA_DatabaseEnum.SendFlag.Y.ToString() : SAA_DatabaseEnum.SendFlag.N.ToString();
                                            writereply = int.Parse(device!.Rows[0]["SIMULATION_REPLYRESULT"].ToString()!);//要更新欄位
                                            SAA_Database.LogMessage($"【{station_naem}】【監控資料】讀取 ReplyResult :{replyresult}，答覆資訊 :{writereply}");
                                        }
                                    }
                                    else if (local == "Stage-Out")
                                    {
                                        var device = SAA_Database.SaaSql?.GetScDevice(setno, model_name, station_naem);
                                        string simulation = device?.Rows.Count != 0 ? device!.Rows[0]["SIMULATION"].ToString()! : SAA_DatabaseEnum.SendFlag.Y.ToString();
                                        SAA_Database.LogMessage($"【{station_naem}】【監控資料】模擬回覆模式 SIMULATION :{simulation}");
                                        if (simulation == SAA_DatabaseEnum.SendFlag.Y.ToString())//Y:上報ASE N:模擬
                                        {
                                            replyresult = data.Rows[0]["REPLYRESULT"].ToString()!;
                                            writereply = replyresult == SAA_DatabaseEnum.SaaSendReply.Y.ToString() ? 1 : 4;
                                        }
                                        else
                                        {
                                            int simulationreplyresult = int.Parse(device!.Rows[0]["SIMULATION_REPLYRESULT"].ToString()!);//要更新欄位
                                            replyresult = simulationreplyresult == 1 || simulationreplyresult == 2 ? SAA_DatabaseEnum.SendFlag.Y.ToString() : SAA_DatabaseEnum.SendFlag.N.ToString();
                                            writereply = int.Parse(device!.Rows[0]["SIMULATION_REPLYRESULT"].ToString()!);//要更新欄位
                                            SAA_Database.LogMessage($"【{station_naem}】【監控資料】讀取 ReplyResult :{replyresult}，答覆資訊 :{writereply}");
                                        }
                                    }
                                    else
                                    {
                                        replyresult = data.Rows[0]["REPLYRESULT"].ToString()!;
                                        writereply = replyresult == SAA_DatabaseEnum.SaaSendReply.Y.ToString() ? 1 : 4;
                                    }
                                    if (replyresult != string.Empty)
                                    {
                                        if (local != "PGV-OUT")
                                            SAA_Database.LogMessage($"【{station_naem}】【監控答覆】卡匣ID:{carrierid}接收到答覆結果:{replyresult}");

                                        SaaEquipmentPlc?.WriteInt(SAA_Database.saaequipmentbitoffset.SaaDB_DataCountAck, SAA_Database.saaequipmentbitoffset.SaaValue_DataCountAck, 1);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.saaequipmentbitoffset.SaaDB_DataCountAck}{SAA_Database.saaequipmentbitoffset.SaaValue_DataCountAck}】PC寫入資料組數1");

                                        SaaEquipmentPlc?.WriteInt(SAA_Database.configattributes.SaaOffsetWord, SaaEquipmentgroup.DataTrackAck, short.Parse(commandid));
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.configattributes.SaaOffsetWord}{SaaEquipmentgroup.DataTrackAck}】PC寫入詢問類型:{commandid}");

                                        SaaEquipmentPlc?.WriteInt(SAA_Database.configattributes.SaaOffsetWord, SaaEquipmentgroup.DataReplyAck, short.Parse(reply));
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.configattributes.SaaOffsetWord}{SaaEquipmentgroup.DataReplyAck}】PC寫入答覆類型:{reply}");

                                        SaaEquipmentPlc?.WriteStringArray(SAA_Database.configattributes.SaaOffsetWord, SaaEquipmentgroup.DataLocalAck, local);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控上報】【{SAA_Database.configattributes.SaaOffsetWord}{SaaEquipmentgroup.DataLocalAck}】PC寫入位置-本地:{local}");

                                        SaaEquipmentPlc?.WriteStringArray(SAA_Database.configattributes.SaaOffsetWord, SaaEquipmentgroup.DataCarrrierIdAck, carrierid);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.configattributes.SaaOffsetWord}{SaaEquipmentgroup.DataCarrrierIdAck}】PC寫入卡匣ID:{carrierid}");

                                        SaaEquipmentPlc?.WriteInt(SAA_Database.configattributes.SaaOffsetWord, SaaEquipmentgroup.DataReplyInfo, writereply);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.configattributes.SaaOffsetWord}{SaaEquipmentgroup.DataReplyInfo}】PC寫入答覆資訊:{writereply}");

                                        SaaEquipmentPlc?.WriteBool(SAA_Database.saaequipmentbitoffset.SaaDB_RequestAllAck, SAA_Database.saaequipmentbitoffset.SaaValue_RequestAllAck, true);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.saaequipmentbitoffset.SaaDB_RequestAllAck}{SAA_Database.saaequipmentbitoffset.SaaValue_RequestAllAck}】寫入PC訊號為True");
                                        while (true)
                                        {
                                            Thread.Sleep(50);
                                            if (EquipmentOffset.PlcRequest[4] == 0) continue;
                                            SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.saaequipmentbitoffset.SaaDB_RequestConfirm}{SAA_Database.saaequipmentbitoffset.SaaValue_RequestConfirm}】接收到PLC訊號為{EquipmentOffset.PlcRequest[2]}");
                                            SaaEquipmentPlc?.WriteBool(SAA_Database.saaequipmentbitoffset.SaaDB_RequestAllAck, SAA_Database.saaequipmentbitoffset.SaaValue_RequestAllAck, false);
                                            SAA_Database.LogMessage($"【{station_naem}】【監控答覆】【{SAA_Database.saaequipmentbitoffset.SaaDB_RequestAllAck}{SAA_Database.saaequipmentbitoffset.SaaValue_RequestAllAck}】PC寫入訊號為False");
                                            SAA_Database.LogMessage($"【{station_naem}】【監控答覆】交握完成");
                                            break;
                                        }

                                        SAA_Database.SaaSql?.DelScPurchase(station_naem, carrierid);
                                        SAA_Database.LogMessage($"【{station_naem}】【監控答覆】刪除答覆任務，卡匣ID:{carrierid}");
                                        Reply = false;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                if (EquipmentOffset.PlcRequest[3] == 1)
                {
                    int[] plcmodel = { 1, 0 };
                    SaaEquipmentPlc?.WriteIntArray(SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck, SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck, plcmodel);
                    SAA_Database.LogMessage($"【{station_naem}】【模式切換】【{SAA_Database.saaequipmentbitoffset.SaaDB_SynchronizeAck}{SAA_Database.saaequipmentbitoffset.SaaValue_SynchronizeAck}】資料同步模式停止");
                    SAA_Database.LogMessage($"【{station_naem}】【模式切換】【{SAA_Database.saaequipmentbitoffset.SaaDB_TrackAck}{SAA_Database.saaequipmentbitoffset.SaaValue_TrackAck}】物料追蹤模式開始");
                    var purchasedata = SAA_Database.SaaSql?.GetScPurchaseTask(station_naem);
                    if (purchasedata!.Rows.Count != 0)
                    {
                        Reply = true;
                    }
                }
            }
        }
        #endregion

        public async void CommandStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        EquipmentCommandStart(EquipmentPlcOffset);
                        Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void EquipmentCommandStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                if (EquipmentOffset.PlcRequest != null)
                {
                    if (EquipmentOffset.PlcRequest[0] == 1)
                    {
                        int[] EquipmentSiteOffsetDkIn = SaaEquipmentPlc?.ReadIntArray("B", "2D0", 8)!;
                        int[] EquipmentSiteOffsetPgvOut = SaaEquipmentPlc?.ReadIntArray("B", "2C0", 8)!;
                        if (EquipmentSiteOffsetDkIn[0] == 0 && EquipmentSiteOffsetPgvOut[0] == 0)
                        {
                            var commandtaskdata = SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                            if (commandtaskdata!.Rows.Count != 0)
                            {
                                SaaScCommandTask CommandTask = new SaaScCommandTask()
                                {
                                    TASKDATETIME = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.TASKDATETIME.ToString()].ToString()!,
                                    SETNO = int.Parse(commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.SETNO.ToString()].ToString()!),
                                    MODEL_NAME = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.MODEL_NAME.ToString()].ToString()!,
                                    STATION_NAME = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.STATION_NAME.ToString()].ToString()!,
                                    COMMANDID = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.COMMANDID.ToString()].ToString()!,
                                    CARRIERID = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.CARRIERID.ToString()].ToString()!,
                                    LOCATIONTAKE = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.LOCATIONTAKE.ToString()].ToString()!,
                                    LOCATIONPUT = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.LOCATIONPUT.ToString()].ToString()!,
                                    RESULT = commandtaskdata.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.RESULT.ToString()].ToString()!
                                };
                                List<int> carrieridlist = ReadStringToInt(CommandTask.CARRIERID);
                                int[] carrierid = new int[10];
                                for (int i = 0; i < carrierid.Length; i++)
                                {
                                    carrierid[i] = carrieridlist.Count > i ? carrieridlist[i] : 0;
                                }
                                if (CommandTask.LOCATIONPUT == "PGV-OUT")
                                {
                                    SaaEquipmentPlc?.WriteIntArray(SAA_Database.configattributes.SaaOffsetWord, "2C0", carrierid);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至PGV-OUT】PC寫入卡匣ID:{CommandTask.CARRIERID}");
                                    SaaEquipmentPlc?.WriteBool("B", "2C0", true);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至PGV-OUT】PC已傳送命令給PLC，W_FICS為 True");

                                    DateTime dtStart = DateTime.Now;
                                    while (true)
                                    {
                                        EquipmentOffset.PlcCommandOffset = SaaEquipmentPlc?.ReadIntArray("B", "6B0", 45)!;
                                        if (EquipmentOffset.PlcCommandOffset.Length >= 18)
                                        {
                                            TimeSpan waitTime = new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks);
                                            if (EquipmentOffset.PlcCommandOffset[16] == 1)
                                            {
                                                if (waitTime.TotalSeconds >= 1)
                                                {
                                                    CommandTask.RESULT = SAA_DatabaseEnum.SendFlag.Y.ToString();
                                                    SAA_Database.SaaSql?.UpdScCommandTask(CommandTask);
                                                    break;
                                                }
                                            }
                                            if (EquipmentOffset.PlcCommandOffset[17] == 1)
                                            {
                                                if (waitTime.TotalSeconds >= 3)
                                                {
                                                    DelCommandTask(commandtaskdata, EquipmentPlcOffset.STATION_NAME, "PGV-OUT");
                                                    SaaEquipmentPlc?.WriteBool("B", "2C0", false);
                                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至PGV-OUT】PC已傳送命令給PLC，W_FICS為 False");
                                                    break;
                                                }
                                            }

                                            if (waitTime.TotalSeconds > 30)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (CommandTask.LOCATIONPUT == "DK-IN")
                                {
                                    SaaEquipmentPlc?.WriteIntArray(SAA_Database.configattributes.SaaOffsetWord, "2D0", carrierid);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至DK-IN】PC寫入卡匣ID:{CommandTask.CARRIERID}");
                                    SaaEquipmentPlc?.WriteBool("B", "2D0", true);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至DK-IN】PC已傳送命令給PLC，W_FICS為 True");

                                    DateTime dtStart = DateTime.Now;
                                    while (true)
                                    {
                                        EquipmentOffset.PlcCommandOffset = SaaEquipmentPlc?.ReadIntArray("B", "6D0", 45)!;
                                        if (EquipmentOffset.PlcCommandOffset.Length >= 5)
                                        {
                                            TimeSpan waitTime = new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks);
                                            if (EquipmentOffset.PlcCommandOffset[0] == 1)
                                            {
                                                if (waitTime.TotalSeconds >= 3)
                                                {
                                                    CommandTask.RESULT = SAA_DatabaseEnum.SendFlag.Y.ToString();
                                                    SAA_Database.SaaSql?.UpdScCommandTask(CommandTask);
                                                    break;
                                                }
                                            }
                                            if (EquipmentOffset.PlcCommandOffset[1] == 1)
                                            {
                                                if (waitTime.TotalSeconds >= 3)
                                                {
                                                    DelCommandTask(commandtaskdata, EquipmentPlcOffset.STATION_NAME, "DK-IN");
                                                    SaaEquipmentPlc?.WriteBool("B", "2D0", false);
                                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至DK-IN】PC已傳送命令給PLC，W_FICS為 False");
                                                    break;
                                                }
                                            }

                                            if (waitTime.TotalSeconds > 30)
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        EquipmentOffset.PlcCommandOffset = SaaEquipmentPlc?.ReadIntArray("B", "6B0", 45)!;
                        if (EquipmentOffset.PlcCommandOffset.Length >= 18)
                        {
                            if (EquipmentOffset.PlcCommandOffset[17] == 1)
                            {
                                SaaEquipmentPlc?.WriteBool("B", "2C0", false);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至PGV-OUT】PC已傳送命令給PLC，W_FICS為 False");
                            }
                        }
                        EquipmentOffset.PlcCommandOffset = SaaEquipmentPlc?.ReadIntArray("B", "6D0", 45)!;
                        if (EquipmentOffset.PlcCommandOffset.Length >= 18)
                        {
                            if (EquipmentOffset.PlcCommandOffset[1] == 1)
                            {
                                SaaEquipmentPlc?.WriteBool("B", "2D0", false);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【手臂搬運】【卡匣傳送至DK-IN】PC已傳送命令給PLC，W_FICS為 False");
                            }
                        }
                        SaaScLiftCarrierInfo scliftcarrierinfo = new SaaScLiftCarrierInfo
                        {
                            SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                            MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                            STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                            CARRIERTYPE = "2",//1:實盒 2:空盒
                        };
                        var liftcarrierinfodata = SAA_Database.SaaSql?.GetLiftCarrierInfoEmpty(scliftcarrierinfo);
                        if (liftcarrierinfodata != null)
                        {
                            if (liftcarrierinfodata.Rows.Count != 0)
                            {
                                SaaScTransportrEquirement ScTransportrEquirement = new SaaScTransportrEquirement
                                {
                                    SETNO = EquipmentPlcOffset.SETNO,
                                    MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                    STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                    REPORT_STATION = EquipmentPlcOffset.STATION_NAME,
                                    REPORT_TIME = SAA_Database.ReadTeidTime(),
                                    CARRIERID = liftcarrierinfodata.Rows[0]["CARRIERID"].ToString()!,
                                    REQUIREMENT_TYPE = scliftcarrierinfo.CARRIERTYPE,
                                    BEGIN_STATION = EquipmentPlcOffset.STATION_NAME,
                                    END_STATION = "NA"
                                };
                                ScTransportrEquirement.REPORTID = $"{ScTransportrEquirement.REPORT_STATION}_{ScTransportrEquirement.REPORT_TIME}";
                                var devicemodel = SAA_Database.SaaSql?.GetScDeviceModelName(int.Parse(EquipmentPlcOffset.SETNO), EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                if (devicemodel?.Rows.Count != 0)
                                {
                                    if (devicemodel?.Rows[0]["DEVICETYPE"].ToString() == "1")
                                    {

                                        SaaScLocationSetting SaaSettingCarrierId = new SaaScLocationSetting
                                        {
                                            SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                            MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                            STATIOM_NAME = EquipmentPlcOffset.STATION_NAME,
                                            CARRIERID = liftcarrierinfodata?.Rows[0]["CARRIERID"].ToString()!,
                                        };
                                        var settingcarrieriddata = SAA_Database.SaaSql?.GetScLocationSettingCarrierId(SaaSettingCarrierId);
                                        if (settingcarrieriddata?.Rows.Count != 0)
                                        {
                                            SAA_Database.SaaSql?.SetScTransportrEquirement(ScTransportrEquirement);
                                            SAA_Database.LogMessage($"【{SaaSettingCarrierId.STATIOM_NAME}】新增叫車需求:SC_TRANSPORTR_EQUIREMENT 卡匣ID:{SaaSettingCarrierId.CARRIERID}");
                                            scliftcarrierinfo.CALL_SHUTTLE = SAA_DatabaseEnum.SendFlag.Y.ToString();
                                            SAA_Database.SaaSql?.UpdScLiftCarrierInfoCallShuttle(scliftcarrierinfo, ScTransportrEquirement.CARRIERID);
                                        }
                                    }
                                }
                            }
                        }

                        // B450 交握開始
                        //SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】開始讀取 JumpDie 點位");
                        int[] EquipmentJumpDie = SaaEquipmentPlc?.ReadIntArray("B", "450", 2)!;
                        if (EquipmentJumpDie[0] == 1)
                        {
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B450】JumpDie 開始點位為 {EquipmentJumpDie[0]}");
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B450】點位為 true，執行 JumpDie 動作");
                            SaaEquipmentPlc?.WriteBool("B", "50", true);
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B50】 點位寫入 true");
                            while (true)
                            {
                                EquipmentJumpDie = SaaEquipmentPlc?.ReadIntArray("B", "450", 2)!;
                                if (EquipmentJumpDie[0] == 0)
                                {
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B450】JumpDie 開始點位變更為 {EquipmentJumpDie[0]}");
                                    SaaEquipmentPlc?.WriteBool("B", "50", false);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B50】 點位寫入 false");
                                    break;
                                }
                                Thread.Sleep(100);
                            }

                            var LocationSetting = SAA_Database.SaaSql?.GetScLocationSetting(int.Parse(EquipmentPlcOffset.SETNO), EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME, "CRANE")!;
                            SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                            {
                                TASKDATETIME = SAA_Database.ReadTime(),
                                SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                STATION_NAEM = EquipmentPlcOffset.STATION_NAME,
                                CARRIERID = !string.IsNullOrEmpty(LocationSetting?.Rows[0]["CARRIERID"].ToString()) ? LocationSetting?.Rows[0]["CARRIERID"].ToString()! : string.Empty,
                                REPORE_DATATRACK = "305",
                            };
                            SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                            SAA_Database.LogMessage($"【{EquipmentReport.STATION_NAEM}】Insert into SC_EQUIPMENT_REPORT 資料，內容 : STATION_NAEM - {EquipmentReport.STATION_NAEM}，CARRIERID - {EquipmentReport.CARRIERID}，REPORE_DATATRACK - {EquipmentReport.REPORE_DATATRACK}");
                        }

                        // B451 交握結束
                        if (EquipmentJumpDie[1] == 1)
                        {
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B451】JumpDie 結束點位為 {EquipmentJumpDie[1]}");
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B451】點位為 true，結束 JumpDie 動作");

                            SaaEquipmentPlc?.WriteBool("B", "51", true);
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B51】 點位寫入 true");
                            while (true)
                            {
                                EquipmentJumpDie = SaaEquipmentPlc?.ReadIntArray("B", "450", 2)!;
                                Thread.Sleep(100);
                                if (EquipmentJumpDie[1] == 0)
                                {
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B451】JumpDie 結束點位變更為 {EquipmentJumpDie[1]}");
                                    SaaEquipmentPlc?.WriteBool("B", "51", false);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【B51】 點位寫入 false");
                                    break;
                                }
                            }

                            var LocationSetting = SAA_Database.SaaSql?.GetScLocationSetting(int.Parse(EquipmentPlcOffset.SETNO), EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME, "CRANE")!;
                            SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                            {
                                TASKDATETIME = SAA_Database.ReadTime(),
                                SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                STATION_NAEM = EquipmentPlcOffset.STATION_NAME,
                                CARRIERID = !string.IsNullOrEmpty(LocationSetting?.Rows[0]["CARRIERID"].ToString()) ? LocationSetting?.Rows[0]["CARRIERID"].ToString()! : string.Empty,
                                REPORE_DATATRACK = "315",
                            };
                            SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);
                            SAA_Database.LogMessage($"【{EquipmentReport.STATION_NAEM}】Insert into SC_EQUIPMENT_REPORT 資料，內容 : STATION_NAEM - {EquipmentReport.STATION_NAEM}，CARRIERID - {EquipmentReport.CARRIERID}，REPORE_DATATRACK - {EquipmentReport.REPORE_DATATRACK}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public async void CommandEquipmentSiteStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        CommandEquipmentSite(EquipmentPlcOffset);
                        Thread.Sleep(1000);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        #region [===下達命令至製程機或退盒===]
        public void CommandEquipmentSite(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                int[] EquipmentSiteOffset = SaaEquipmentPlc?.ReadIntArray("B", "6C0", 32)!;
                if (EquipmentSiteOffset != null)
                {
                    if (EquipmentSiteOffset.Length > 20)
                    {
                        //進製程
                        if (EquipmentSiteOffset?[19] == 1)
                        {
                            if (plceqpoffset != EquipmentSiteOffset?[19])
                            {
                                if (EquipmentSiteOffset?[19] != null)
                                    plceqpoffset = EquipmentSiteOffset[19]!;
                            }
                            var sitdata = SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.STATION_NAME);
                            if (sitdata != null)
                            {
                                if (sitdata.Rows.Count == 0)
                                {
                                    var devicedata = SAA_Database.SaaSql?.GetScDevice(int.Parse(EquipmentPlcOffset.SETNO), EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                    if (devicedata != null)
                                    {
                                        if (devicedata?.Rows.Count != 0)
                                        {
                                            string devicetype = devicedata?.Rows[0]["DEVICETYPE"].ToString()!;
                                            if (devicetype == "2")//1:LD 2:ULD
                                            {
                                                //ULD製程機要空盒卡匣
                                                var ScCommandTask = SAA_Database.SaaSql?.GetScCommandTaskResult(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                                if (ScCommandTask?.Rows.Count == 0)
                                                {
                                                    var liftcarrierinfoempty = SAA_Database.SaaSql?.GetScLiftCarrierInfoEmpty(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                                    if (liftcarrierinfoempty != null)
                                                    {
                                                        if (liftcarrierinfoempty?.Rows.Count != 0)
                                                        {
                                                            foreach (DataRow dr in liftcarrierinfoempty!.Rows)
                                                            {
                                                                SaaScLocationSetting SaaSettingCarrierId = new SaaScLocationSetting
                                                                {
                                                                    SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                    MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                    STATIOM_NAME = EquipmentPlcOffset.STATION_NAME,
                                                                    CARRIERID = dr["CARRIERID"].ToString()!,
                                                                };
                                                                var settingcarrieriddata = SAA_Database.SaaSql?.GetScLocationSettingCarrierId(SaaSettingCarrierId);
                                                                if (settingcarrieriddata != null)
                                                                {
                                                                    if (settingcarrieriddata.Rows.Count != 0)
                                                                    {
                                                                        SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                                                        {
                                                                            TASKDATETIME = SAA_Database.ReadTime(),
                                                                            SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                            MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                            STATION_NAEM = EquipmentPlcOffset.STATION_NAME,
                                                                            CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                                            REPORE_DATATRACK = "105",
                                                                            REPORE_DATAREMOTE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,
                                                                            REPORE_DATALOCAL = "DK-IN",
                                                                        };
                                                                        SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);


                                                                        SaaScCommandTask CommandTask = new SaaScCommandTask
                                                                        {
                                                                            TASKDATETIME = SAA_Database.ReadTime(),
                                                                            SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                            MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                            STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                                                            CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                                            COMMANDID = $"{DateTime.Now:fff}",
                                                                            LOCATIONPUT = "DK-IN",
                                                                            LOCATIONTAKE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,
                                                                        };
                                                                        if (SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.STATION_NAME).Rows.Count == 0)
                                                                            SAA_Database.SaaSql?.SetScCommandTask(CommandTask);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //LD製程機要實盒卡匣
                                                var ScCommandTask = SAA_Database.SaaSql?.GetScCommandTaskResult(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                                if (ScCommandTask?.Rows.Count == 0)
                                                {
                                                    var liftcarrierinfoempty = SAA_Database.SaaSql?.GetScLiftCarrierInfoMaterial(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                                    if (liftcarrierinfoempty?.Rows.Count != 0)
                                                    {
                                                        if (liftcarrierinfoempty != null)
                                                        {
                                                            foreach (DataRow dr in liftcarrierinfoempty.Rows)
                                                            {
                                                                SaaScLocationSetting SaaSettingCarrierId = new SaaScLocationSetting
                                                                {
                                                                    SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                    MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                    STATIOM_NAME = EquipmentPlcOffset.STATION_NAME,
                                                                    CARRIERID = dr["CARRIERID"].ToString()!,
                                                                };
                                                                var settingcarrieriddata = SAA_Database.SaaSql?.GetScLocationSettingCarrierId(SaaSettingCarrierId);
                                                                if (settingcarrieriddata?.Rows.Count != 0)
                                                                {
                                                                    SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                                                    {
                                                                        TASKDATETIME = SAA_Database.ReadTime(),
                                                                        SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                        MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                        STATION_NAEM = EquipmentPlcOffset.STATION_NAME,
                                                                        CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                                        REPORE_DATATRACK = "105",
                                                                        REPORE_DATAREMOTE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,
                                                                        REPORE_DATALOCAL = "DK-IN",
                                                                    };
                                                                    SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);

                                                                    SaaScCommandTask CommandTask = new SaaScCommandTask
                                                                    {
                                                                        TASKDATETIME = SAA_Database.ReadTime(),
                                                                        SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                                        MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                                        STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                                                        CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                                        COMMANDID = $"{DateTime.Now:fff}",
                                                                        LOCATIONPUT = "DK-IN",
                                                                        LOCATIONTAKE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,

                                                                    };
                                                                    if (SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.STATION_NAME).Rows.Count == 0)
                                                                    {
                                                                        SAA_Database.SaaSql?.SetScCommandTask(CommandTask);
                                                                        SAA_Database.SaaSql?.SetScCommandTaskHistory(CommandTask);
                                                                    }
                                                                }
                                                                //else
                                                                //{
                                                                //    SAA_Database.SaaSql?.DelScLiftCarrierInfoMaterial(SaaSettingCarrierId.SETNO, SaaSettingCarrierId.MODEL_NAME, SaaSettingCarrierId.STATIOM_NAME, SaaSettingCarrierId.CARRIERID);
                                                                //    SAA_Database.LogMessage($"【{SaaSettingCarrierId.STATIOM_NAME}】LocationSetting資料表查無此卡匣ID:{SaaSettingCarrierId.CARRIERID}");
                                                                //}
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //退REJECT
                        if (EquipmentSiteOffset?[3] == 1)
                        {
                            var sitdata = SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.STATION_NAME);
                            if (sitdata != null)
                            {
                                if (sitdata.Rows.Count == 0)
                                {
                                    var ScCommandTask = SAA_Database.SaaSql?.GetScCommandTaskResult(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME, "PGV-OUT");
                                    if (ScCommandTask?.Rows.Count == 0)
                                    {
                                        var liftcarrierinfojeject = SAA_Database.SaaSql?.GetScLiftCarrierInfoReject(EquipmentPlcOffset.SETNO, EquipmentPlcOffset.MODEL_NAME, EquipmentPlcOffset.STATION_NAME);
                                        if (liftcarrierinfojeject?.Rows.Count != 0)
                                        {
                                            SaaScLocationSetting SaaSettingCarrierId = new SaaScLocationSetting
                                            {
                                                SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                STATIOM_NAME = liftcarrierinfojeject?.Rows[0]["STATION_NAME"].ToString()!,
                                                CARRIERID = liftcarrierinfojeject?.Rows[0]["CARRIERID"].ToString()!,
                                            };
                                            var settingcarrieriddata = SAA_Database.SaaSql?.GetScLocationSettingCarrierId(SaaSettingCarrierId);
                                            if (settingcarrieriddata != null)
                                            {
                                                if (settingcarrieriddata.Rows.Count != 0)
                                                {
                                                    SaaScEquipmentReport EquipmentReport = new SaaScEquipmentReport
                                                    {
                                                        TASKDATETIME = SAA_Database.ReadTime(),
                                                        SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                        MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                        STATION_NAEM = EquipmentPlcOffset.STATION_NAME,
                                                        CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                        REPORE_DATATRACK = "405",
                                                        REPORE_DATAREMOTE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,
                                                        REPORE_DATALOCAL = "PGV-OUT",
                                                    };
                                                    SAA_Database.SaaSql?.SetEquipmentReport(EquipmentReport);

                                                    SaaScCommandTask CommandTask = new SaaScCommandTask
                                                    {
                                                        TASKDATETIME = SAA_Database.ReadTime(),
                                                        SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                                        MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                                        STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                                        CARRIERID = settingcarrieriddata?.Rows[0]["CARRIERID"].ToString()!,
                                                        COMMANDID = $"{DateTime.Now:fff}",
                                                        LOCATIONPUT = "PGV-OUT",
                                                        LOCATIONTAKE = settingcarrieriddata?.Rows[0]["LOCATIONID"].ToString()!,

                                                    };
                                                    if (SAA_Database.SaaSql?.GetScCommandTask(EquipmentPlcOffset.STATION_NAME).Rows.Count == 0)
                                                    {
                                                        SAA_Database.SaaSql?.SetScCommandTask(CommandTask);
                                                        SAA_Database.SaaSql?.SetScCommandTaskHistory(CommandTask);
                                                    }
                                                }
                                                //else
                                                //{
                                                //    SAA_Database.SaaSql?.DelScLiftCarrierInfoReject(SaaSettingCarrierId.SETNO, SaaSettingCarrierId.MODEL_NAME, SaaSettingCarrierId.STATIOM_NAME, SaaSettingCarrierId.CARRIERID);
                                                //    SAA_Database.LogMessage($"【{SaaSettingCarrierId.STATIOM_NAME}】LocationSetting資料表查無此卡匣ID:{SaaSettingCarrierId.CARRIERID}");
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        public async void ReadEquipmentAlarm(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        EquipmentAlarmStart(EquipmentPlcOffset);
                        EquipmentPlcRejectStart(EquipmentPlcOffset);
                        Thread.Sleep(3000);
                    }
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void EquipmentAlarmStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                EquipmentOffset.PlcAlarmList = SaaEquipmentPlc?.ReadIntArray("W", "460", 21)!;
                if (SaaAlarm)
                {
                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】PLC:Alarm Index:{EquipmentOffset.PlcAlarmList[0]}，PC:Alarm Index:{EquipmentOffset.PcAlarmList![0]}");
                    for (int i = 1; i < EquipmentOffset.PlcAlarmList.Length; i++)
                    {
                        if (EquipmentOffset.PlcAlarmList[i] != 0)
                        {
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】PLC:ALARM_CODE:{EquipmentOffset.PlcAlarmList[i]}，PC:ALARM_CODE:{EquipmentOffset.PcAlarmList![i]}");
                            SaaScAlarmCurrent alarmcurrent = new SaaScAlarmCurrent
                            {
                                SETNO = EquipmentPlcOffset.SETNO,
                                MODELNAME = EquipmentPlcOffset.MODEL_NAME,
                                STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                ALARM_CODE = EquipmentOffset!.PlcAlarmList[i].ToString().PadLeft(4, '0'),
                                START_TIME = SAA_Database.ReadTime(),
                                ALARM_STATUS = "1",//1:發生 2:解除
                                REPORT_STATUS = string.Empty,
                            };
                            var alarmdata = SAA_Database.SaaSql?.GetScAlarmCurrent(alarmcurrent);
                            if (alarmdata?.Rows.Count == 0)
                            {

                                SAA_Database.SaaSql?.SetScAlarmCurrent(alarmcurrent);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】新增ALARM資料:ALARM_CODE:{alarmcurrent.ALARM_CODE}，START_TIME:{alarmcurrent.STATION_NAME}，ALARM_STATUS:{alarmcurrent.ALARM_STATUS}(1:發生，2:解除)。");
                            }
                            int index = EquipmentOffset.PlcAlarmListCount.IndexOf(alarmcurrent.ALARM_CODE);
                            if (index == -1)
                            {
                                EquipmentOffset.PlcAlarmListCount.Add(alarmcurrent.ALARM_CODE);
                                SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】記憶體新增ALARM資料:ALARM_CODE:{alarmcurrent.ALARM_CODE}");
                            }
                            EquipmentOffset.PcAlarmList = EquipmentOffset.PlcAlarmList;
                        }
                    }

                    for (int i = 0; i < EquipmentOffset.PlcAlarmListCount.Count; i++)
                    {
                        SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【初始化】【ALARM LIST】查詢記憶體ALARM資料==>第{i}筆:{EquipmentOffset.PlcAlarmListCount[i]}。");
                    }
                    SaaAlarm = false;
                }

                if (EquipmentOffset.PlcAlarmList != null && EquipmentOffset.PcAlarmList != null)
                {
                    if (EquipmentOffset.PlcAlarmList.Length != 0)
                    {
                        if (EquipmentOffset.PlcAlarmList[0] != EquipmentOffset.PcAlarmList[0])
                        {
                            EquipmentOffset.PlcAlarmListCount.Clear();
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】PLC:Alarm Index:{EquipmentOffset.PlcAlarmList[0]}，PC:Alarm Index:{EquipmentOffset.PcAlarmList![0]}");
                            for (int i = 1; i < EquipmentOffset.PlcAlarmList.Length; i++)
                            {
                                if (EquipmentOffset.PlcAlarmList[i] != 0)
                                {
                                    SaaScAlarmCurrent alarmcurrent = new SaaScAlarmCurrent
                                    {
                                        SETNO = EquipmentPlcOffset.SETNO,
                                        MODELNAME = EquipmentPlcOffset.MODEL_NAME,
                                        STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                        ALARM_CODE = EquipmentOffset!.PlcAlarmList[i].ToString().PadLeft(4, '0'),
                                        START_TIME = SAA_Database.ReadTime(),
                                        ALARM_STATUS = "1",//1:發生 2:解除
                                        REPORT_STATUS = string.Empty,
                                    };
                                    var alarmdata = SAA_Database.SaaSql?.GetScAlarmCurrent(alarmcurrent);
                                    if (alarmdata?.Rows.Count == 0)
                                    {
                                        SAA_Database.SaaSql?.SetScAlarmCurrent(alarmcurrent);
                                        SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】新增ALARM資料:ALARM_CODE:{alarmcurrent.ALARM_CODE}，START_TIME:{alarmcurrent.STATION_NAME}，ALARM_STATUS:{alarmcurrent.ALARM_STATUS}(1:發生，2:解除)。");
                                    }

                                    int index = EquipmentOffset.PlcAlarmListCount.IndexOf(alarmcurrent.ALARM_CODE);
                                    if (index == -1)
                                        EquipmentOffset.PlcAlarmListCount.Add(alarmcurrent.ALARM_CODE);
                                }
                            }

                            var alarmstationdata = SAA_Database.SaaSql?.GetScAlarmCurrentStation(EquipmentPlcOffset.STATION_NAME);
                            if (alarmstationdata?.Rows.Count != 0)
                            {
                                foreach (DataRow dr in alarmstationdata!.Rows)
                                {
                                    for (int i = 0; i < EquipmentOffset.PlcAlarmListCount.Count; i++)
                                    {
                                        SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】查詢記憶體ALARM資料==>第{i}筆:{EquipmentOffset.PlcAlarmListCount[i]}。");
                                    }
                                    int index = EquipmentOffset.PlcAlarmListCount.IndexOf(dr["ALARM_CODE"].ToString()!);
                                    SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】查詢記憶體ALARM資料:ALARM_CODE:{dr["ALARM_CODE"]}，結果:{index}(-1解除)。");
                                    if (index == -1)
                                    {
                                        SaaScAlarmCurrent alarmcurrent = new SaaScAlarmCurrent
                                        {
                                            SETNO = EquipmentPlcOffset.SETNO,
                                            MODELNAME = EquipmentPlcOffset.MODEL_NAME,
                                            STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                            ALARM_CODE = dr["ALARM_CODE"].ToString()!.PadLeft(4, '0'),
                                            ALARM_STATUS = "2",//1:發生 2:解除
                                            END_TME = SAA_Database.ReadTime(),
                                            REPORT_STATUS = string.Empty,
                                        };
                                        SaaScAlarmCurrent alarmcurrentend = new SaaScAlarmCurrent
                                        {
                                            SETNO = EquipmentPlcOffset.SETNO,
                                            MODELNAME = EquipmentPlcOffset.MODEL_NAME,
                                            STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                            ALARM_CODE = dr["ALARM_CODE"].ToString()!.PadLeft(4, '0'),
                                            ALARM_STATUS = "1",//1:發生 2:解除
                                            REPORT_STATUS = SAA_DatabaseEnum.SendFlag.Y.ToString(),
                                        };
                                        var CurrentNnd = SAA_Database.SaaSql?.GetScAlarmCurrentNnd(alarmcurrentend);
                                        if (CurrentNnd?.Rows.Count != 0)
                                        {
                                            SAA_Database.SaaSql?.UpdScAlarmCurrent(alarmcurrent);
                                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】更新ALARM資料:ALARM_CODE:{alarmcurrent.ALARM_CODE}，END_TME:{alarmcurrent.END_TME}，ALARM_STATUS:{alarmcurrent.ALARM_STATUS}(1:發生，2:解除)。");
                                        }
                                        else
                                        {
                                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】【ALARM LIST】查詢數量{CurrentNnd?.Rows.Count}，ALARM資料:ALARM_CODE:{dr["ALARM_CODE"]}，結果:{index}(解除)。");
                                        }
                                    }
                                }
                            }

                            EquipmentOffset.PcAlarmList = EquipmentOffset.PlcAlarmList;
                            EquipmentOffset.PcAlarmListCount = EquipmentOffset.PlcAlarmListCount;

                            SaaLiftReceive saaLift = new SaaLiftReceive
                            {
                                Statiom_Name = EquipmentPlcOffset.STATION_NAME,
                                CommandName = SAA_DatabaseEnum.SaaLiftCommandName.EquipmentLiftAlarmList.ToString(),
                            };
                            Dictionary<string, object> dicstatusb = new Dictionary<string, object>
                           {
                               { SAA_DatabaseEnum.EquipmentStatusCommand.Statiom_Name.ToString(),  saaLift.Statiom_Name},
                               { SAA_DatabaseEnum.EquipmentStatusCommand.CommandName.ToString(), saaLift.CommandName}
                           };
                            string commandcontent = JsonConvert.SerializeObject(saaLift);
                            string ReportMessage = SAA_Database.SaaSendCommandSystems(commandcontent, SAA_DatabaseEnum.SendWebApiCommandName.GetLiftMessage.ToString());
                            SAA_Database.LogMessage($"【{saaLift.Statiom_Name}】【傳送設備】【EQ-LCS至轉譯程式】接收結果:{ReportMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public void EquipmentPlcRejectStart(SaaEquipmentPlcOffsetAttributes EquipmentPlcOffset)
        {
            try
            {
                EquipmentOffset.PlcReject = SaaEquipmentPlc?.ReadByteArray("W", "413", 16)!;
                if (EquipmentOffset.PlcReject != null)
                {
                    if (EquipmentOffset.PlcReject.Length >= 15)
                    {
                        if (!EquipmentOffset.PlcReject.SequenceEqual(EquipmentOffset.PcReject))
                        {
                            int autoreject = EquipmentOffset.PlcReject[1] || EquipmentOffset.PlcReject[0] ? 1 : 0;//1:PLC開啟強制退盒 0:PLC關閉強制退盒
                            SaaScDevice device = new SaaScDevice
                            {
                                SETNO = int.Parse(EquipmentPlcOffset.SETNO),
                                MODEL_NAME = EquipmentPlcOffset.MODEL_NAME,
                                STATION_NAME = EquipmentPlcOffset.STATION_NAME,
                                AUTOREJECT = autoreject,
                            };
                            SAA_Database.SaaSql?.UpdScDeviceAutoReject(device);
                            SAA_Database.LogMessage($"【{EquipmentPlcOffset.STATION_NAME}】{EquipmentPlcOffset.STATION_NAME}機台強制退盒模式為:{device.AUTOREJECT} (1:PLC開啟強制退盒 0:PLC關閉強制退盒)");
                            EquipmentOffset.PcReject = EquipmentOffset.PlcReject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        public string ReadSringArray(int[] readblockcount)
        {
            int[] readint = readblockcount;
            var StrArrays = readint
                    .Select(num => (ushort)num)
                    .Select(num => num.AsBitArray())
                    .Select(bits =>
                    {
                        var binaryStr_1 = "";
                        var binaryStr_2 = "";
                        for (var i = 0; i < bits.Length; i++)
                        {
                            if (i < 8)
                                binaryStr_1 = (bits[i] ? "1" : "0") + binaryStr_1;
                            else
                                binaryStr_2 = (bits[i] ? "1" : "0") + binaryStr_2;
                        }
                        var ascii_1 = ((char)Convert.ToUInt32(binaryStr_1, 2)).ToString();
                        var ascii_2 = ((char)Convert.ToUInt32(binaryStr_2, 2)).ToString();
                        return ascii_1 + ascii_2;
                    }).ToArray();
            return string.Join("", StrArrays).CleanString();
        }

        #region [===字串轉Int===]
        /// <summary>
        /// 字串轉Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<int> ReadStringToInt(string str)
        {
            var char_arr = str.ToCharArray().ToArray();
            var chunk_char_arr = char_arr.Chunk(2);
            var tmp = new List<int>();
            foreach (var item in chunk_char_arr)
            {
                var binary_str = "";
                foreach (var c in item)
                {
                    binary_str = Convert.ToString(c, 2).PadLeft(8, '0') + binary_str;
                }
                tmp.Add(Convert.ToUInt16(binary_str, 2));
            }
            return tmp;
        }
        #endregion

        private void ReadOffset()
        {
            for (int i = 0; i < SAA_Database.configattributes.SaaOffsetGroup; i++)
            {
                switch (i)
                {
                    case 0:
                        SaaDataTrack[i] = SAA_Database.EquipmentCommon.DataTrack1;
                        SaaDaataReply[i] = SAA_Database.EquipmentCommon.DataReply1;
                        SaaDataLocal[i] = SAA_Database.EquipmentCommon.DataLocal1;
                        SaaDataRemote[i] = SAA_Database.EquipmentCommon.DataRemote1;
                        SaaDataCarrierId[i] = SAA_Database.EquipmentCommon.DataCarrierId1;
                        break;
                    case 1:
                        SaaDataTrack[i] = SAA_Database.EquipmentCommon.DataTrack2;
                        SaaDaataReply[i] = SAA_Database.EquipmentCommon.DataReply2;
                        SaaDataLocal[i] = SAA_Database.EquipmentCommon.DataLocal2;
                        SaaDataRemote[i] = SAA_Database.EquipmentCommon.DataRemote2;
                        SaaDataCarrierId[i] = SAA_Database.EquipmentCommon.DataCarrierId2;
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdOffsetOff()
        {
            SaaEquipmentPlc?.WriteBool("B", "2C0", false);
            SaaEquipmentPlc?.WriteBool("B", "2D0", false);
            SaaEquipmentPlc?.WriteBool("B", "51", false);
            SaaEquipmentPlc?.WriteBool("B", "50", false);
            SAA_Database.SaaSql?.UpdScCommandTask();
            SAA_Database.LogMessage($"SC_SOMMANDTASK任務更新為Null");
        }

        public void DelCommandTask(DataTable data, string stationname, string location)
        {
            if (data.Rows.Count != 0)
            {
                SaaScCommandTask CommandTask = new SaaScCommandTask()
                {
                    TASKDATETIME = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.TASKDATETIME.ToString()].ToString()!,
                    SETNO = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.SETNO.ToString()].ToString()!),
                    MODEL_NAME = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.MODEL_NAME.ToString()].ToString()!,
                    STATION_NAME = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.STATION_NAME.ToString()].ToString()!,
                    COMMANDID = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.COMMANDID.ToString()].ToString()!,
                    CARRIERID = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.CARRIERID.ToString()].ToString()!,
                    LOCATIONTAKE = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.LOCATIONTAKE.ToString()].ToString()!,
                    LOCATIONPUT = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.LOCATIONPUT.ToString()].ToString()!,
                    RESULT = data.Rows[0][SAA_DatabaseEnum.SC_COMMAND_TASK.RESULT.ToString()].ToString()!
                };
                CommandTask.RESULT = SAA_DatabaseEnum.SendFlag.X.ToString();
                SAA_Database.SaaSql?.UpdScCommandTask(CommandTask);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】無卡匣取消");

                SAA_Database.SaaSql?.DelScCommandTask(stationname, CommandTask.CARRIERID);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除SC_COMMAND_TASK資料卡匣ID:{CommandTask.CARRIERID}");
                SAA_Database.SaaSql?.DelScLiftCarrierInfoEmpty(CommandTask.SETNO, CommandTask.MODEL_NAME, stationname, CommandTask.CARRIERID);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除LiftCarrierInfoEmpty資料，卡匣ID:{CommandTask.CARRIERID}");
                SAA_Database.SaaSql?.DelScLiftCarrierInfoMaterial(CommandTask.SETNO, CommandTask.MODEL_NAME, stationname, CommandTask.CARRIERID);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除LiftCarrierInfoMaterial資料，卡匣ID:{CommandTask.CARRIERID}");
                string readplc = string.Empty;
                var equipmentcarrierinfo = SAA_Database.SaaSql?.GetScEquipmentCarrierInfo(stationname, CommandTask.CARRIERID);
                if (equipmentcarrierinfo?.Rows.Count != 0)
                {
                    readplc = equipmentcarrierinfo?.Rows[0]["READPLC"].ToString()!;
                }
                if (readplc != "98" && readplc != "99")
                {
                    SAA_Database.SaaSql?.DelScLiftCarrierInfoReject(CommandTask.SETNO, CommandTask.MODEL_NAME, stationname, CommandTask.CARRIERID);
                    SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除LiftCarrierInfoReject資料，卡匣ID:{CommandTask.CARRIERID}");
                }
                SAA_Database.SaaSql?.DelScEquipmentCarrierInfo(CommandTask.STATION_NAME, CommandTask.CARRIERID);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至D{location}】刪除ScEquipmentCarrierInfo資料，卡匣ID:{CommandTask.CARRIERID}");


                SaaScLiftCarrierInfo LiftCarrierInfo = new SaaScLiftCarrierInfo
                {
                    SETNO = CommandTask.SETNO,
                    MODEL_NAME = CommandTask.MODEL_NAME,
                    STATION_NAME = stationname,
                    CARRIERID = CommandTask.CARRIERID,
                };

                SAA_Database.SaaSql?.DelLiftCarrierInfo(LiftCarrierInfo);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除LiftCarrierInfo資料，卡匣ID:{LiftCarrierInfo.CARRIERID}");
                SAA_Database.SaaSql?.DelScTransportrequirement(LiftCarrierInfo.STATION_NAME, LiftCarrierInfo.CARRIERID);
                SAA_Database.LogMessage($"【{stationname}】【手臂搬運】【卡匣傳送至{location}】刪除ScTransportrequirement資料(叫車資料表)，站點:{LiftCarrierInfo.STATION_NAME}，卡匣ID:{LiftCarrierInfo.CARRIERID}");
            }
        }
    }
}
