using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.DataTableAttributes
{
    public class SaaScCommon
    {
        public int SETNO { get; set; }
        public string MODEL_NAME { get; set; } = string.Empty;
        public string ITEM_NAME { get; set; } = string.Empty;
        public string ITEM_VALUE { get; set; } = string.Empty;
        public string VALUECOMMENT { get; set; } = string.Empty;
    }
}
