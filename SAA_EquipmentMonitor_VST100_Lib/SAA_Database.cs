using NLog;
using SAA_EquipmentMonitor_VST100_Lib.Attributes;
using SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;
using SAA_EquipmentMonitor_VST100_Lib.MsSqlData;
using SAA_EquipmentMonitor_VST100_Lib.WebApiSendCommand;
using SAA_EquipmentMonitor_VST100_Lib.WebApiServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib
{
    public class SAA_Database
    {
        /// <summary>
        /// Log訊息
        /// </summary>
        /// <param name="message">訊息</param>
        public delegate void DelLogMessage(string message, SAA_DatabaseEnum.LogType logtype);
        public static event DelLogMessage? OnLogMessage;

        /// <summary>
        /// 路徑名稱
        /// </summary>
        public static readonly string Config = "Config";

        /// <summary>
        /// Config檔名稱
        /// </summary>
        public static readonly string SystemSetting = "SystemSetting.json";

        /// <summary>
        /// NLog方法
        /// </summary>
        public static Logger? SaaLog;

        /// <summary>
        /// Config屬性
        /// </summary>
        public static SaaConfigAttributes configattributes = new SaaConfigAttributes();

        /// <summary>
        /// SQL方法
        /// </summary>
        public static SqlData? SaaSql;

        /// <summary>
        /// 設備點位屬性
        /// </summary>
        public static SaaEquipmentOffsetAttributes saaequipmentoffset = new SaaEquipmentOffsetAttributes();

        /// <summary>
        /// 設備Bit點位
        /// </summary>
        public static SaaEquipmentBitOffsetAttributes saaequipmentbitoffset = new SaaEquipmentBitOffsetAttributes();

        #region [===字串轉Dictionary===]
        /// <summary>
        /// 字串轉Dictionary
        /// </summary>
        /// <param name="commandtext"></param>
        /// <returns></returns>
        public static Dictionary<string, string>? ContentToDictionary(string commandtext)
        {
            try
            {
                string messagetemp = commandtext.Replace("\r\n", "").Replace("\"", "").Replace("{", "").Replace("}", "");
                Dictionary<string, string> mydictionary = new Dictionary<string, string>();
                string[] aftercontent = messagetemp.Split(',');
                foreach (var datas in aftercontent)
                {
                    var data = datas.Split(':');
                    mydictionary.Add(data[0], data[1]);
                }
                return mydictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return null;
            }
        }
        #endregion

        #region [===寫入Log訊息===]
        /// <summary>
        ///寫入Log訊息
        /// </summary>
        /// <param name="message">訊息</param>
        /// <param name="logtype">log分類</param>
        public static void LogMessage(string message, SAA_DatabaseEnum.LogType logtype = SAA_DatabaseEnum.LogType.Normal)
        {
            try
            {
                string Message = $"【{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}】{message}";
                switch ((SAA_DatabaseEnum.LogType)Enum.Parse(typeof(SAA_DatabaseEnum.LogType), logtype.ToString()))
                {
                    case SAA_DatabaseEnum.LogType.Normal:
                        SaaLog?.Info(Message);
                        break;
                    case SAA_DatabaseEnum.LogType.Warnning:
                        SaaLog?.Warn(Message);
                        break;
                    case SAA_DatabaseEnum.LogType.Error:
                        SaaLog?.Error(Message);
                        break;
                }
                OnLogMessage?.Invoke(Message, logtype);
            }
            catch (Exception ex)
            {
                SaaLog?.Error($"{ex.Message}-{ex.StackTrace}");
            }
        }
        #endregion

        #region [===讀取當前時間===]
        /// <summary>
        /// 讀取當前時間
        /// </summary>
        /// <returns></returns>
        public static string ReadTime()
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
        }
        #endregion

        #region [===讀取當前時間===]
        /// <summary>
        /// 讀取當前時間
        /// </summary>
        /// <returns></returns>
        public static string ReadTeidTime()
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}";
        }
        #endregion

        /// <summary>
        /// 第一組資料點位
        /// </summary>
        public static Dictionary<string, string> DictionaryOffset1 = new Dictionary<string, string>();

        /// <summary>
        /// 第二組資料點位
        /// </summary>
        public static Dictionary<string, string> DictionaryOffset2 = new Dictionary<string, string>();

        /// <summary>
        /// 第三組資料點位
        /// </summary>
        public static Dictionary<string, string> DictionaryOffset3 = new Dictionary<string, string>();

        /// <summary>
        /// 第四組資料點位
        /// </summary>
        public static Dictionary<string, string> DictionaryOffset4 = new Dictionary<string, string>();

        /// <summary>
        /// 第五組資料點位
        /// </summary>
        public static Dictionary<string, string> DictionaryOffset5 = new Dictionary<string, string>();

        public static SaaEquipmentCommon EquipmentCommon = new SaaEquipmentCommon();

        /// <summary>
        /// Web Api啟動
        /// </summary>
        public static MainVM webapiserver = new MainVM();

        /// <summary>
        /// 傳送Web API方法
        /// </summary>
        public static SAA_WebApiSendCommand webapisendcommand = new SAA_WebApiSendCommand();

        public static string SaaSendCommandSystems(string commanddata, string reportevent)
        {
            try
            {
                return webapisendcommand.Post($"http://{configattributes.WebApiServerIP}:{configattributes.WebApiServerPort}/" + reportevent, commanddata)!;
            }
            catch (Exception ex)
            {
                LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error );
                return string.Empty;
            }
        }

        public static void ReadSaaOffset(int setno, string model_name)
        {
            try
            {
                foreach (string iteme in Enum.GetNames(typeof(SAA_DatabaseEnum.SaaOffsetName)))
                {
                    SaaEcaIoMapping iomapping = new SaaEcaIoMapping
                    {
                        SETNO = setno,
                        MODEL_NAME = model_name,
                        EVENTFUNCTION = iteme,
                    };
                    var data = SaaSql!.GetEcaIoMapping(iomapping)!;
                    if (data.Rows.Count != 0)
                    {
                        switch ((SAA_DatabaseEnum.SaaOffsetName)Enum.Parse(typeof(SAA_DatabaseEnum.SaaOffsetName), iteme))
                        {
                            case SAA_DatabaseEnum.SaaOffsetName.DateTime:
                                saaequipmentoffset.SaaDB_DateTime = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentoffset.SaaValue_DateTime = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.HearBit:
                                saaequipmentoffset.SaaDB_HeartBit = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentoffset.SaaValue_HeartBit = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.Track:
                                saaequipmentbitoffset.SaaDB_Track = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_Track = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.TrackAck:
                                saaequipmentbitoffset.SaaDB_TrackAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_TrackAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.Synchronize:
                                saaequipmentbitoffset.SaaDB_Synchronize = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_Synchronize = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.SynchronizeAck:
                                saaequipmentbitoffset.SaaDB_SynchronizeAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_SynchronizeAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.Request:
                                saaequipmentbitoffset.SaaDB_Request = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_Request = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RequestAck:
                                saaequipmentbitoffset.SaaDB_RequestAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RequestAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RequestAll:
                                saaequipmentbitoffset.SaaDB_RequestAll = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RequestAll = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RequestAllAck:
                                saaequipmentbitoffset.SaaDB_RequestAllAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RequestAllAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RequestConfirm:
                                saaequipmentbitoffset.SaaDB_RequestConfirm = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RequestConfirm = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RequestModeError:
                                saaequipmentbitoffset.SaaDB_RequestModeError = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RequestModeError = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DataConfirmError:
                                saaequipmentbitoffset.SaaDB_DataConfirmError = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DataConfirmError = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DataCount:
                                saaequipmentbitoffset.SaaDB_DataCount = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DataCount = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DataCountAck:
                                saaequipmentbitoffset.SaaDB_DataCountAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DataCountAck = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DataRGVE84:
                                saaequipmentbitoffset.SaaDB_DataRGVE84 = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DataRGVE84 = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.RGV:
                                saaequipmentbitoffset.SaaDB_RGV = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_RGV = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.PUGVOUT:
                                saaequipmentbitoffset.SaaDB_PUGVOUT = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_PUGVOUT = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DKIN:
                                saaequipmentbitoffset.SaaDB_DKIN = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DKIN = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            case SAA_DatabaseEnum.SaaOffsetName.DeviceStatus:
                                saaequipmentbitoffset.SaaDB_DeviceStatus = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.SYMBOL.ToString()].ToString()!;
                                saaequipmentbitoffset.SaaValue_DeviceStatus = data.Rows[0][SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!;
                                break;
                            default:
                                break;
                        }
                    }
                }

                for (int i = 1; i < configattributes.SaaOffsetGroup + 1; i++)
                {
                    SaaEcaIoMapping iomapping = new SaaEcaIoMapping
                    {
                        SETNO = setno,
                        MODEL_NAME = model_name,
                        SCGROUP = i.ToString(),
                        SYMBOL = configattributes.SaaOffsetWord
                    };
                    DataTable datagroup = SaaSql!.GetEcaIoMappingGroup(iomapping);
                    switch (i)
                    {
                        case 1:
                            foreach (DataRow dr1 in datagroup.Rows)
                            {
                                SaaEcaIoMapping dic1 = new SaaEcaIoMapping
                                {
                                    OFFSET_ID = dr1[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET_ID.ToString()].ToString()!,
                                    OFFSET = dr1[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!
                                };
                                if (!DictionaryOffset1.ContainsKey(dic1.OFFSET_ID))
                                    DictionaryOffset1.Add(dic1.OFFSET_ID, dic1.OFFSET);
                            }
                            break;
                        case 2:
                            foreach (DataRow dr2 in datagroup.Rows)
                            {
                                SaaEcaIoMapping dic2 = new SaaEcaIoMapping
                                {
                                    OFFSET_ID = dr2[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET_ID.ToString()].ToString()!,
                                    OFFSET = dr2[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!
                                };
                                if (!DictionaryOffset2.ContainsKey(dic2.OFFSET_ID))
                                    DictionaryOffset2.Add(dic2.OFFSET_ID, dic2.OFFSET);
                            }
                            break;
                        case 3:
                            foreach (DataRow dr3 in datagroup.Rows)
                            {
                                SaaEcaIoMapping dic3 = new SaaEcaIoMapping
                                {
                                    OFFSET_ID = dr3[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET_ID.ToString()].ToString()!,
                                    OFFSET = dr3[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!
                                };
                                if (!SAA_Database.DictionaryOffset3.ContainsKey(dic3.OFFSET_ID))
                                    DictionaryOffset3.Add(dic3.OFFSET_ID, dic3.OFFSET);
                            }
                            break;
                        case 4:
                            foreach (DataRow dr4 in datagroup.Rows)
                            {
                                SaaEcaIoMapping dic4 = new SaaEcaIoMapping
                                {
                                    OFFSET_ID = dr4[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET_ID.ToString()].ToString()!,
                                    OFFSET = dr4[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!
                                };
                                if (!SAA_Database.DictionaryOffset4.ContainsKey(dic4.OFFSET_ID))
                                    SAA_Database.DictionaryOffset4.Add(dic4.OFFSET_ID, dic4.OFFSET);
                            }
                            break;
                        case 5:
                            foreach (DataRow dr5 in datagroup.Rows)
                            {
                                SaaEcaIoMapping dic5 = new SaaEcaIoMapping
                                {
                                    OFFSET_ID = dr5[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET_ID.ToString()].ToString()!,
                                    OFFSET = dr5[SAA_DatabaseEnum.SaaEcaIoMapping.OFFSET.ToString()].ToString()!
                                };
                                if (!DictionaryOffset5.ContainsKey(dic5.OFFSET_ID))
                                    DictionaryOffset5.Add(dic5.OFFSET_ID, dic5.OFFSET);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{ex.Message}-{ex.StackTrace}", SAA_DatabaseEnum.LogType.Error);
            }
        }
    }
}
