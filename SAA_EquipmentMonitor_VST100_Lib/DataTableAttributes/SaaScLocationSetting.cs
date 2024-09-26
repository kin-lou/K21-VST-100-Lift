using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScLocationSetting
    {
        public int SETNO { get; set; }

        public string MODEL_NAME { get; set; } = string.Empty;

        public string STATIOM_NAME { get; set; }= string.Empty;

        public string LOCATIONID { get; set; } = string.Empty;

        public string HOSTID { get; set; } = string.Empty;

        public string CARRIERID { get; set; } = string.Empty;

        public string PARTNO { get; set; } = string.Empty;

        public string BANK { get; set; } = string.Empty;

        public string BAY { get; set; } = string.Empty;

        public string LV { get; set; } = string.Empty;

        public string LOCATIONSTATUS { get; set; } = string.Empty;

        public string LOCATIONMODE { get; set; } = string.Empty;

        public string LOCATIONTYPE { get; set; } = string.Empty;

        public int INVENTORYFULL { get; set; }

        public string ZONEID { get; set; } = string.Empty;

        public string LOCATIONPRIORITIZ { get; set; } = string.Empty;

        public string PUTTIME { get; set; } = string.Empty;

        public string OPER { get; set; } = string.Empty;

        public string USERFLAG { get; set; } = string.Empty;

        public string CARRIERSTATE { get; set; } = string.Empty;

        public string DESTINATIONTYPE { get; set; } = string.Empty;

        public string CARRIERID1 { get; set; } = string.Empty;
    }
}
