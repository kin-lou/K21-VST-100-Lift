using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentAttributes
{
    public class SaaEquipmentAttributes
    {
        /// <summary>
        /// 資料類別(PLC)
        /// </summary>
        public string DataTrack { get; set; }=string.Empty;

        /// <summary>
        /// 詢問類型(PLC)
        /// </summary>
        public string DataReply { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-本地(PLC)
        /// </summary>
        public string DataLocal { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-來源(PLC)
        /// </summary>
        public string DataRemote { get; set; } = string.Empty;

        /// <summary>
        /// 在籍資料
        /// </summary>
        public string DataEnrollmentInfo { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣ID
        /// </summary>
        public string DataCarrierId { get; set; } = string.Empty;

        /// <summary>
        /// 資料類別(PC)
        /// </summary>
        public string DataTrackAck { get; set; } = string.Empty;

        /// <summary>
        /// 答覆類型(PC)
        /// </summary>
        public string DataReplyAck { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-本地(PC)
        /// </summary>
        public string DataLocalAck { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-來源(PC)
        /// </summary>
        public string DataRemoteAck { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣ID(PC)
        /// </summary>
        public string DataCarrrierIdAck { get; set; } = string.Empty;

        /// <summary>
        /// 答覆資訊(PC)
        /// </summary>
        public string DataReplyInfo { get; set; } = string.Empty;
    }
}
