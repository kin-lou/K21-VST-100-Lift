using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.Enums
{
    public class EnumHelper
    {
        public static Array GetHttpTypeEnumValues()
        {
            return Enum.GetValues(typeof(HttpTypeEnum))
                       .Cast<HttpTypeEnum>()
                       .ToArray();
        }
    }
}
