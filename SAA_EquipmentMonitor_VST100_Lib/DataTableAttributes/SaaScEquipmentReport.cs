using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScEquipmentReport
    {
        /// <summary>
        /// 任務時間
        /// </summary>
        public string TASKDATETIME { get; set; } = string.Empty;

        /// <summary>
        /// 機台編號
        /// </summary>
        public int SETNO { get; set; }

        /// <summary>
        /// 機型名稱
        /// </summary>
        public string MODEL_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 站點名稱
        /// </summary>
        public string STATION_NAEM { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣編號
        /// </summary>
        public string CARRIERID { get; set; } = string.Empty;

        /// <summary>
        /// 機台追蹤
        /// </summary>
        public string REPORE_DATATRACK { get; set; } = string.Empty;

        /// <summary>
        /// 來源報告
        /// </summary>
        public string REPORE_DATAREMOTE { get; set; } = string.Empty;

        /// <summary>
        /// 本地報告
        /// </summary>
        public string REPORE_DATALOCAL { get; set; } = string.Empty;

        /// <summary>
        /// 是否傳送
        /// </summary>
        public string SENDFLAG { get; set; } = string.Empty;

        public string REPORE_SEND { get; set; } = string.Empty;
    }
}
