using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.WebApiServer
{
    public partial class ApiSetting : ObservableObject
    {
        [ObservableProperty] string _ApiIP = "127.0.0.1";

        [ObservableProperty] int _ApiPort = 7075;

        [ObservableProperty] int _ApiTimeout = 3000;

        public ApiSetting() { }

        public ApiSetting(ApiSetting setting)
        {
            ApiIP = setting.ApiIP;
            ApiPort = setting.ApiPort;
            ApiTimeout = setting.ApiTimeout;
        }
    }
}
