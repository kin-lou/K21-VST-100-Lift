using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.Attributes
{
    public class SaaEquipmentPlcOffsetAttributes
    {
        public string SETNO { get; set; } = string.Empty;

        public string MODEL_NAME { get; set; } = string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string E84PLC_DB { get; set; } = string.Empty;

        public string E84PLC_VALUE { get; set; } = string.Empty;

        public string E84PC_DB { get; set; } = string.Empty;

        public string E84PC_VALUE { get; set; } = string.Empty;

        public string E84PC_CARRRIERID_DB { get; set; } = string.Empty;

        public string E84PC_CARRRIERID_VALUE { get; set; } = string.Empty;
    }
}
