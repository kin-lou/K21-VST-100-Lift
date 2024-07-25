using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaEcaIoMapping
    {
        public int SETNO { get; set; } = 0;

        public string MODEL_NAME { get; set; } = string.Empty;

        public string SCGROUP { get; set; } = string.Empty;

        public string INVENTORYMODE { get; set; } = string.Empty;

        public string EVENTFUNCTION { get; set; } = string.Empty;

        public string SYMBOL { get; set; } = string.Empty;

        public string OFFSET { get; set; } = string.Empty;

        public string OFFSET_ID { get; set; } = string.Empty;

        public string OFFSET_TYPE { get; set; } = string.Empty;

        public string REMARK { get; set; } = string.Empty;
    }
}
