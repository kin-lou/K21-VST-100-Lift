using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.Attributes
{
    public class SaaConfigAttributes
    {
        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string SaaDataBase { get; set; } = string.Empty;

        /// <summary>
        /// 資料庫IP
        /// </summary>
        public string SaaDataBaseIP { get; set; } = string.Empty;

        /// <summary>
        /// 資料庫使用者名稱
        /// </summary>
        public string SaaDataBaseName { get; set; } = string.Empty;

        /// <summary>
        /// 資料庫使用者密碼
        /// </summary>
        public string SaaDataBasePassword { get; set; } = string.Empty;

        /// <summary>
        /// 點位群組
        /// </summary>
        public int SaaOffsetGroup { get; set; } = 0;

        /// <summary>
        /// 點位:W
        /// </summary>
        public string SaaOffsetWord { get; set; } = string.Empty;

        /// <summary>
        /// 詢問代碼:16
        /// </summary>
        public string SaaDataAsk { get; set; } = string.Empty;

        /// <summary>
        /// 回覆代碼:32
        /// </summary>
        public string SaaDataReply { get; set; } = string.Empty;

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

        /// <summary>
        /// 設備編號
        /// </summary>
        public int SaaSetNo { get; set; } = 0;

        /// <summary>
        /// 設備機型
        /// </summary>
        public string SaaModelName { get; set; } = string.Empty;

        /// <summary>
        /// NLog檔案名稱
        /// </summary>
        public string SaaNLogName { get; set; } = string.Empty;

        /// <summary>
        /// 轉譯程式Server IP位置
        /// </summary>
        public string WebApiServerIP { get; set; }= string.Empty;

        /// <summary>
        /// 轉譯程式Server Port位置
        /// </summary>
        public string WebApiServerPort { get; set; } = string.Empty;

        /// <summary>
        /// 回覆LCS接受結果完成
        /// </summary>
        public string WebApiResultOK { get; set; } = string.Empty;

        /// <summary>
        /// 回覆LCS接受結果失敗
        /// </summary>
        public string WebApiResultFAIL { get; set; } = string.Empty;

        public string PARTICLE { get; set; }= string.Empty;
    }
}
