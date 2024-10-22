using Microsoft.AspNetCore.Mvc;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.Controllers
{
    public class WebApiController : ControllerBase
    {
        public delegate void DelLiftE84PcListData(SaaScLiftE84Pc LiftE84Pc);
        public static event DelLiftE84PcListData? OnLiftE84PcData;

        public delegate void DeShelfMaxAmountData(SaaEquipmentShelfMaxAmount ShelfMaxAmount);
        public static event DeShelfMaxAmountData? OnShelfMaxAmountData;

        [Route("SaaEquipmentMonitorE84PcSendStart")]
        [HttpPost]
        public string SaaEquipmentMonitorE84PcSendStart([FromBody] SaaScLiftE84iLisPc data)
        {
            try
            {
                if (data != null)
                {
                    SAA_Database.LogMessage($"【{data.STATION_NAME}】E84站點:TASKDATETIME:{data.TASKDATETIME}，STATION_NAME:{data.STATION_NAME}，SHUTTLEID:{data.SHUTTLEID}");
                    SaaScLiftE84Pc LiftE84Pc = new SaaScLiftE84Pc
                    {
                        TASKDATETIME = data.TASKDATETIME,
                        STATION_NAME = data.STATION_NAME,
                        SHUTTLEID = data.SHUTTLEID,
                        COMMANDID = data.COMMANDID,
                        CARRIERID = data.CARRIERID,
                        Mode = data.Mode,
                        VALID = data.VALID == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        CS_0 = data.CS_0 == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        CS_1 = data.CS_1 == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        TR_REQ = data.TR_REQ == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        L_REQ = data.L_REQ == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        U_REQ = data.U_REQ == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        READY = data.READY == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        BUSY = data.BUSY == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        COMPT = data.COMPT == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        CONT = data.CONT == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        HOA_VBL = data.HOA_VBL == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        ES = data.ES == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        VA = data.VA == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        AM_AVBL = data.AM_AVBL == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        VS_0 = data.VS_0 == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                        VS_1 = data.VS_1 == SAA_DatabaseEnum.E84DataStatus.True.ToString() ? 1 : 0,
                    };
                    OnLiftE84PcData?.Invoke(LiftE84Pc);
                    return SAA_Database.configattributes.WebApiResultOK;
                }
                return SAA_Database.configattributes.WebApiResultFAIL;
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
                return SAA_Database.configattributes.WebApiResultFAIL;
            }
        }

        [Route("SaaEquipmentMonitorShelfMaxAmount")]
        [HttpPost]
        public string SaaEquipmentMonitorShelfMaxAmount([FromBody] SaaEquipmentShelfMaxAmount data)
        {
            try
            {
                if (data != null)
                {
                    SAA_Database.LogMessage($"【{data.StationName}】實盒最大數量:{data.MaterialCount}，空盒最大數量:{data.EmptyCount}，分散式儲格最大數量");
                    OnShelfMaxAmountData?.Invoke(data);
                    return SAA_Database.configattributes.WebApiResultOK;
                }
                return SAA_Database.configattributes.WebApiResultFAIL;
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
                return SAA_Database.configattributes.WebApiResultFAIL;
            }
        }
    }
}
