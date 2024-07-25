using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.WebApiServer
{
    public enum NoticeLevel
    {
        Info,
        Warning,
        Error,
        Success,
        Fail,
    }
    public partial class Notice : ObservableObject
    {
        [ObservableProperty] string _Header = string.Empty;

        [ObservableProperty] string _Message = string.Empty;

        [ObservableProperty] NoticeLevel _NoticeLevel = NoticeLevel.Info;

        //[ObservableProperty] Brush _NoticeBrush = new SolidColorBrush(Colors.White);
    }
}
