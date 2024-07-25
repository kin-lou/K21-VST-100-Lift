using SAA.Core.PLC.Models;
using SAA.Core.PLC.Slmp.Functions;
using SAA.Core.PLC.Slmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcImp
{
    public class EquipmentPlc
    {
        private PlcRep? report = null;
        private Slmp SaaSlmp;
        private readonly object plclock = new object();

        public EquipmentPlc(SaaEquipmentPlcAttributes SaaEquipment)
        {
            SaaSlmp = new Slmp();
            SaaSlmp.ActHostAddress = SaaEquipment.ActHostAddress;
            SaaSlmp.ActPort = SaaEquipment.ActPort;
            SaaSlmp.ConnTimeout = SaaEquipment.ConnTimeout;
            SaaSlmp.RecvTimeout = SaaEquipment.RecvTimeout;
            SaaSlmp.SendTimeout = SaaEquipment.SendTimeout;
            SaaSlmp.IsOctalXY = SaaEquipment.IsOctalXY == 1 ? true : false;
        }

        #region [===連線===]
        /// <summary>
        /// 連線
        /// </summary>
        /// <returns></returns>
        public bool SAA_PlcConnect()
        {
            var connect = SaaSlmp.Connect();
            return connect.IsSuccess;
        }
        #endregion

        #region [===中斷連線===]
        /// <summary>
        /// 中斷連線
        /// </summary>
        /// <returns></returns>
        public bool SAA_PlcDisconnect()
        {
            var disconnect = SaaSlmp.Disconnect();
            return disconnect.IsSuccess;
        }
        #endregion

        #region [===讀取PLC單一點位(Int)===]
        /// <summary>
        /// 讀取PLC單一點位(Int)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int ReadInt(string db, string offset)
        {
            lock (plclock)
            {
                if (SaaSlmp.IsConnected)
                {
                    return SaaSlmp.Read(db + offset).Data!.ToArray()[0];
                }
                else
                {
                    SAA_PlcConnect();
                    return 0;
                }
            }
        }
        #endregion

        #region [===讀取PLC點位數字(Int陣列)===]
        /// <summary>
        /// 讀取 PLC點位數字(Int陣列)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="readblockcount"></param>
        /// <returns></returns>
        public int[] ReadIntArray(string db, string offset, int readblockcount)
        {
            try
            {
                lock (plclock)
                {
                    if(SaaSlmp.IsConnected)
                    {
                        report = SaaSlmp.ReadBlock(db + offset, readblockcount);
                        return report.Data!.ToArray();
                    }
                    else
                    {
                        SAA_PlcConnect();
                        return new int[16];
                    }
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.Message}-{ex.StackTrace}");
                return new int[16];
            }
        }
        #endregion

        #region [===讀取PLC點為Bit(bool陣列)===]
        /// <summary>
        /// 讀取PLC點為Bit(bool陣列)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="readblockcount"></param>
        /// <returns></returns>
        public bool[] ReadBitArray(string db, string offset, int readblockcount)
        {
            int[] readint = ReadIntArray(db, offset, readblockcount);
            bool[] result = new bool[readint.Count() * readblockcount];
            for (int i = 0; i < readint.Count(); i++)
            {
                int tmp = readint[i];
                for (int j = 0; j < readblockcount; j++)
                {
                    result[i * readblockcount + j] = (tmp & 0x01) == 1 ? true : false;
                    tmp >>= 1;
                }
            }
            return result;
        }
        #endregion

        #region [=== 讀取PLC點位字串===]
        /// <summary>
        /// 讀取PLC點位字串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="readblockcount">長度</param>
        /// <returns></returns>
        public string ReadSringArray(string db, string offset, int readblockcount)
        {
            int[] readint = ReadIntArray(db, offset, readblockcount);
            var StrArrays = readint
                    .Select(num => (ushort)num)
                    .Select(num => num.AsBitArray())
                    .Select(bits =>
                    {
                        var binaryStr_1 = "";
                        var binaryStr_2 = "";
                        for (var i = 0; i < bits.Length; i++)
                        {
                            if (i < 8)
                                binaryStr_1 = (bits[i] ? "1" : "0") + binaryStr_1;
                            else
                                binaryStr_2 = (bits[i] ? "1" : "0") + binaryStr_2;
                        }
                        var ascii_1 = ((char)Convert.ToUInt32(binaryStr_1, 2)).ToString();
                        var ascii_2 = ((char)Convert.ToUInt32(binaryStr_2, 2)).ToString();
                        return ascii_1 + ascii_2;
                    }).ToArray();
            return string.Join("", StrArrays).CleanString();
        }
        #endregion

        #region [===讀取PLC點位Byte===]
        /// <summary>
        /// 讀取PLC點位Byte
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="readblockcount"></param>
        /// <returns></returns>
        public bool[] ReadByteArray(string db, string offset, int readblockcount = 16)
        {
            int value = ReadInt(db, offset);
            string BitString = Convert.ToString(value, 2).PadLeft(16, '0');
            bool[] returnvalue = new bool[readblockcount];
            for (int i = 0; i < returnvalue.Length; i++)
            {
                returnvalue[i] = BitString.Substring(i, 1) == "1" ? true : false;
            }
            //反矩陣排序一次
            Array.Reverse(returnvalue);
            return returnvalue;
        }
        #endregion

        #region [===寫入單一點位(Int)===]
        /// <summary>
        /// 寫入單一點位(Int)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteInt(string db, string offset, int value)
        {
            try
            {
                lock (plclock)
                {
                    return SaaSlmp.Write(db + offset, value).IsSuccess;
                }
            }
            catch (Exception ex)
            {
                SAA_Database.LogMessage($"{ex.StackTrace}-{ex.Message}");
                return false;
            }
        }
        #endregion

        #region [===寫入區域點位===]
        /// <summary>
        /// 寫入區域點位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteIntArray(string db, string offset, int[] value)
        {
            lock (plclock)
            {
                return SaaSlmp.WriteBlock(db + offset, value).IsSuccess;
            }
        }
        #endregion

        #region [===寫入布林值===]
        /// <summary>
        /// 寫入布林值
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteBool(string db, string offset, bool value)
        {
            lock (plclock)
            {
                int returnvalue = value ? 1 : 0;
                return WriteInt(db, offset, returnvalue);
            }
        }
        #endregion

        #region [===寫入點位字串===]
        /// <summary>
        /// 寫入點位字串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="offset"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteStringArray(string db, string offset, string value)
        {
            lock (plclock)
            {
                int[] test = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                WriteIntArray(db, offset, test);
                List<int> readint = ReadStringToInt(value);
                return WriteIntArray(db, offset, readint.ToArray());
            }
        }
        #endregion

        #region [===字串轉Int===]
        /// <summary>
        /// 字串轉Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<int> ReadStringToInt(string str)
        {
            var char_arr = str.ToCharArray().ToArray();
            var chunk_char_arr = char_arr.Chunk(2);
            var tmp = new List<int>();
            foreach (var item in chunk_char_arr)
            {
                var binary_str = "";
                foreach (var c in item)
                {
                    binary_str = Convert.ToString(c, 2).PadLeft(8, '0') + binary_str;
                }
                tmp.Add(Convert.ToUInt16(binary_str, 2));
            }
            return tmp;
        }
        #endregion
    }

    public static class Extensions
    {
        public static string CleanString(this string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }
    }
}
