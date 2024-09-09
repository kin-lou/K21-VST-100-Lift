using SAA.Core.PLC.Slmp.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScDevice
    {
        public int SETNO { get; set; }

        public string MODEL_NAME { get; set; } = string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string DEVICENO { get; set; } = string.Empty;

        public string DEVICEID { get; set; } = string.Empty;

        public string HOSTDEVICEID { get; set; } = string.Empty;

        public string DEVICETYPE { get; set; } = string.Empty;

        public string LIFTIP { get; set; } = string.Empty;

        public string LIFTPORT { get; set; } = string.Empty;

        public string CONNECTSTATUS { get; set; } = string.Empty;

        public string DEVICESTATUS { get; set; } = string.Empty;

        public int AUTOREJECT { get; set; } = 0;
    }
}
