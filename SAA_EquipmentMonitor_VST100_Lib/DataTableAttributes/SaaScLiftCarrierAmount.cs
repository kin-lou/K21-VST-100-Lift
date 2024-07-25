using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScLiftCarrierAmount
    {
        public int SETNO { get; set; }

        public string MODEL_NAME { get; set; } = string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string TASKDATETIME { get; set; } = string.Empty;

        public string STATION { get; set; } = string.Empty;

        public string ACTION { get; set; } = string.Empty;

        public string CARRIERID { get; set; } = string.Empty;

        public string PARTNO { get; set; } = string.Empty;
    }
}
