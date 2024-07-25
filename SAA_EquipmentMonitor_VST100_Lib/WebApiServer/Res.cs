using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.WebApiServer
{
    public class Res
    {
        public bool IsSuccess { get; set; } = false;

        public string Msg { get; set; } = string.Empty;
    }

    public class RecvMsg
    {
        public DateTime RecvTime { get; } = DateTime.Now;

        public bool IsSuccess { get; set; } = false;

        public string Msg { get; } = string.Empty;

        public RecvMsg(string msg)
        {
            Msg = msg;
        }

        public RecvMsg(bool isSuccess, string msg)
        {
            IsSuccess = isSuccess;
            Msg = msg;
        }
    }
}
