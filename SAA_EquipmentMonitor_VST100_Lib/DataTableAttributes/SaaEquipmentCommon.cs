using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaEquipmentCommon
    {
        public int GroupCount { get; set; }

        public int DataTrack1 { get; set; }

        public int DataReply1 { get; set; }

        public int DataLocal1 { get; set; }

        public int DataRemote1 { get; set; }

        public int DataCarrierId1 { get; set; }

        public int DataTrack2 { get; set; }

        public int DataReply2 { get; set; }

        public int DataLocal2 { get; set; }

        public int DataRemote2 { get; set; }

        public int DataCarrierId2 { get; set; }

        /// <summary>
        /// LIFT物料追蹤-搬移
        /// </summary>
        public string Move { get; set; }=string.Empty;

        /// <summary>
        /// LIFT物料追蹤-建立
        /// </summary>
        public string Establish { get; set; } = string.Empty;

        /// <summary>
        /// LIFT物料追蹤-清除
        /// </summary>
        public string Clear { get; set; } = string.Empty;

        /// <summary>
        /// LIFT物料追蹤-詢問
        /// </summary>
        public string Ask { get; set; } = string.Empty;

        /// <summary>
        /// LIFT物料追蹤-更新
        /// </summary>
        public string Update { get; set; } = string.Empty;

        /// <summary>
        /// LIFT物料追蹤-回覆
        /// </summary>
        public string Reply { get; set; } = string.Empty;

        /// <summary>
        /// LIFT資料同步-有帳
        /// </summary>
        public string Have { get; set; } = string.Empty;

        /// <summary>
        /// LIFT資料同步-無帳
        /// </summary>
        public string None { get; set; } = string.Empty;

        /// <summary>
        /// ERROR讀取
        /// </summary>
        public string ReaderError { get; set; } = string.Empty;
    }
}
