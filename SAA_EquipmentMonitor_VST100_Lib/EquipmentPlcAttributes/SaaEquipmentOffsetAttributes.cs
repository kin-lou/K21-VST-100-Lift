using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes
{
    public class SaaEquipmentOffsetAttributes
    {
        /// <summary>
        /// E84 PLC 要求點位
        /// </summary>
        public int[]? PlcE84Request { get; set; } = new int[864];

        public int[]? PcE84Request { get; set; } = new int[864];

        public int[]? PlcAlarmList { get; set; } = new int[21];

        public int[]? PcAlarmList { get; set; }=new int[21];

        /// <summary>
        /// PLC 上報點位
        /// </summary>
        public int[]? PlcRequest { get; set; }

        /// <summary>
        /// PC 上報點位
        /// </summary>
        public int[]? PcRequest { get; set; }

        public List<string> PlcAlarmListCount = new List<string>();

        public List<string> PcAlarmListCount = new List<string>();

        /// <summary>
        /// 心跳包(DB)
        /// </summary>
        public string SaaDB_HeartBit { get; set; } = string.Empty;

        /// <summary>
        /// 心跳包(值)
        /// </summary>
        public string SaaValue_HeartBit { get; set; } = string.Empty;

        /// <summary>
        /// 時間(DB)
        /// </summary>
        public string SaaDB_DateTime { get; set; } = string.Empty;

        /// <summary>
        /// 時間(值)
        /// </summary>
        public string SaaValue_DateTime { get; set; } = string.Empty;

        public int[]? PlcCommandOffset { get; set; }
    }
}
