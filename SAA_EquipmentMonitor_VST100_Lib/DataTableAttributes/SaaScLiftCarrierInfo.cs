using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScLiftCarrierInfo
    {
        public int SETNO { get; set; }

        public string MODEL_NAME { get; set; }=string.Empty;

        public string STATION_NAME { get; set; } = string.Empty;

        public string CARRIERID { get; set; } = string.Empty;

        public string PARTNO { get; set; } = string.Empty;

        public string CARRIERTYPE { get; set; } = string.Empty;

        public string CALL_SHUTTLE { get; set; } = string.Empty;

        public string CARRIERID_UPDATE { get; set; } = string.Empty;

        public string REMOTE { get; set; }= string.Empty;

        public string READPLC { get; set; } = string.Empty;
    }
}
