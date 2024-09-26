using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib
{
    public class SAA_DatabaseEnum
    {
        public enum LogType
        {
            /// <summary>
            /// 成功
            /// </summary>
            Normal = 0,

            /// <summary>
            /// 警告
            /// </summary>
            Warnning = 1,

            /// <summary>
            /// 失敗
            /// </summary>
            Error = 2
        }

        public enum SaaOffsetName
        {
            /// <summary>
            /// 資料比對錯誤(上報確認資料與上報資料比對相異)
            /// </summary>
            DataConfirmError,

            /// <summary>
            /// 資料組數(PLC)
            /// </summary>
            DataCount,

            /// <summary>
            /// 資料組數(PC)
            /// </summary>
            DataCountAck,

            /// <summary>
            /// 寫入心跳
            /// </summary>
            HearBit,

            /// <summary>
            /// 資料上報要求
            /// </summary>
            Request,

            /// <summary>
            /// 資料上報確認
            /// </summary>
            RequestAck,

            /// <summary>
            /// 資料已全數上報完成
            /// </summary>
            RequestAll,

            /// <summary>
            /// 資訊答覆要求
            /// </summary>
            RequestAllAck,

            /// <summary>
            /// 資訊答覆確認
            /// </summary>
            RequestConfirm,

            /// <summary>
            /// 模式不支援的動作(非物料追蹤模式下接收到答覆資訊)
            /// </summary>
            RequestModeError,

            /// <summary>
            /// 目前模式為資料同步
            /// </summary>
            Synchronize,

            /// <summary>
            /// 指定模式為資料同步
            /// </summary>
            SynchronizeAck,

            /// <summary>
            /// 寫入時間
            /// </summary>
            DateTime,

            /// <summary>
            /// 目前模式為物料追蹤
            /// </summary>
            Track,

            /// <summary>
            /// 指定模式為物料追蹤
            /// </summary>
            TrackAck,

            DataRGVE84,

            RGV,
            PUGVOUT,
            DKIN,

            DeviceStatus,
        }

        public enum SaaEcaIoMapping
        {
            SETNO,

            MODEL_NAME,

            SCGROUP,

            INVENTORYMODE,

            EVENTFUNCTION,

            SYMBOL,

            OFFSET,

            OFFSET_ID,

            OFFSET_TYPE,

            REMARK
        }

        public enum EquipmentOffset
        {
            GroupCount,
            DataTrack1,
            DataReply1,
            DataLocal1,
            DataRemote1,
            DataCarrierId1,
            DataTrack2,
            DataReply2,
            DataLocal2,
            DataRemote2,
            DataCarrierId2,

            /// <summary>
            /// LIFT物料追蹤-搬移
            /// </summary>
            Move,

            /// <summary>
            /// LIFT物料追蹤-更新
            /// </summary>
            Update,

            /// <summary>
            /// LIFT物料追蹤-建立
            /// </summary>
            Establish,

            /// <summary>
            /// LIFT物料追蹤-清除
            /// </summary>
            Clear,

            /// <summary>
            /// LIFT物料追蹤-詢問
            /// </summary>
            Ask,

            /// <summary>
            /// LIFT物料追蹤-回覆
            /// </summary>
            Reply,

            /// <summary>
            /// LIFT資料同步-有帳
            /// </summary>
            Have,

            /// <summary>
            /// LIFT資料同步-無帳
            /// </summary>
            None,

            ReaderError,

            /// <summary>
            /// 傳送轉譯程式Server IP位置
            /// </summary>
            WebApiServerIP,
        }

        public enum SC_COMMON
        {
            SETNO,
            MODEL_NAME,
            ITEM_NAME,
            ITEM_VALUE,
            VALUECOMMENT
        }

        public enum SaaOffsetGroup
        {
            DataTrack,

            DataReply,

            DataLocal,

            DataRemote,

            DataEnrollmentInfo,

            DataCarrierId,

            DataTrackAck,

            DataReplyAck,

            DataLocalAck,

            DataRemoteAck,

            DataCarrrierIdAck,

            DataReplyInfo
        }

        public enum EquipmentType
        {
            PGVOUT,

            DKIN,
        }

        public enum PlcEventfunction
        {
            RGVCARRIERID,

            RGVE84,

            RGVE84Ack,
        }

        public enum SC_COMMAND_TASK
        {
            TASKDATETIME,
            SETNO,
            MODEL_NAME,
            STATION_NAME,
            COMMANDID,
            CARRIERID,
            LOCATIONTAKE,
            LOCATIONPUT,
            RESULT
        }

        #region [===是否傳送列舉===]
        /// <summary>
        /// 是否傳送列舉
        /// </summary>
        public enum SendFlag
        {
            /// <summary>
            /// 傳送
            /// </summary>
            Y,

            /// <summary>
            /// 未傳送
            /// </summary>
            N,

            /// <summary>
            /// 取消
            /// </summary>
            X,
        }
        #endregion

        public enum SaaSendReply
        {
            /// <summary>
            /// 入庫
            /// </summary>
            Y,

            /// <summary>
            /// 退REJECT
            /// </summary>
            N,
        }

        public enum ConnectStatus
        {
            /// <summary>
            /// 連線成功
            /// </summary>
            Y,

            /// <summary>
            /// 連線失敗
            /// </summary>
            N,
        }

        public enum DeviceStatus
        {
            /// <summary>
            /// 自動
            /// </summary>
            Y,

            /// <summary>
            /// 手動
            /// </summary>
            N,
        }

        public enum SaaLiftCommandName
        {
            /// <summary>
            /// 設備狀態
            /// </summary>
            EquipmentStatus,

            EquipmnetHardwareInfo,
            EquipmentLiftE84PlcHandshakeInfo,
            EquipmentLiftAlarmList,
        }

        public enum SendWebApiCommandName
        {
            GetLiftMessage,

            SaaEquipmentMonitorE84PlcSendStart,
        }

        public enum EquipmentStatusCommand
        {
            Statiom_Name,

            CommandName,
        }

        public enum CarrierState
        {
            Unknow,
            Empty,
            Material,
        }

        public enum E84DataStatus
        {
            True
        }

        public enum DestinationType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow,

            /// <summary>
            /// 製程區
            /// </summary>
            EQP,

            /// <summary>
            ///暫存區 
            /// </summary>
            Buffer,

            /// <summary>
            /// 退盒區
            /// </summary>
            Reject,
        }

        public enum CarrierTyoe
        {
            Normal,
        }

        public enum SaaScLiftE84iLisPlc
        {
            TASKDATETIME,
            STATION_NAME,
            SHUTTLEID,
            COMMANDID,
            CARRIERID,
            Mode,
            VALID,
            CS_0,
            CS_1,
            TR_REQ,
            L_REQ,
            U_REQ,
            READY,
            BUSY,
            COMPT,
            CONT,
            HOA_VBL,
            ES,
            VA,
            AM_AVBL,
            VS_0,
            VS_1,
            SELECT,
            RESULT,
        }

        public enum SC_LOCATIONSETTING
        {
            SETNO,
            MODEL_NAME,
            STATIOM_NAME,
            LOCATIONID,
            HOSTID,
            CARRIERID,
            PARTNO,
            BANK,
            BAY,
            LV,
            LOCATIONSTATUS,
            LOCATIONMODE,
            LOCATIONTYPE,
            INVENTORYFULL,
            ZONEID,
            LOCATIONPRIORITIZ,
            PUTTIME,
        }

        public enum SC_EQUIPMENT_CARRIER_INFO
        {
            SETNO,
            MODEL_NAME,
            STATIOM_NAME,
            CARRIERID,
            PARTNO,
            REMOTE,
            CARRIERTYOE,
            ROTFLAG,
            FLIPFLAG,
            OPER,
            RECIPE,
            ORIGIN,
            DESTINATION,
            CARRIERSTATE,
            DESTINATIONTYPE,
            QTIME,
            CYCLETIME,
            REJECT_CODE,
            REJECT_MESSAGE,
            CARRIERFLAG,
        }

        public enum SC_REJECT_LIST_PLC
        {
            PLC_REJECT_CODE,
            REMOTE_REJECT_CODE,
            REMOTE_REJECT_MSG,
            COMMENT,
        }

        public enum LOCATIONTYPE
        {
            Shelf,

            Shelf_Global,
        }
    }
}
