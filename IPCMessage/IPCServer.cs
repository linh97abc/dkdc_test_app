using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace IPCMessage
{
    
    public class IPCServer
    {
        public delegate void MsgHandler(DkdcIpc.FrontendToBackendMessage message);
        public event MsgHandler OnMsgHandler;
        public event IPCExceptionHandler OnExceptionHandler;
        public event IPCCommonCallback OnDisconnect;
        public event IPCCommonCallback OnConnect;

        public async Task TaskListenner()
        {
            while (true)
            {
                using (var pipeServer = new NamedPipeServerStream(
                    IPCCommon.PipeName,
                    PipeDirection.InOut,
                    //NamedPipeServerStream.MaxAllowedServerInstances, 
                    1,
                    PipeTransmissionMode.Message,
                    //PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous))
                {
                    await pipeServer.WaitForConnectionAsync();
                    this.OnConnect?.Invoke();
                    this.serverStream = pipeServer;

                    try
                    {
                        await AppClientTask(pipeServer);

                    }
                    catch (Exception ex)
                    {
                        this.OnExceptionHandler?.Invoke(ex);
                    }

                    this.OnDisconnect?.Invoke();
                }

            }
        }

        

        async Task AppClientTask(NamedPipeServerStream stream)
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int msgLen;

                try
                {
                    msgLen = await stream.ReadAsync(buffer, 0, buffer.Length);
                }
                catch (Exception)
                {

                    break;
                }

                if (msgLen <= 0)
                {
                    break;
                }

                var message = DkdcIpc.FrontendToBackendMessage.Parser.ParseFrom(buffer, 0, msgLen);

                this.OnMsgHandler?.Invoke(message);

            
            }
        }

        NamedPipeServerStream serverStream;

        public async Task SendAsync(DkdcIpc.BackendToFrontendMessage response)
        {
            if (this.serverStream == null)
            {
                return;
            }

            if (!this.serverStream.IsConnected)
            {
                return;
            }



            byte[] res;
            using (MemoryStream mem = new MemoryStream())
            {
                var tmp = new byte[4];
                mem.Write(tmp, 0, 4);

                response.WriteTo(mem);

                res = mem.ToArray();
                var lenBuffer = BitConverter.GetBytes(res.Length - 4);

                Array.Copy(lenBuffer, 0, res, 0, 4);
            }



            try
            {
                await this.serverStream.WriteAsync(res, 0, res.Length);
                await this.serverStream.FlushAsync();
            }
            catch (Exception)
            {
                try
                {

                    this.serverStream.Close();
                }
                catch (Exception)
                {

                    
                }
                //this.serverStream = null;
            }

           
        }

    }
}
