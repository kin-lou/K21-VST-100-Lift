using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;
using SAA_MsSqlData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.MsSqlData
{
    public class SqlData
    {
        private SaaMsSql SaaSql = new SaaMsSql(SAA_Database.configattributes.SaaDataBase, SAA_Database.configattributes.SaaDataBaseIP, SAA_Database.configattributes.SaaDataBaseName, SAA_Database.configattributes.SaaDataBasePassword);

        /*===============================新增=======================================*/

        #region [===新增卡匣詢問資料===]
        /// <summary>
        /// 新增卡匣詢問資料
        /// </summary>
        /// <param name="scpurchase"></param>
        public void SetScPurchase(SaaScPurchase scpurchase)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_PURCHASE Values('{scpurchase.TASKDATETIME}', '{scpurchase.SETNO}', '{scpurchase.MODEL_NAME}', '{scpurchase.STATION_NAME}', '{scpurchase.CARRIERID}', '{scpurchase.REPLY}', '{scpurchase.LOCAL}','{scpurchase.COMMANDID}', '{scpurchase.REPLYRESULT}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetScPurchase寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }
        #endregion

        public void SetScLifte84Plc(SaaEquipmentE84PlcAttributes lifte84plc)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_LIFT_E84PLC(TASKDATETIME, STATION_NAME, SHUTTLEID, COMMANDID, CARRIERID, CS_0, Valid, TR_REQ, Busy, Complete, [Continue], [SELECT], AM_AVBL) Values('{lifte84plc.TASKDATETIME}','{lifte84plc.STATION_NAME}','{lifte84plc.SHUTTLEID}','{lifte84plc.COMMANDID}','{lifte84plc.CARRIERID}','{lifte84plc.CS_0}','{lifte84plc.Valid}','{lifte84plc.TR_REQ}','{lifte84plc.Busy}','{lifte84plc.Complete}','{lifte84plc.Continue}','{lifte84plc.SELECT}','{lifte84plc.AM_AVBL}')");
                SAA_Database.LogMessage($"【新增SQL方法】【新增結果:{sqlreport}】Insert into SC_LIFT_E84PLC(TASKDATETIME, STATION_NAME, SHUTTLEID, COMMANDID, CARRIERID, CS_0, Valid, TR_REQ, Busy, Complete, [Continue], [SELECT], AM_AVBL) Values('{lifte84plc.TASKDATETIME}','{lifte84plc.STATION_NAME}','{lifte84plc.SHUTTLEID}','{lifte84plc.COMMANDID}','{lifte84plc.CARRIERID}','{lifte84plc.CS_0}','{lifte84plc.Valid}','{lifte84plc.TR_REQ}','{lifte84plc.Busy}','{lifte84plc.Complete}','{lifte84plc.Continue}','{lifte84plc.SELECT}','{lifte84plc.AM_AVBL}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetScLifte84Plc寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }

        public void SetEquipmentReportMonitor(SaaScEquipmentReportMonitor equipmentreportmonitor)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_EQUIPMENT_REPORT_MONITOR Values('{equipmentreportmonitor.SETNO}', '{equipmentreportmonitor.MODEL_NAME}', '{equipmentreportmonitor.STATION_NAEM}', '{equipmentreportmonitor.REPORTID}', '{equipmentreportmonitor.REPORT_TIME}', '{equipmentreportmonitor.REPORE_DATACOUNT}', '{equipmentreportmonitor.GROUP_NO}', '{equipmentreportmonitor.REPORE_DATATRACK}', '{equipmentreportmonitor.REPORE_DATAREPLY}','{equipmentreportmonitor.REPORE_DATALOCAL}','{equipmentreportmonitor.REPORE_DATAREMOTE}', '{equipmentreportmonitor.REPORE_DATAENROLLMENTINFO}', '{equipmentreportmonitor.CARRIERID}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetEquipmentReportMonitor寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }

        public void SetEquipmentReport(SaaScEquipmentReport EquipmentReport)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_EQUIPMENT_REPORT(TASKDATETIME, SETNO, MODEL_NAME, STATION_NAME, CARRIERID, REPORE_DATATRACK, REPORE_DATAREMOTE, REPORE_DATALOCAL) Values('{EquipmentReport.TASKDATETIME}','{EquipmentReport.SETNO}', '{EquipmentReport.MODEL_NAME}', '{EquipmentReport.STATION_NAEM}', '{EquipmentReport.CARRIERID}', '{EquipmentReport.REPORE_DATATRACK}', '{EquipmentReport.REPORE_DATAREMOTE}', '{EquipmentReport.REPORE_DATALOCAL}')");
                SAA_Database.LogMessage($"【新增SQL方法】【新增結果:{sqlreport}】Insert into SC_EQUIPMENT_REPORT(TASKDATETIME, SETNO, MODEL_NAME, STATION_NAME, CARRIERID, REPORE_DATATRACK, REPORE_DATAREMOTE, REPORE_DATALOCAL) Values('{EquipmentReport.TASKDATETIME}','{EquipmentReport.SETNO}', '{EquipmentReport.MODEL_NAME}', '{EquipmentReport.STATION_NAEM}', '{EquipmentReport.CARRIERID}', '{EquipmentReport.REPORE_DATATRACK}', '{EquipmentReport.REPORE_DATAREMOTE}', '{EquipmentReport.REPORE_DATALOCAL}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetEquipmentReport寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }

        public void SetScLiftCarrierInfo(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_LIFT_CARRIER_INFO(SETNO, MODEL_NAME, STATION_NAME, CARRIERID, CARRIERTYPE, REMOTE) Values('{LiftCarrierInfo.SETNO}','{LiftCarrierInfo.MODEL_NAME}','{LiftCarrierInfo.STATION_NAME}','{LiftCarrierInfo.CARRIERID}', '{LiftCarrierInfo.CARRIERTYPE}', '{LiftCarrierInfo.REMOTE}')");
                SAA_Database.LogMessage($"【新增SQL方法】【新增結果:{sqlreport}】Insert into SC_LIFT_CARRIER_INFO(SETNO, MODEL_NAME, STATION_NAME, CARRIERID, CARRIERTYPE, REMOTE) Values('{LiftCarrierInfo.SETNO}','{LiftCarrierInfo.MODEL_NAME}','{LiftCarrierInfo.STATION_NAME}','{LiftCarrierInfo.CARRIERID}', '{LiftCarrierInfo.CARRIERTYPE}', '{LiftCarrierInfo.REMOTE}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetScLiftCarrierInfo寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }//READPLC

        public void SetScLiftCarrierInfoReadPlc(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_LIFT_CARRIER_INFO(SETNO, MODEL_NAME, STATION_NAME, CARRIERID, CARRIERTYPE, REMOTE, READPLC) Values('{LiftCarrierInfo.SETNO}','{LiftCarrierInfo.MODEL_NAME}','{LiftCarrierInfo.STATION_NAME}','{LiftCarrierInfo.CARRIERID}', '{LiftCarrierInfo.CARRIERTYPE}', '{LiftCarrierInfo.REMOTE}', '{LiftCarrierInfo.READPLC}')");
                SAA_Database.LogMessage($"【新增SQL方法】【新增結果:{sqlreport}】Insert into SC_LIFT_CARRIER_INFO(SETNO, MODEL_NAME, STATION_NAME, CARRIERID, CARRIERTYPE, REMOTE, READPLC) Values('{LiftCarrierInfo.SETNO}','{LiftCarrierInfo.MODEL_NAME}','{LiftCarrierInfo.STATION_NAME}','{LiftCarrierInfo.CARRIERID}', '{LiftCarrierInfo.CARRIERTYPE}', '{LiftCarrierInfo.REMOTE}', '{LiftCarrierInfo.READPLC}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetScLiftCarrierInfo寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }//READPLC

        public void SetScLocationSetting(SaaScLocationSetting locationsetting)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_LOCATIONSETTING(SETNO, MODEL_NAME, STATIOM_NAME, LOCATIONID, HOSTID, CARRIERID, PARTNO, INVENTORYFULL, PUTTIME) Values('{locationsetting.SETNO}','{locationsetting.MODEL_NAME}', '{locationsetting.STATIOM_NAME}', '{locationsetting.LOCATIONID}', '{locationsetting.HOSTID}', '{locationsetting.CARRIERID}', '{locationsetting.PARTNO}','{locationsetting.INVENTORYFULL}', '{locationsetting.PUTTIME}')");
                if (sqlreport)
                    break;
                else
                    SAA_Database.LogMessage($"【新增SQL方法】資料庫SetScLocationSetting寫入失敗", SAA_DatabaseEnum.LogType.Error);
                Thread.Sleep(50);
            }
        }

        public void SetScCommandTask(SaaScCommandTask commandtask)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_COMMAND_TASK(TASKDATETIME, SETNO, MODEL_NAME, STATION_NAME, COMMANDID, CARRIERID, LOCATIONTAKE, LOCATIONPUT) Values('{commandtask.TASKDATETIME}', '{commandtask.SETNO}', '{commandtask.MODEL_NAME}', '{commandtask.STATION_NAME}','{commandtask.COMMANDID}', '{commandtask.CARRIERID}', '{commandtask.LOCATIONTAKE}', '{commandtask.LOCATIONPUT}')");
                if (sqlreport)
                    break;
                Thread.Sleep(50);
            }
        }

        public void SetScCommandTaskHistory(SaaScCommandTask commandtask)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_COMMAND_TASK_HISTORY(TASKDATETIME, SETNO, MODEL_NAME, STATION_NAME, COMMANDID, CARRIERID, LOCATIONTAKE, LOCATIONPUT) Values('{commandtask.TASKDATETIME}', '{commandtask.SETNO}', '{commandtask.MODEL_NAME}', '{commandtask.STATION_NAME}','{commandtask.COMMANDID}', '{commandtask.CARRIERID}', '{commandtask.LOCATIONTAKE}', '{commandtask.LOCATIONPUT}')");
                if (sqlreport)
                    break;
                Thread.Sleep(50);
            }
        }

        public void SetScEquipmentCarrierInfo(SaaEquipmentCarrierInfo equipmentcarrierinfo)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_EQUIPMENT_CARRIER_INFO(SETNO, MODEL_NAME, STATIOM_NAME, CARRIERID, PARTNO , CARRIERTYOE , ROTFLAG, FLIPFLAG, OPER, RECIPE, ORIGIN, DESTINATION, QTIME, CYCLETIME, REJECT_CODE, REJECT_MESSAGE, CARRIERSTATE, DESTINATIONTYPE) " +
                $"Values('{equipmentcarrierinfo.SETNO}', '{equipmentcarrierinfo.MODEL_NAME}', '{equipmentcarrierinfo.STATIOM_NAME}', '{equipmentcarrierinfo.CARRIERID}','{equipmentcarrierinfo.PARTNO}', '{equipmentcarrierinfo.CARRIERTYOE}', '{equipmentcarrierinfo.ROTFLAG}', '{equipmentcarrierinfo.FLIPFLAG}', '{equipmentcarrierinfo.OPER}', '{equipmentcarrierinfo.RECIPE}', '{equipmentcarrierinfo.ORIGIN}', '{equipmentcarrierinfo.DESTINATION}', '{equipmentcarrierinfo.QTIME}', '{equipmentcarrierinfo.CYCLETIME}', '{equipmentcarrierinfo.REJECT_CODE}', '{equipmentcarrierinfo.REJECT_MESSAGE}', '{equipmentcarrierinfo.CARRIERSTATE}', '{equipmentcarrierinfo.DESTINATIONTYPE}')");
                if (sqlreport)
                    break;
                Thread.Sleep(50);
            }
        }

        public void SetScTransportrEquirement(SaaScTransportrEquirement TransportrEquirement)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_TRANSPORTR_EQUIREMENT(SETNO, MODEL_NAME, STATION_NAME, REPORTID, REPORT_TIME , REPORT_STATION , REQUIREMENT_TYPE, CARRIERID, BEGIN_STATION, END_STATION) " +
                $"Values('{TransportrEquirement.SETNO}', '{TransportrEquirement.MODEL_NAME}', '{TransportrEquirement.STATION_NAME}', '{TransportrEquirement.REPORTID}','{TransportrEquirement.REPORT_TIME}', '{TransportrEquirement.REPORT_STATION}', '{TransportrEquirement.REQUIREMENT_TYPE}', '{TransportrEquirement.CARRIERID}','{TransportrEquirement.BEGIN_STATION}', '{TransportrEquirement.END_STATION}')");
                SAA_Database.LogMessage($"Insert into SC_TRANSPORTR_EQUIREMENT(SETNO, MODEL_NAME, STATION_NAME, REPORTID, REPORT_TIME , REPORT_STATION , REQUIREMENT_TYPE, CARRIERID, BEGIN_STATION, END_STATION) " +
                $"Values('{TransportrEquirement.SETNO}', '{TransportrEquirement.MODEL_NAME}', '{TransportrEquirement.STATION_NAME}', '{TransportrEquirement.REPORTID}','{TransportrEquirement.REPORT_TIME}', '{TransportrEquirement.REPORT_STATION}', '{TransportrEquirement.REQUIREMENT_TYPE}', '{TransportrEquirement.CARRIERID}','{TransportrEquirement.BEGIN_STATION}', '{TransportrEquirement.END_STATION}')");
                if (sqlreport)
                    break;
                Thread.Sleep(50);
            }
        }

        public void SetScLiftCarrierInfoReject(SaaScLiftCarrierInfoReject ScLiftCarrierInfoReject)
        {
            SaaSql.WriteSqlByAutoOpen($"Insert into SC_LIFT_CARRIER_INFO_REJECT(TASKDATETIME, SETNO, MODEL_NAME, STATION_NAME, CARRIERID, DEVICETYPE, QTIME, CYCLETIME) Values('{ScLiftCarrierInfoReject.TASKDATETIME}','{ScLiftCarrierInfoReject.SETNO}','{ScLiftCarrierInfoReject.MODEL_NAME}','{ScLiftCarrierInfoReject.STATION_NAME}','{ScLiftCarrierInfoReject.CARRIERID}', '{ScLiftCarrierInfoReject.DEVICETYPE}', '{ScLiftCarrierInfoReject.QTIME}', '{ScLiftCarrierInfoReject.CYCLETIME}')");
        }

        public void SetScLiftAmount(SaaScLiftCarrierAmount LiftCarrierAmount)
        {
            SaaSql.WriteSqlByAutoOpen($"Insert into SC_LIFT_CARRIER_AMOUNT(SETNO, MODEL_NAME, STATION_NAME, TASKDATETIME, ACTION, CARRIERID) Values('{LiftCarrierAmount.SETNO}','{LiftCarrierAmount.MODEL_NAME}','{LiftCarrierAmount.STATION_NAME}','{LiftCarrierAmount.TASKDATETIME}', '{LiftCarrierAmount.ACTION}', '{LiftCarrierAmount.CARRIERID}')");
        }

        public void SetScAlarmCurrent(SaaScAlarmCurrent AlarmCurrent)
        {
            SaaSql.WriteSqlByAutoOpen($"Insert into SC_ALARM_CURRENT(SETNO, MODEL_NAME, STATION_NAME, ALARM_CODE, ALARM_STATUS, REPORT_STATUS, START_TIME) " +
                $"Values('{AlarmCurrent.SETNO}', '{AlarmCurrent.MODELNAME}', '{AlarmCurrent.STATION_NAME}', '{AlarmCurrent.ALARM_CODE}', '{AlarmCurrent.ALARM_STATUS}', '{AlarmCurrent.REPORT_STATUS}', '{AlarmCurrent.START_TIME}')");
        }

        //public void SetScEquipmentCarrierInfo(SaaEquipmentCarrierInfo equipmentcarrierinfo)
        //{
        //    while (true)
        //    {
        //        bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Insert into SC_EQUIPMENT_CARRIER_INFO(SETNO, MODEL_NAME, STATIOM_NAME, CARRIERID, PARTNO , CARRIERTYOE , ROTFLAG, FLIPFLAG, OPER, RECIPE, ORIGIN, DESTINATION, QTIME, CYCLETIME, REJECT_CODE, REJECT_MESSAGE) " +
        //        $"Values('{equipmentcarrierinfo.SETNO}', '{equipmentcarrierinfo.MODEL_NAME}', '{equipmentcarrierinfo.STATIOM_NAME}', '{equipmentcarrierinfo.CARRIERID}','{equipmentcarrierinfo.PARTNO}', '{equipmentcarrierinfo.CARRIERTYOE}', '{equipmentcarrierinfo.ROTFLAG}', '{equipmentcarrierinfo.FLIPFLAG}', '{equipmentcarrierinfo.OPER}', '{equipmentcarrierinfo.RECIPE}', '{equipmentcarrierinfo.ORIGIN}', '{equipmentcarrierinfo.DESTINATION}', '{equipmentcarrierinfo.QTIME}', '{equipmentcarrierinfo.CYCLETIME}', '{equipmentcarrierinfo.REJECT_CODE}', '{equipmentcarrierinfo.REJECT_MESSAGE}')");
        //        if (sqlreport)
        //            break;
        //        Thread.Sleep(50);
        //    }
        //}

        /*===============================刪除=======================================*/

        public void DelScPurchase(string statiomname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Delete From SC_PURCHASE Where STATION_NAME = '" + statiomname + "' And CARRIERID = '" + carrierid + "' And REPLYRESULT is Not Null");
                SAA_Database.LogMessage($"【方法名稱DelScPurchase】【SQL指令】Delete From SC_PURCHASE Where STATION_NAME = '" + statiomname + "' And CARRIERID = '" + carrierid + "' And REPLYRESULT is Not Null");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }


        public void DelLiftCarrierInfo(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_LIFT_CARRIER_INFO Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERID = '" + LiftCarrierInfo.CARRIERID + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScLiftCarrierInfoEmpty(int setno, string model_name, string stationname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_LIFT_CARRIER_INFO_EMPTY Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScLiftCarrierInfoReject(int setno, string model_name, string stationname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_LIFT_CARRIER_INFO_REJECT Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScLiftCarrierInfoMaterial(int setno, string model_name, string stationname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_LIFT_CARRIER_INFO_MATERIAL Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScEquipmentCarrierInfo(string stationname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_EQUIPMENT_CARRIER_INFO Where STATIOM_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScTransportrequirement(string stationname, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_TRANSPORTR_EQUIREMENT Where STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScTransportrequirement(string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_TRANSPORTR_EQUIREMENT Where CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScAlarmCurrent(SaaScAlarmCurrent AlarmCurrent)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Delete From SC_ALARM_CURRENT Where STATION_NAME = '" + AlarmCurrent.STATION_NAME + "' And ALARM_CODE = '" + AlarmCurrent.ALARM_CODE + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScCommandTask(string station_name, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Delete From SC_COMMAND_TASK Where STATION_NAME = '" + station_name + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScCommandTask(string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Delete From SC_COMMAND_TASK Where CARRIERID = '" + carrierid + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void DelScTransportrEquirementMaterial(string carrierid)
        {
            SaaSql.WriteSqlByAutoOpen($"Delete From SC_TRANSPORTR_EQUIREMENT_MATERIAL Where CARRIERID = '" + carrierid + "'");
        }

        /*===============================更新=======================================*/

        public void UpdScLocationSetting(SaaScLocationSetting locationsetting)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Update SC_LOCATIONSETTING Set CARRIERID = '" + locationsetting.CARRIERID + "', PARTNO = '" + locationsetting.PARTNO + "', INVENTORYFULL = '" + locationsetting.INVENTORYFULL + "', PUTTIME = '" + locationsetting.PUTTIME + "', OPER = '" + locationsetting.OPER + "', CARRIERSTATE = '" + locationsetting.CARRIERSTATE + "', DESTINATIONTYPE = '" + locationsetting.DESTINATIONTYPE + "' Where LOCATIONID = '" + locationsetting.LOCATIONID + "' And SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And STATIOM_NAME = '" + locationsetting.STATIOM_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLocationSettingiLIS(SaaScLocationSetting locationsetting)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Update SC_LOCATIONSETTING_iLIS Set CARRIERID = '" + locationsetting.CARRIERID + "', PARTNO = '" + locationsetting.PARTNO + "', INVENTORYFULL = '" + locationsetting.INVENTORYFULL + "', PUTTIME = '" + locationsetting.PUTTIME + "', OPER = '" + locationsetting.OPER + "', CARRIERSTATE = '" + locationsetting.CARRIERSTATE + "', DESTINATIONTYPE = '" + locationsetting.DESTINATIONTYPE + "' Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And STATIOM_NAME = '" + locationsetting.STATIOM_NAME + "' And CARRIERID = '" + locationsetting.CARRIERID1 + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLocationSettingMode(SaaScLocationSetting locationsetting)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Update SC_LOCATIONSETTING Set LOCATIONMODE = '" + locationsetting.LOCATIONMODE + "' Where LOCATIONID = '" + locationsetting.LOCATIONID + "' And SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And STATIOM_NAME = '" + locationsetting.STATIOM_NAME + "' And CARRIERID = '" + locationsetting.CARRIERID + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLocationSettingCarrierid(SaaScLocationSetting locationsetting)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LOCATIONSETTING Set CARRIERID = '" + locationsetting.CARRIERID + "' Where LOCATIONID = '" + locationsetting.LOCATIONID + "' And SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And STATIOM_NAME = '" + locationsetting.STATIOM_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdateScLiftE84Pc(SaaScLiftE84Pc lifttask)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_E84PC Set RESULT = '" + lifttask.RESULT + "' Where TASKDATETIME = '" + lifttask.TASKDATETIME + "' And STATION_NAME = '" + lifttask.STATION_NAME + "' And COMMANDID = '" + lifttask.COMMANDID + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScDeviceConnectStatus(SaaScDevice scdevice)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_DEVICE Set CONNECTSTATUS = '" + scdevice.CONNECTSTATUS + "' Where SETNO = '" + scdevice.SETNO + "' And STATION_NAME = '" + scdevice.STATION_NAME + "' And MODEL_NAME = '" + scdevice.MODEL_NAME + "'");
                SAA_Database.LogMessage("Update SC_DEVICE Set CONNECTSTATUS = '" + scdevice.CONNECTSTATUS + "' Where SETNO = '" + scdevice.SETNO + "' And STATION_NAME = '" + scdevice.STATION_NAME + "' And MODEL_NAME = '" + scdevice.MODEL_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScDeviceModeStatus(SaaScDevice scdevice)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_DEVICE Set DEVICESTATUS = '" + scdevice.DEVICESTATUS + "' Where SETNO = '" + scdevice.SETNO + "' And STATION_NAME = '" + scdevice.STATION_NAME + "' And MODEL_NAME = '" + scdevice.MODEL_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScDeviceAutoReject(SaaScDevice scdevice)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_DEVICE Set AUTOREJECT = '" + scdevice.AUTOREJECT + "' Where SETNO = '" + scdevice.SETNO + "' And STATION_NAME = '" + scdevice.STATION_NAME + "' And MODEL_NAME = '" + scdevice.MODEL_NAME + "'");
                SAA_Database.LogMessage("Update SC_DEVICE Set AUTOREJECT = '" + scdevice.AUTOREJECT + "' Where SETNO = '" + scdevice.SETNO + "' And STATION_NAME = '" + scdevice.STATION_NAME + "' And MODEL_NAME = '" + scdevice.MODEL_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScCommandTask(SaaScCommandTask commandtask)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Update SC_COMMAND_TASK Set RESULT = '" + commandtask.RESULT + "' Where SETNO = '" + commandtask.SETNO + "' And MODEL_NAME = '" + commandtask.MODEL_NAME + "' And CARRIERID = '" + commandtask.CARRIERID + "'");
                if (sqlreport) break;
                Thread.Sleep(500);
            }
        }

        public void UpdScCommandTask()
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen($"Update SC_COMMAND_TASK Set RESULT = null");
                if (sqlreport) break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLiftCarrierInfoCarrieridUpdate(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_CARRIER_INFO Set CARRIERID_UPDATE = '" + LiftCarrierInfo.CARRIERID_UPDATE + "' Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATIOM_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERID = '" + LiftCarrierInfo.CARRIERID + "'");
                if (sqlreport) break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLiftCarrierInfoCallShuttle(SaaScLiftCarrierInfo LiftCarrierInfo, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_CARRIER_INFO Set CALL_SHUTTLE = '" + LiftCarrierInfo.CALL_SHUTTLE + "' Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERID = '" + carrierid + "'");
                SAA_Database.LogMessage("Update SC_LIFT_CARRIER_INFO Set CALL_SHUTTLE = '" + LiftCarrierInfo.CALL_SHUTTLE + "' Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERID = '" + carrierid + "'");
                if (sqlreport) break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLiftCarrierInfoCarrierid(SaaScLiftCarrierInfo LiftCarrierInfo, string carrierid)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_CARRIER_INFO Set CARRIERID = '" + LiftCarrierInfo.CARRIERID + "', CARRIERID_UPDATE = '" + LiftCarrierInfo.CARRIERID_UPDATE + "' Where CARRIERID = '" + carrierid + "' And SETNO = '" + LiftCarrierInfo.SETNO + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "'");
                SAA_Database.LogMessage("Update SC_LIFT_CARRIER_INFO Set CARRIERID = '" + LiftCarrierInfo.CARRIERID + "', CARRIERID_UPDATE = '" + LiftCarrierInfo.CARRIERID_UPDATE + "' Where CARRIERID = '" + carrierid + "' And SETNO = '" + LiftCarrierInfo.SETNO + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScLiftCarrierInfoCarrieridReadPlc(SaaScLiftCarrierInfo LiftCarrierInfo, string readplc)
        {
            while (true)
            {
                bool sqlreport = SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_CARRIER_INFO Set READPLC = '" + readplc + "' Where CARRIERID = '" + LiftCarrierInfo.CARRIERID + "' And SETNO = '" + LiftCarrierInfo.SETNO + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "'");
                SAA_Database.LogMessage("Update SC_LIFT_CARRIER_INFO Set READPLC = '" + readplc + "' Where CARRIERID = '" + LiftCarrierInfo.CARRIERID + "' And SETNO = '" + LiftCarrierInfo.SETNO + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "'");
                if (sqlreport)
                    break;
                Thread.Sleep(500);
            }
        }

        public void UpdScEquipmentCarrierInfo(SaaEquipmentCarrierInfo equipmentcarrierinfo)
        {
            SaaSql.WriteSqlByAutoOpen("Update SC_EQUIPMENT_CARRIER_INFO Set REJECT_CODE ='" + equipmentcarrierinfo.REJECT_CODE + "', REJECT_MESSAGE = '" + equipmentcarrierinfo.REJECT_MESSAGE + "' Where CARRIERID = '" + equipmentcarrierinfo.CARRIERID + "' And STATIOM_NAME = '" + equipmentcarrierinfo.STATIOM_NAME + "'");
            SAA_Database.LogMessage("Update SC_EQUIPMENT_CARRIER_INFO Set REJECT_CODE ='" + equipmentcarrierinfo.REJECT_CODE + "', REJECT_MESSAGE = '" + equipmentcarrierinfo.REJECT_MESSAGE + "' Where CARRIERID = '" + equipmentcarrierinfo.CARRIERID + "' And STATIOM_NAME = '" + equipmentcarrierinfo.STATIOM_NAME + "'");
        }

        public void UpdScLifte84PlcStatus(SaaEquipmentE84PlcAttributes lifte84plc)
        {
            SaaSql.WriteSqlByAutoOpen("Update SC_LIFT_E84PLC_STATUS Set TASKDATETIME = '" + lifte84plc.TASKDATETIME + "' ,CARRIERID = '" + lifte84plc.CARRIERID + "', CS_0 = '" + lifte84plc.CS_0 + "', Valid = '" + lifte84plc.Valid + "', TR_REQ = '" + lifte84plc.TR_REQ + "', Busy = '" + lifte84plc.Busy + "', Complete = '" + lifte84plc.Complete + "', [Continue] = '" + lifte84plc.Continue + "', [SELECT] = '" + lifte84plc.SELECT + "', AM_AVBL = '" + lifte84plc.AM_AVBL + "' Where STATION_NAME = '" + lifte84plc.STATION_NAME + "'");
            SAA_Database.LogMessage("Update SC_LIFT_E84PLC_STATUS Set TASKDATETIME = '" + lifte84plc.TASKDATETIME + "' ,CARRIERID = '" + lifte84plc.CARRIERID + "', CS_0 = '" + lifte84plc.CS_0 + "', Valid = '" + lifte84plc.Valid + "', TR_REQ = '" + lifte84plc.TR_REQ + "', Busy = '" + lifte84plc.Busy + "', Complete = '" + lifte84plc.Complete + "', [Continue] = '" + lifte84plc.Continue + "', [SELECT] = '" + lifte84plc.SELECT + "', AM_AVBL = '" + lifte84plc.AM_AVBL + "' Where STATION_NAME = '" + lifte84plc.STATION_NAME + "'");
        }

        public void UpdScScPurchase(int setno, string statiomname, string carrierid, string replyresult)
        {
            SaaSql.WriteSqlByAutoOpen("Update SC_PURCHASE Set REPLYRESULT = '" + replyresult + "' Where SETNO = '" + setno + "' And STATION_NAME = '" + statiomname + "' And CARRIERID = '" + carrierid + "'");
            SAA_Database.LogMessage($"【SQL語法】【UpdScScPurchase】Update SC_PURCHASE Set REPLYRESULT = '" + replyresult + "' Where SETNO = '" + setno + "' And STATION_NAME = '" + statiomname + "' And CARRIERID = '" + carrierid + "'");
        }

        public void UpdScAlarmCurrent(SaaScAlarmCurrent AlarmCurrent)
        {
            SaaSql.WriteSqlByAutoOpen("Update SC_ALARM_CURRENT Set ALARM_STATUS = '" + AlarmCurrent.ALARM_STATUS + "', END_TIME = '" + AlarmCurrent.END_TME + "', REPORT_STATUS = '" + AlarmCurrent.REPORT_STATUS + "' Where STATION_NAME = '" + AlarmCurrent.STATION_NAME + "' And ALARM_CODE = '" + AlarmCurrent.ALARM_CODE + "'");
        }

        /*===============================查詢=======================================*/

        /// <summary>
        /// 讀取機台IP
        /// </summary>
        /// <param name="setno">設備編號</param>
        /// <param name="model_name">設備名稱</param>
        /// <returns></returns>
        public DataTable GetScDevice(int setno, string model_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_DEVICE Where SETNO = '" + setno + "' And MODEL_NAME= '" + model_name + "'")!;
        }

        public DataTable GetScEcaIoMapping(int setno, string model_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From ECA_IO_MAPPING Where SETNO = '" + setno + "' And MODEL_NAME= '" + model_name + "'")!;
        }

        #region [===查詢LIFT E84 PC狀態===]
        public DataTable GetScLiftE84PcStatsus(string station_name)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_LIFT_E84PC_STATSUS Where STATION_NAME  = '" + station_name + "'")!;
                if (sqlreport != null) break;
                Thread.Sleep(50);
            }
            return sqlreport;
        }
        #endregion

        #region [===讀取資料點位===]
        /// <summary>
        /// 讀取資料點位
        /// </summary>
        /// <param name="ecaIoMapping"></param>
        /// <returns></returns>
        public DataTable GetEcaIoMapping(SaaEcaIoMapping ecaIoMapping)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From ECA_IO_MAPPING Where SETNO = '" + ecaIoMapping.SETNO + "' And MODEL_NAME = '" + ecaIoMapping.MODEL_NAME + "' And EVENTFUNCTION = '" + ecaIoMapping.EVENTFUNCTION + "'")!;
        }
        #endregion

        #region [===讀取資料點位(已群組方式)===]
        /// <summary>
        /// 讀取資料點位(已群組方式)
        /// </summary>
        /// <param name="ecaIoMapping"></param>
        /// <returns></returns>
        public DataTable GetEcaIoMappingGroup(SaaEcaIoMapping ecaIoMapping)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From ECA_IO_MAPPING Where SETNO = '" + ecaIoMapping.SETNO + "' And MODEL_NAME = '" + ecaIoMapping.MODEL_NAME + "' And SCGROUP = '" + ecaIoMapping.SCGROUP + "' And SYMBOL = '" + ecaIoMapping.SYMBOL + "' Order By Offset")!;
        }
        #endregion

        public DataTable GetScCommon(SaaScCommon common)
        {
            return SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMON Where SETNO = '" + common.SETNO + "' And MODEL_NAME = '" + common.MODEL_NAME + "' And  ITEM_NAME = '" + common.ITEM_NAME + "'")!;
        }

        public DataTable GetScPurchase(string station_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_PURCHASE Where STATION_NAME = '" + station_name + "' And (REPLYRESULT is not null or REPLYRESULT <>'')")!;
        }

        public DataTable GetScPurchaseTask(string station_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_PURCHASE Where STATION_NAME = '" + station_name + "'")!;
        }

        public DataTable GetPurchase(string carrierid)
        {
            return SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_PURCHASE WHERE CARRIERID = '" + carrierid + "' AND ReplyResult is not NULL")!;
        }

        public DataTable GetPurchasePgvOut(string carrierid)
        {
            return SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_PURCHASE WHERE CARRIERID = '" + carrierid + "'")!;
        }

        public DataTable GetScLifte84PC(string station_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LIFT_E84PC Where STATION_NAME = '" + station_name + "' And (RESULT is null or RESULT = 'R') Order By TASKDATETIME")!;
        }

        public DataTable GetScLocationSetting(SaaScLocationSetting locationsetting)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And LOCATIONID = '" + locationsetting.LOCATIONID + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "'")!;
                SAA_Database.LogMessage($"【查詢SQL方法】【查詢結果:{sqlreport}】Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And LOCATIONID = '" + locationsetting.LOCATIONID + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "'");
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLocationSettingMode(SaaScLocationSetting locationsetting)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And LOCATIONTYPE = '" + locationsetting.LOCATIONTYPE + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "' And LOCATIONMODE = '" + locationsetting.LOCATIONMODE + "' And LOCATIONTYPE = '" + locationsetting.LOCATIONTYPE + "' Order By PUTTIME")!;
        }

        public DataTable GetScLocationSettingCarrierId(SaaScLocationSetting locationsetting)
        {
            //SAA_Database.LogMessage("Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And CARRIERID = '" + locationsetting.CARRIERID + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "' And LOCATIONTYPE = 'Shelf' ");
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And CARRIERID = '" + locationsetting.CARRIERID + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "' And LOCATIONTYPE = 'Shelf'")!;
        }

        public DataTable GetScLocationSettingCarrierIdNotNull(string statiom_name)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING Where STATIOM_NAME ='" + statiom_name + "' And LOCATIONTYPE = 'Shelf' And CARRIERID is not null and CARRIERID <> ''")!;
        }


        public DataTable GetScLocationsetting(int setno, string modelname, string statiomname)
        {
            return SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_LOCATIONSETTING Where SETNO = '" + setno + "' And MODEL_NAME = '" + modelname + "' And STATIOM_NAME = '" + statiomname + "'")!;
        }

        public DataTable GetScLocationSetting(int setno, string modelname, string statiomname, string locationid)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING Where SETNO = '" + setno + "' And MODEL_NAME = '" + modelname + "' And STATIOM_NAME = '" + statiomname + "' And LOCATIONID = '" + locationid + "'")!;
        }

        #region [===查詢站點===]
        /// <summary>
        /// 查詢站點
        /// </summary>
        public DataTable GetScLocationsetting()
        {
            return SaaSql.QuerySqlByAutoOpen("Select DISTINCT STATIOM_NAME From SC_LOCATIONSETTING")!;
        }
        #endregion

        public DataTable GetScLocationsettingLocationid(string setno, string model_name, string locationid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_LOCATIONSETTING Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And LOCATIONID = '" + locationid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLocationsettingLocationidStation(string setno, string statiom_name, string locationid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_LOCATIONSETTING Where SETNO = '" + setno + "' And STATIOM_NAME = '" + statiom_name + "' And LOCATIONID = '" + locationid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLocationsettingLocationidShelf(string statiom_name, string locationid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_LOCATIONSETTING Where STATIOM_NAME = '" + statiom_name + "' And LOCATIONID = '" + locationid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTask(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And  STATION_NAME = '" + stationname + "' And (RESULT is Null or RESULT = '' ) Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTask(string setno, string model_name, string stationname, string carrierid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And  STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTaskResultCount(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And  STATION_NAME = '" + stationname + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTask(string stationname, string carrierid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where STATION_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTask(string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where STATION_NAME = '" + stationname + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTaskResult(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And  STATION_NAME = '" + stationname + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScCommandTaskResult(string setno, string model_name, string stationname, string locationput)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_COMMAND_TASK Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And  STATION_NAME = '" + stationname + "' And LOCATIONPUT = '" + locationput + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScDevice(int setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_DEVICE Where SETNO = '" + setno + "' And  MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetLiftCarrierInfo(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen($"Select * From SC_LIFT_CARRIER_INFO Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATIOM_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERID = '" + LiftCarrierInfo.CARRIERID + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetLiftCarrierInfoCarrier(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen($"Select * From SC_LIFT_CARRIER_INFO Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And CARRIERID = '" + LiftCarrierInfo.CARRIERID + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetLiftCarrierInfoEmpty(SaaScLiftCarrierInfo LiftCarrierInfo)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen($"Select * From SC_LIFT_CARRIER_INFO Where SETNO = '" + LiftCarrierInfo.SETNO + "' And MODEL_NAME = '" + LiftCarrierInfo.MODEL_NAME + "' And STATION_NAME = '" + LiftCarrierInfo.STATION_NAME + "' And CARRIERTYPE = '" + LiftCarrierInfo.CARRIERTYPE + "' And CALL_SHUTTLE is Null And CARRIERID_UPDATE is Not Null")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetLiftCarrierInfoEmpty(string STATION_NAME, string CARRIERTYPE)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen($"Select * From SC_LIFT_CARRIER_INFO Where STATION_NAME = '" + STATION_NAME + "' And CARRIERTYPE = '" + CARRIERTYPE + "' And CALL_SHUTTLE is Null And CARRIERID_UPDATE is Not Null")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScDeviceModelName(int setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_DEVICE Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScPurchase(SaaScPurchase purchase)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_PURCHASE Where SETNO = '" + purchase.SETNO + "' And MODEL_NAME = '" + purchase.MODEL_NAME + "' And STATION_NAME = '" + purchase.STATION_NAME + "' And CARRIERID = '" + purchase.CARRIERID + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLiftCarrierInfoEmpty(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_LIFT_CARRIER_INFO_EMPTY Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLiftCarrierInfoMaterial(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_LIFT_CARRIER_INFO_MATERIAL Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLiftCarrierInfoReject(string setno, string model_name, string stationname)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_LIFT_CARRIER_INFO_REJECT Where SETNO = '" + setno + "' And MODEL_NAME = '" + model_name + "' And STATION_NAME = '" + stationname + "' Order By TASKDATETIME")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScEquipmentCarrierInfo(string stationname, string carrierid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("Select * From SC_EQUIPMENT_CARRIER_INFO Where STATIOM_NAME = '" + stationname + "' And CARRIERID = '" + carrierid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScLiftCarrierInfoReject(string statiom_name, string carrierid)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_LIFT_CARRIER_INFO_REJECT Where STATION_NAME = '" + statiom_name + "' And CARRIERID = '" + carrierid + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScAlarmCurrent(SaaScAlarmCurrent AlarmCurrent)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_ALARM_CURRENT Where STATION_NAME = '" + AlarmCurrent.STATION_NAME + "' And ALARM_CODE = '" + AlarmCurrent.ALARM_CODE + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScAlarmCurrentStation(string station_name)
        {
            DataTable sqlreport;
            while (true)
            {
                sqlreport = SaaSql.QuerySqlByAutoOpen("SELECT * FROM SC_ALARM_CURRENT Where STATION_NAME = '" + station_name + "'")!;
                if (sqlreport != null) break;
            }
            return sqlreport;
        }

        public DataTable GetScAlarmCurrentNnd(SaaScAlarmCurrent AlarmCurrent)
        {
            SAA_Database.LogMessage("Select * From SC_ALARM_CURRENT Where STATION_NAME = '" + AlarmCurrent.STATION_NAME + "' And ALARM_CODE= '" + AlarmCurrent.ALARM_CODE + "' And ALARM_STATUS = '" + AlarmCurrent.ALARM_STATUS + "' And REPORT_STATUS = '" + AlarmCurrent.REPORT_STATUS + "'");
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_ALARM_CURRENT Where STATION_NAME = '" + AlarmCurrent.STATION_NAME + "' And ALARM_CODE= '" + AlarmCurrent.ALARM_CODE + "' And ALARM_STATUS = '" + AlarmCurrent.ALARM_STATUS + "' And REPORT_STATUS = '" + AlarmCurrent.REPORT_STATUS + "'")!;
        }

        public DataTable GetScTransportrEquirementMaterialCarrierId(string carrierid)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_TRANSPORTR_EQUIREMENT_MATERIAL Where  CARRIERID = '" + carrierid + "'")!;
        }

        public DataTable GetScRejectListPlc(string remoterejectcode)
        {
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_REJECT_LIST_PLC Where  REMOTE_REJECT_CODE = '" + remoterejectcode + "'")!;
        }

        public DataTable GetScLocationSettingLocationType(string statiom_name, string locationtype)
        {
            //SAA_Database.LogMessage("Select * From SC_LOCATIONSETTING Where SETNO = '" + locationsetting.SETNO + "' And MODEL_NAME = '" + locationsetting.MODEL_NAME + "' And CARRIERID = '" + locationsetting.CARRIERID + "' And STATIOM_NAME ='" + locationsetting.STATIOM_NAME + "' And LOCATIONTYPE = 'Shelf' ");
            return SaaSql.QuerySqlByAutoOpen("Select * From SC_LOCATIONSETTING_iLIS Where STATIOM_NAME ='" + statiom_name + "' And LOCATIONTYPE = '" + locationtype + "' And CARRIERID = ''")!;
        }
    }
}
