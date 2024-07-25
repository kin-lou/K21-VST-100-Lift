using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.EquipmentPlcAttributes
{
    public class SaaEquipmentBitOffsetAttributes
    {
        public string SaaDB_TrackAck { get; set; } = string.Empty;
        public string SaaDB_Track { get; set; } = string.Empty;
        public string SaaDB_Synchronize { get; set; } = string.Empty;
        public string SaaDB_SynchronizeAck { get; set; } = string.Empty;
        public string SaaDB_Request { get; set; } = string.Empty;
        public string SaaDB_RequestAck { get; set; } = string.Empty;
        public string SaaDB_RequestAll { get; set; } = string.Empty;
        public string SaaDB_RequestAllAck { get; set; } = string.Empty;
        public string SaaDB_RequestConfirm { get; set; } = string.Empty;
        public string SaaDB_RequestModeError { get; set; } = string.Empty;
        public string SaaDB_DataConfirmError { get; set; } = string.Empty;
        public string SaaDB_DataCount { get; set; } = string.Empty;
        public string SaaDB_DataCountAck { get; set; } = string.Empty;

        public string SaaDB_RGV { get; set; } = string.Empty;

        public string SaaDB_PUGVOUT { get; set; } = string.Empty;

        public string SaaDB_DKIN { get; set; } = string.Empty;

        public string SaaDB_DataRGVE84 { get; set; } = string.Empty;

        public string SaaValue_Track { get; set; } = string.Empty;
        public string SaaValue_TrackAck { get; set; } = string.Empty;
        public string SaaValue_Synchronize { get; set; } = string.Empty;
        public string SaaValue_SynchronizeAck { get; set; } = string.Empty;
        public string SaaValue_Request { get; set; } = string.Empty;
        public string SaaValue_RequestAck { get; set; } = string.Empty;
        public string SaaValue_RequestAll { get; set; } = string.Empty;
        public string SaaValue_RequestAllAck { get; set; } = string.Empty;
        public string SaaValue_RequestConfirm { get; set; } = string.Empty;
        public string SaaValue_RequestModeError { get; set; } = string.Empty;
        public string SaaValue_DataConfirmError { get; set; } = string.Empty;
        public string SaaValue_DataCount { get; set; } = string.Empty;
        public string SaaValue_DataCountAck { get; set; } = string.Empty;

        public string SaaValue_RGV { get; set; } = string.Empty;

        public string SaaValue_PUGVOUT { get; set; } = string.Empty;

        public string SaaValue_DKIN { get; set; } = string.Empty;

        public string SaaValue_DataRGVE84 { get; set; } = string.Empty;


        /// <summary>
        /// 設備狀態(DB)
        /// </summary>
        public string SaaDB_DeviceStatus { get; set; } = string.Empty;

        /// <summary>
        /// 設備狀態(值)
        /// </summary>
        public string SaaValue_DeviceStatus { get; set; } = string.Empty;
    }
}
