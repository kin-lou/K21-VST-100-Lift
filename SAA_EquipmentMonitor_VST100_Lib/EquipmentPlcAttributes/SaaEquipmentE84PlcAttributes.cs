using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes
{
    public class SaaEquipmentE84PlcAttributes
    {
        public string TASKDATETIME { get; set; } = string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string SHUTTLEID { get; set; } = string.Empty;

        public string COMMANDID { get; set; } = string.Empty;

        public string CARRIERID { get; set; } = string.Empty;

        public int CS_0 { get; set; }

        public int Valid { get; set; }

        public int TR_REQ { get; set; }

        public int Busy { get; set; }

        public int Complete { get; set; }

        public int Continue { get; set; }

        public int SELECT { get; set; }

        public int AM_AVBL { get; set; }

        public string RESULT { get; set; } = string.Empty;
    }
}
