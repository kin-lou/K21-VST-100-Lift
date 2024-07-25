using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HttpTypeEnum
    {
        [EnumMember(Value = "Get")]
        Get,
        [EnumMember(Value = "Post")]
        Post,
        [EnumMember(Value = "Put")]
        Put,
        [EnumMember(Value = "Delete")]
        Delete
    }
}
