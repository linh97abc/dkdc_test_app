using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf;
using IPCMessage;

namespace dkdc_test.Services
{
    internal class IpcService
    {
        public delegate void NewLogCallback(string line);

        public event NewLogCallback OnNewLog;

        public event IPCExceptionHandler OnExceptionHandler
        {
            add { this.ipcClient.OnExceptionHandler += value; }
            remove { this.ipcClient.OnExceptionHandler -= value; }
        }

        IPCMessage.IPCClient ipcClient;

        public IpcService()
        {
            this.ipcClient = new IPCMessage.IPCClient();
            this.ipcClient.OnMsgHandler += IpcClient_OnMsgHandler;
            this.ipcClient.OnDisconnect += IpcClient_OnDisconnect;

            
        }

        public void Start()
        {
            // This method is called to start the IPC client connection.
            // The connection is established in the constructor, so this method can be empty.
            Task.Run(async () => await this.ipcClient.TaskClientConnect());
        }

        private void IpcClient_OnDisconnect()
        {
            ServiceProvider.Inst.GUIService.ShowNotifyException("ipc", "Backend disconnected");
            ServiceProvider.Inst.AppBusiness.IsSerialOpen = false;
        }

        public async Task NotifyEventBtnClick(uint btnID)
        {
            try
            {
                await this.ipcClient.SendAsync(new DkdcIpc.FrontendToBackendMessage()
                {
                    ButtonClick = new DkdcIpc.ButtonClick()
                    {
                        ButtonId = btnID
                    }
                });

            }
            catch (Exception ex)
            {

                ServiceProvider.Inst.GUIService.ShowNotifyError("Button Click", ex.Message);
            }
        }

        public async Task RequestOpenSerialPort(string port)
        {


            await this.ipcClient.SendAsync(new DkdcIpc.FrontendToBackendMessage()
            {
                OpenPort = new DkdcIpc.OpenPort()
                {
                    Port = port
                }
            });

        }

        public async Task RequestCloseSerialPort()
        {

            await this.ipcClient.SendAsync(new DkdcIpc.FrontendToBackendMessage()
            {
                ClosePort = new DkdcIpc.ClosePort()
                {

                }
            });


        }

        void OnNotifyData(DkdcIpc.NotifyData data)
        {
            switch (data.Type)
            {
                case DkdcIpc.NotifyData.Types.NotifyType.Error:
                    ServiceProvider.Inst.GUIService.ShowNotifyError(data.Title, data.Message);
                    break;
                case DkdcIpc.NotifyData.Types.NotifyType.Info:
                    ServiceProvider.Inst.GUIService.ShowNotifyInfo(data.Title, data.Message);
                    break;
                case DkdcIpc.NotifyData.Types.NotifyType.Warning:
                    ServiceProvider.Inst.GUIService.ShowNotifyWarning(data.Title, data.Message);
                    break;
                case DkdcIpc.NotifyData.Types.NotifyType.Success:
                    ServiceProvider.Inst.GUIService.ShowNotifySuccess(data.Title, data.Message);
                    break;
                case DkdcIpc.NotifyData.Types.NotifyType.Exception:
                    ServiceProvider.Inst.GUIService.ShowNotifyException(data.Title, data.Message);
                    break;
                default:
                    break;
            }
        }

        async Task OnQueryInput(DkdcIpc.QueryInput query)
        {
            var data = ServiceProvider.Inst.InputDataModel.GetInputValue(query.InputId);
            await this.ipcClient.SendAsync(new DkdcIpc.FrontendToBackendMessage()
            {
                InputData = new DkdcIpc.InputData()
                {
                    InputId = query.InputId,
                    Value = data
                }
            });
        }

        void OnNewChartData(DkdcIpc.ChartData chartData)
        {
            // This method should handle new chart data.
            // It might involve updating a chart view model or notifying other components.
            // Implementation is not provided in the original code.
            ServiceProvider.Inst.ChartViewModel.AddLineData(chartData.Data.ToArray());
        }

        private void IpcClient_OnMsgHandler(DkdcIpc.BackendToFrontendMessage message)
        {
            switch (message.CmdCase)
            {
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.None:
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.ClosePort:
                    ServiceProvider.Inst.AppBusiness.IsSerialOpen = false;
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.NotifyData:
                    this.OnNotifyData(message.NotifyData);
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.QueryInput:
                    _ = this.OnQueryInput(message.QueryInput);
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.ChartData:
                    this.OnNewChartData(message.ChartData);
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.LogData:
                    this.OnNewLog?.Invoke(message.LogData.Log);
                    break;
                case DkdcIpc.BackendToFrontendMessage.CmdOneofCase.InputData:
                    ServiceProvider.Inst.InputDataModel.UpdateInputData(
                        message.InputData.InputId,
                        message.InputData.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
