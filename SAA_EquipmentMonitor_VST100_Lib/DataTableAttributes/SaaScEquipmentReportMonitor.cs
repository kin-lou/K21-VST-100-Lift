using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScEquipmentReportMonitor
    {
        /// <summary>
        /// 機型編號
        /// </summary>
        public int SETNO { get; set; } = 0;

        /// <summary>
        /// 機型名稱
        /// </summary>
        public string MODEL_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 站點名稱
        /// </summary>
        public string STATION_NAEM { get; set; }=string.Empty;

        /// <summary>
        /// 上報編號
        /// </summary>
        public string REPORTID { get; set; } = string.Empty;

        /// <summary>
        /// 上報時間(yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        public string REPORT_TIME { get; set; } = string.Empty;

        /// <summary>
        /// 資料組數
        /// </summary>
        public int REPORE_DATACOUNT { get; set; } = 0;

        /// <summary>
        /// 第幾組上報
        /// </summary>
        public int GROUP_NO { get; set; } = 0;

        /// <summary>
        /// 資料類別(1=物料追蹤-搬移, 4=物料追蹤-建立, 8=物料追蹤-清除, 16=物料追蹤-詢問, 64=資料同步-有帳, 128=資料同步-無帳)
        /// </summary>
        public string REPORE_DATATRACK { get; set; } = string.Empty;

        /// <summary>
        /// 詢問類型
        /// </summary>
        public string REPORE_DATAREPLY { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-本地
        /// </summary>
        public string REPORE_DATALOCAL { get; set; } = string.Empty;

        /// <summary>
        /// 位置名稱-來源
        /// </summary>
        public string REPORE_DATAREMOTE { get; set; } = string.Empty;

        /// <summary>
        /// 在籍資料
        /// </summary>
        public string REPORE_DATAENROLLMENTINFO { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣號碼
        /// </summary>
        public string CARRIERID { get; set; } = string.Empty;
    }
}
