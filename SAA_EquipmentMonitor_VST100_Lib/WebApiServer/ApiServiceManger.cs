using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_EquipmentMonitor_VST100_Lib.WebApiServer
{
    public class ApiServiceManger : IDisposable
    {
        IHost _host;
        public ApiServiceManger(string ip, int port)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://{ip}:{port}");
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            Task.Run(() =>
            {
                _host.Run();
                _host.Start();
            });
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
