using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentAttributes
{
    public class SaaEquipmentShelfMaxAmount
    {
        public string StationName { get; set; } = string.Empty;

        public short EmptyCount { get; set; } = 5;

        public short MaterialCount { get; set; } = 5;

        public short ShelfGloBalCount { get; set; } =4;
    }
}
