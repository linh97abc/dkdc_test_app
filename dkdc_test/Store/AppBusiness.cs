using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Store
{
    internal class AppBusiness : Components.utils.BindableBase
    {
        bool isSerialOpen;
        public bool IsSerialOpen
        {
            get => isSerialOpen;
            set
            {
                Set(ref isSerialOpen, value);
            }
        }

      

        public async Task Connect(string port)
        {
            this.IsSerialOpen = true;
            try
            {
                await Services.ServiceProvider.Inst.IpcService.RequestOpenSerialPort(port);
                Services.ServiceProvider.Inst.ChartViewModel.StartChartMonitor();

            }
            catch (Exception ex)
            {
                this.IsSerialOpen = false;
                Services.ServiceProvider.Inst.GUIService.ShowNotifyError("Connect", ex.Message);

            }
        }

        public async Task Disconnect()
        {
            this.IsSerialOpen = false;
            try
            {
                await Services.ServiceProvider.Inst.IpcService.RequestCloseSerialPort();
                Services.ServiceProvider.Inst.ChartViewModel.StopChart();

            }
            catch (Exception ex)
            {
                this.IsSerialOpen = true;
                Services.ServiceProvider.Inst.GUIService.ShowNotifyError("Disconnect", ex.Message);

            }
        }
    }
}
