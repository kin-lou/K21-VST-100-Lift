using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentAttributes
{
    public class SaaEquipmentE84Plc
    {
        /// <summary>
        /// 指定傳輸Port1
        /// </summary>
        public int CS_0 { get; set; }

        /// <summary>
        /// 是否可交握
        /// </summary>
        public int Valid { get; set; }

        /// <summary>
        /// 傳輸請求
        /// </summary>
        public int TR_REQ { get; set; }

        /// <summary>
        /// 正在進行傳輸
        /// </summary>
        public int Busy { get; set; }

        /// <summary>
        /// 完成傳輸
        /// </summary>
        public int Complete { get; set; }

        /// <summary>
        /// 繼續傳輸
        /// </summary>
        public int Continue { get; set; }

        /// <summary>
        /// 選擇
        /// </summary>
        public int SELECT { get; set; }

        /// <summary>
        /// 運輸(搬運)設備可使用
        /// </summary>
        public int AM_AVBL { get; set; }
    }
}
