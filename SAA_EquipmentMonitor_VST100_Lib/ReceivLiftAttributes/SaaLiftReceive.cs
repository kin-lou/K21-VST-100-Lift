﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.ReceivLiftAttributes
{
    public class SaaLiftReceive
    {
        /// <summary>
        /// 站點名稱
        /// </summary>
        public string Statiom_Name { get; set; } = string.Empty;

        /// <summary>
        /// 指令名稱
        /// </summary>
        public string CommandName { get; set; } = string.Empty;
    }
}
