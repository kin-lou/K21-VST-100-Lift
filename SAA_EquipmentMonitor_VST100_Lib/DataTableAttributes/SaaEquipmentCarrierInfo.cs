using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaEquipmentCarrierInfo
    {
        /// <summary>
        /// 設備編號
        /// </summary>
        public int SETNO { get; set; }

        /// <summary>
        /// 設備名稱
        /// </summary>
        public string MODEL_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 設備站點
        /// </summary>
        public string STATIOM_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣ID
        /// </summary>
        public string CARRIERID { get; set; } = string.Empty;

        /// <summary>
        /// 批號
        /// </summary>
        public string PARTNO { get; set; } = string.Empty;

        /// <summary>
        /// 載體類型
        /// </summary>
        public string CARRIERTYOE { get; set; } = string.Empty;

        /// <summary>
        /// 旋轉資訊
        /// </summary>
        public string ROTFLAG { get; set; } = string.Empty;

        /// <summary>
        /// 翻轉資訊
        /// </summary>
        public string FLIPFLAG { get; set; } = string.Empty;

        /// <summary>
        /// 貨批站點
        /// </summary>
        public string OPER { get; set; } = string.Empty;

        /// <summary>
        /// 貨批參數
        /// </summary>
        public string RECIPE { get; set; } = string.Empty;

        /// <summary>
        /// 原始位置
        /// </summary>
        public string ORIGIN { get; set; } = string.Empty;

        /// <summary>
        /// 現在位置
        /// </summary>
        public string DESTINATION { get; set; } = string.Empty;

        /// <summary>
        /// 貨批效期
        /// </summary>
        public string QTIME { get; set; } = string.Empty;

        /// <summary>
        /// 盒效期
        /// </summary>
        public string CYCLETIME { get; set; } = string.Empty;

        /// <summary>   
        /// 退盒編碼
        /// </summary>
        public string REJECT_CODE { get; set; } = string.Empty;

        /// <summary>
        /// 退盒資訊
        /// </summary>
        public string REJECT_MESSAGE { get; set; } = string.Empty;

        /// <summary>
        /// 卡匣屬性
        /// </summary>
        public string CARRIERSTATE { get; set; } = string.Empty;

        public string DESTINATIONTYPE { get; set; } = string.Empty;
    }
}
