using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib
{
    public class SAA_Config
    {
        #region [===讀取Config資料===]
        /// <summary>
        /// 讀取Config資料
        /// </summary>
        public void SaaReadConfig()
        {
            try
            {
                var systemsetting = ReadConfigString(SAA_Database.SystemSetting);
                Dictionary<string, string> dicsystemsetting = SAA_Database.ContentToDictionary(systemsetting)!;
                foreach (var data in dicsystemsetting)
                {
                    if (Enum.IsDefined(typeof(ConfigName), data.Key.Trim()))
                    {
                        switch ((ConfigName)Enum.Parse(typeof(ConfigName), data.Key.Trim()))
                        {
                            case ConfigName.SaaDataBase:
                                SAA_Database.configattributes.SaaDataBase = data.Value.Trim();
                                break;
                            case ConfigName.SaaDataBaseIP:
                                SAA_Database.configattributes.SaaDataBaseIP = data.Value.Trim();
                                break;
                            case ConfigName.SaaDataBaseName:
                                SAA_Database.configattributes.SaaDataBaseName = data.Value.Trim();
                                break;
                            case ConfigName.SaaDataBasePassword:
                                SAA_Database.configattributes.SaaDataBasePassword = data.Value.Trim();
                                break;
                            case ConfigName.SaaOffsetGroup:
                                SAA_Database.configattributes.SaaOffsetGroup = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.SaaOffsetWord:
                                SAA_Database.configattributes.SaaOffsetWord = data.Value.Trim();
                                break;
                            case ConfigName.SaaDataAsk:
                                SAA_Database.configattributes.SaaDataAsk = data.Value.Trim();
                                break;
                            case ConfigName.SaaDataReply:
                                SAA_Database.configattributes.SaaDataReply = data.Value.Trim();
                                break;
                            case ConfigName.ActHostAddress:
                                SAA_Database.configattributes.ActHostAddress = data.Value.Trim();
                                break;
                            case ConfigName.ActPort:
                                SAA_Database.configattributes.ActPort = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.ConnTimeout:
                                SAA_Database.configattributes.ConnTimeout = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.RecvTimeout:
                                SAA_Database.configattributes.RecvTimeout = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.SendTimeout:
                                SAA_Database.configattributes.SendTimeout = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.IsOctalXY:
                                SAA_Database.configattributes.IsOctalXY = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.SaaSetNo:
                                SAA_Database.configattributes.SaaSetNo = int.Parse(data.Value.Trim());
                                break;
                            case ConfigName.SaaModelName:
                                SAA_Database.configattributes.SaaModelName = data.Value.Trim();
                                break;
                            case ConfigName.SaaNLogName:
                                SAA_Database.configattributes.SaaNLogName = data.Value.Trim();
                                break;
                            case ConfigName.WebApiServerIP:
                                SAA_Database.configattributes.WebApiServerIP = data.Value.Trim();
                                break;
                            case ConfigName.WebApiServerPort:
                                SAA_Database.configattributes.WebApiServerPort = data.Value.Trim();
                                break;
                            case ConfigName.WebApiResultOK:
                                SAA_Database.configattributes.WebApiResultOK = data.Value.Trim();
                                break;
                            case ConfigName.WebApiResultFAIL:
                                SAA_Database.configattributes.WebApiResultFAIL = data.Value.Trim();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
        #endregion

        #region [===讀取Config字串===]
        private string ReadConfigString(string fileName)
        {
            try
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var configDir = Path.Combine(appPath, SAA_Database.Config);
                var path = Path.Combine(configDir, fileName);
                using StreamReader sr = new(path);
                var json_str = sr.ReadToEnd();
                sr.Close();
                return json_str;
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
                return string.Empty;
            }
        }
        #endregion

        public void ReadScCommon(int setno, string model_name)
        {
            try
            {
                foreach (string item in Enum.GetNames(typeof(SAA_DatabaseEnum.EquipmentOffset)))
                {
                    SaaScCommon saaCommon = new SaaScCommon
                    {
                        SETNO = setno,
                        MODEL_NAME = model_name,
                        ITEM_NAME = item,
                    };
                    var data = SAA_Database.SaaSql?.GetScCommon(saaCommon)!;
                    if (data.Rows.Count != 0)
                    {
                        switch ((SAA_DatabaseEnum.EquipmentOffset)Enum.Parse(typeof(SAA_DatabaseEnum.EquipmentOffset), item))
                        {
                            case SAA_DatabaseEnum.EquipmentOffset.GroupCount:
                                SAA_Database.EquipmentCommon.GroupCount = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataTrack1:
                                SAA_Database.EquipmentCommon.DataTrack1 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataReply1:
                                SAA_Database.EquipmentCommon.DataReply1 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataLocal1:
                                SAA_Database.EquipmentCommon.DataLocal1 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataRemote1:
                                SAA_Database.EquipmentCommon.DataRemote1 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataCarrierId1:
                                SAA_Database.EquipmentCommon.DataCarrierId1 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataTrack2:
                                SAA_Database.EquipmentCommon.DataTrack2 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataReply2:
                                SAA_Database.EquipmentCommon.DataReply2 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataLocal2:
                                SAA_Database.EquipmentCommon.DataLocal2 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataRemote2:
                                SAA_Database.EquipmentCommon.DataRemote2 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.DataCarrierId2:
                                SAA_Database.EquipmentCommon.DataCarrierId2 = int.Parse(data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!);
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Move:
                                SAA_Database.EquipmentCommon.Move = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Update:
                                SAA_Database.EquipmentCommon.Update = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Establish:
                                SAA_Database.EquipmentCommon.Establish = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Clear:
                                SAA_Database.EquipmentCommon.Clear = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Ask:
                                SAA_Database.EquipmentCommon.Ask = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Reply:
                                SAA_Database.EquipmentCommon.Reply = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.Have:
                                SAA_Database.EquipmentCommon.Have = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.None:
                                SAA_Database.EquipmentCommon.None = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.EquipmentOffset.ReaderError:
                                SAA_Database.EquipmentCommon.ReaderError = data.Rows[0][SAA_DatabaseEnum.SC_COMMON.ITEM_VALUE.ToString()].ToString()!;
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }
        }

        public enum ConfigName
        {
            /// <summary>
            /// 資料庫名稱
            /// </summary>
            SaaDataBase,

            /// <summary>
            /// 資料庫IP位置
            /// </summary>
            SaaDataBaseIP,

            /// <summary>
            /// 資料庫使用者名稱
            /// </summary>
            SaaDataBaseName,

            /// <summary>
            /// 資料庫使用者密碼
            /// </summary>
            SaaDataBasePassword,

            /// <summary>
            /// 點位群組
            /// </summary>
            SaaOffsetGroup,

            /// <summary>
            /// 點位:W
            /// </summary>
            SaaOffsetWord,

            /// <summary>
            /// 詢問代碼:16
            /// </summary>
            SaaDataAsk,

            /// <summary>
            /// 回覆代碼:32
            /// </summary>
            SaaDataReply,

            /// <summary>
            /// PLC IP位置
            /// </summary>
            ActHostAddress,

            /// <summary>
            /// PLC Portt位置
            /// </summary>
            ActPort,

            /// <summary>
            /// 連線超時時間
            /// </summary>
            ConnTimeout,

            /// <summary>
            /// 接收超時時間
            /// </summary>
            RecvTimeout,

            /// <summary>
            /// 傳送超時時間
            /// </summary>
            SendTimeout,

            /// <summary>
            /// FX5U 的 CPU X、Y 位置是 8 進位，需要把這個設定為 True
            /// </summary>
            IsOctalXY,

            /// <summary>
            /// 設備編號
            /// </summary>
            SaaSetNo,

            /// <summary>
            /// 機型名稱
            /// </summary>
            SaaModelName,

            /// <summary>
            /// NLOG檔案名稱
            /// </summary>
            SaaNLogName,

            /// <summary>
            /// 傳送轉譯程式Server IP位置
            /// </summary>
            WebApiServerIP,

            /// <summary>
            /// 傳送轉譯程式Server Port位置
            /// </summary>
            WebApiServerPort,

            /// <summary>
            /// 回覆LCS接受結果完成
            /// </summary>
            WebApiResultOK,

            /// <summary>
            ///  回覆LCS接受結果失敗
            /// </summary>
            WebApiResultFAIL,
        }
    }
}
