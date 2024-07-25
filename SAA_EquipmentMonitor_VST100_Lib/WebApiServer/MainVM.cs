using CommunityToolkit.Mvvm.ComponentModel;
using SAA_EquipmentMonitor_VST100_Lib.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.WebApiServer
{
    public partial class MainVM : ObservableObject
    {
        [ObservableProperty] static ApiSetting _ApiSettingTmp = new();

        [ObservableProperty] HttpTypeEnum _SelectedHttpType;

        [ObservableProperty] string _SendMessage = string.Empty;

        [ObservableProperty] ObservableCollection<RecvMsg> _RecvLogs = new();

        [ObservableProperty] ObservableCollection<Notice> _Notices = new();

        private ApiSetting ApiSetting = new();

        public ApiServiceManger? ApiServiceManger;

        public MainVM() { }

        /// <summary>
        /// 關閉ApiService
        /// </summary>
        public Res StopApiService()
        {
            try
            {
                if (ApiServiceManger is not null)
                {
                    ApiServiceManger.Dispose();
                }
                return new Res() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new Res() { IsSuccess = false, Msg = e.Message };
            }
        }

        /// <summary>
        /// 啟動ApiService
        /// </summary>
        public Res StartApiService()
        {
            try
            {
                if (ApiServiceManger is not null)
                {
                    ApiServiceManger.Dispose();
                }
                ApiServiceManger = new ApiServiceManger(ApiSetting.ApiIP, ApiSetting.ApiPort);
                return new Res() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new Res() { IsSuccess = false, Msg = e.Message };
            }
        }
    }
}
