using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes
{
    public class SaaEquipmentPlcAttributes
    {
        /// <summary>
        /// 設備編號
        /// </summary>
        public int SaaSetNo { get; set; } = 0;

        /// <summary>
        /// 設備機型
        /// </summary>
        public string SaaModelName { get; set; } = string.Empty;

        /// <summary>
        /// 設備站點
        /// </summary>
        public string EquipmentStation { get; set; } = string.Empty;

        /// <summary>
        /// PLC IP位置
        /// </summary>
        public string ActHostAddress { get; set; } = string.Empty;

        /// <summary>
        /// PLC Portt位置
        /// </summary>
        public int ActPort { get; set; } = 0;

        /// <summary>
        /// 連線超時時間
        /// </summary>
        public int ConnTimeout { get; set; } = 0;

        /// <summary>
        /// 連線超時時間
        /// </summary>
        public int RecvTimeout { get; set; } = 0;

        /// <summary>
        /// 傳送超時時間
        /// </summary>
        public int SendTimeout { get; set; } = 0;

        /// <summary>
        /// FX5U 的 CPU X、Y 位置是 8 進位，需要把這個設定為 True
        /// </summary>
        public int IsOctalXY { get; set; } = 0;
    }
}
