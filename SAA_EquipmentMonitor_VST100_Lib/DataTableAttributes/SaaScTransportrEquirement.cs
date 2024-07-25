using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScTransportrEquirement
    {
        public string SETNO { get; set; } = string.Empty;
        public string MODEL_NAME { get; set; } = string.Empty;
        public string STATION_NAME { get; set; } = string.Empty;
        public string REPORTID { get; set; } = string.Empty;
        public string REPORT_TIME { get; set; } = string.Empty;
        public string REPORT_STATION { get; set; } = string.Empty;
        public string REQUIREMENT_TYPE { get; set; } = string.Empty;
        public string CARRIERID { get; set; } = string.Empty;
        public string BEGIN_STATION { get; set; } = string.Empty;
        public string END_STATION { get; set; } = string.Empty;
        public string REQUIREMENT_RESULT { get; set; } = string.Empty;
    }
}
