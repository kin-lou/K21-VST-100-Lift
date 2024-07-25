using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScCommandTask
    {
        public string TASKDATETIME { get; set; }=string.Empty;

        public int SETNO { get; set; }

        public string MODEL_NAME { get; set; } = string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string COMMANDID { get; set; } = string.Empty;

        public string CARRIERID { get; set; } = string.Empty;

        public string LOCATIONTAKE { get; set; } = string.Empty;

        public string LOCATIONPUT { get; set; } = string.Empty;

        public string RESULT { get; set; } = string.Empty;
    }
}
