using SAA.Core.PLC.Slmp.Functions;
using SAA_EquipmentMonitor_VST100_Lib.Attributes;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentImp.MonitorCommands;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcImp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentLib
{
    public class SaaEquipmentLib
    {
        private bool connect = false;
        private EquipmentPlc? SaaEquipmentPlc = null;
        private SaaEquipmentPlcE84Handshake? SaaEquipmentPlcE84;
        private string rgvcarrierid_db = string.Empty;
        private string rgve84_db = string.Empty;
        private string rgve84ack_db = string.Empty;
        private string rgvcarrierid_value = string.Empty;
        private string rgve84_value = string.Empty;
        private string rgve84ack_value = string.Empty;

        public async void SaaEquipmentMonitorStart(SaaEquipmentPlcAttributes SaaEquipment)
        {
            try
            {
                SaaEquipmentPlc = new EquipmentPlc(SaaEquipment);
                await Task.Run(() =>
                {
                    while (true)
                    {
                        connect = EquipmentPlcConnect();
                        SaaScDevice device = new SaaScDevice
                        {
                            SETNO = SaaEquipment.SaaSetNo,
                            MODEL_NAME = SaaEquipment.SaaModelName,
                            STATION_NAME = SaaEquipment.EquipmentStation,
                        };
                        if (connect)
                        {
                            SAA_Database.LogMessage($"【{SaaEquipment.SaaModelName}】設備機型:{SaaEquipment.SaaModelName}設備連線成功 IP:{SaaEquipment.ActHostAddress}，Port:{SaaEquipment.ActPort}，站點名稱:{SaaEquipment.EquipmentStation}");
                            SAA_Database.LogMessage($"【{SaaEquipment.SaaModelName}】設備初始化完成");
                            device.CONNECTSTATUS = SAA_DatabaseEnum.ConnectStatus.Y.ToString();
                            SAA_Database.SaaSql?.UpdScDeviceConnectStatus(device);
                            break;
                        }
                        else
                        {
                            SAA_Database.LogMessage($"【{SaaEquipment.SaaModelName}】設備機型:{SaaEquipment.SaaModelName}設備連線失敗 IP:{SaaEquipment.ActHostAddress}，Port:{SaaEquipment.ActPort}，站點名稱:{SaaEquipment.EquipmentStation}，重新連線", SAA_DatabaseEnum.LogType.Error);
                            device.CONNECTSTATUS = SAA_DatabaseEnum.ConnectStatus.N.ToString();
                            SAA_Database.SaaSql?.UpdScDeviceConnectStatus(device);
                        }
                        Thread.Sleep(1000);
                    }
                    SaaEquipmentPlcE84 = new SaaEquipmentPlcE84Handshake(SaaEquipment.EquipmentStation, SaaEquipmentPlc);
                    var mappingdata = SAA_Database.SaaSql?.GetScEcaIoMapping(SaaEquipment.SaaSetNo, SaaEquipment.SaaModelName)!;
                    rgvcarrierid_db = string.Empty;
                    rgve84_db = string.Empty;
                    rgve84ack_db = string.Empty;
                    rgvcarrierid_value = string.Empty;
                    rgve84_value = string.Empty;
                    rgve84ack_value = string.Empty;
                    foreach (DataRow dt in mappingdata.Rows)
                    {
                        string eventfunction = dt["EVENTFUNCTION"].ToString()!;
                        
                        if (Enum.IsDefined(typeof(SAA_DatabaseEnum.PlcEventfunction), eventfunction))
                        {
                            switch ((SAA_DatabaseEnum.PlcEventfunction)Enum.Parse(typeof(SAA_DatabaseEnum.PlcEventfunction), eventfunction))
                            {
                                case SAA_DatabaseEnum.PlcEventfunction.RGVCARRIERID:
                                    rgvcarrierid_db = dt[SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                    rgvcarrierid_value = dt[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                    break;
                                case SAA_DatabaseEnum.PlcEventfunction.RGVE84:
                                    rgve84_db = dt[SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                    rgve84_value = dt[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                    break;
                                case SAA_DatabaseEnum.PlcEventfunction.RGVE84Ack:
                                    rgve84ack_db = dt[SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                    rgve84ack_value = dt[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                    break;
                            }
                        }
                    }
                    SaaEquipmentPlcOffsetAttributes SaaEquipmentPlcOffset = new SaaEquipmentPlcOffsetAttributes
                    {
                        SETNO = SaaEquipment.SaaSetNo.ToString(),
                        MODEL_NAME = SaaEquipment.SaaModelName,
                        STATION_NAME = SaaEquipment.EquipmentStation,
                        E84PLC_DB = rgve84_db,
                        E84PLC_VALUE = rgve84_value,
                        E84PC_DB = rgve84ack_db,
                        E84PC_VALUE = rgve84ack_value,
                        E84PC_CARRRIERID_DB = rgvcarrierid_db,
                        E84PC_CARRRIERID_VALUE = rgvcarrierid_value,
                    };
                    //SaaEquipmentPlcE84.SaaEquipmentMonitorE84PlcSendStart(SaaEquipmentPlcOffset);
                    //SaaEquipmentPlcE84.SaaEquipmentMonitorE84PcSendStart(SaaEquipmentPlcOffset);
                    SaaEquipmentPlcE84.SaaEquipmentUpdDateTimeLifebitClock(SaaEquipmentPlcOffset);
                    SaaEquipmentPlcE84.CommandStart(SaaEquipmentPlcOffset);
                    SaaEquipmentPlcE84.CommandEquipmentSiteStart(SaaEquipmentPlcOffset);
                    SaaEquipmentPlcE84.ReadPlcActionOffset();
                    SaaEquipmentPlcE84.SaaEquipmentMonitorStart(SaaEquipment.SaaSetNo, SaaEquipment.SaaModelName, SaaEquipmentPlcOffset.STATION_NAME);
                    SaaEquipmentPlcE84.ReadEquipmentAlarm(SaaEquipmentPlcOffset);
                });
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }

        #region [===設備連線===]
        /// <summary>
        /// 設備連線
        /// </summary>
        /// <returns></returns>
        public bool EquipmentPlcConnect()
        {
            return SaaEquipmentPlc!.SAA_PlcConnect();
        }
        #endregion
    }
}
