using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;


namespace IPCMessage
{
    public class IPCClient
    {
        public delegate void MsgHandler(DkdcIpc.BackendToFrontendMessage message);

        public event MsgHandler OnMsgHandler;
        public event IPCExceptionHandler OnExceptionHandler;
        public event IPCCommonCallback OnDisconnect;

        NamedPipeClientStream stream;

        public async Task TaskClientConnect()
        {
            while (true)
            {
                try
                {
                    using (var pipeClient = 
                        new NamedPipeClientStream(".", 
                        IPCCommon.PipeName,
                        PipeDirection.InOut,
                        PipeOptions.Asynchronous))
                    {
                        await pipeClient.ConnectAsync(1000);

                        this.stream = pipeClient;

                        await this.ParserBackendData();
                        this.OnDisconnect?.Invoke();
                    }

                    //this.stream = null;
                }
                catch (IOException)
                {
                    await Task.Delay(200);
                    //return;
                }

            }

        }

        public async Task SendAsync(DkdcIpc.FrontendToBackendMessage req)
        {
            if (this.stream == null)
            {
                throw new Exception( "Backend not connect");
            }

            if (!this.stream.IsConnected)
            {
                throw new Exception("Backend disconnected");
                
            }

            try
            {
                var buf = req.ToByteArray();
                await stream.WriteAsync(buf, 0, buf.Length);
                await this.stream.FlushAsync();


            }
            catch (Exception ex)
            {
                //this.stream = null;
                try
                {

                    this.stream.Close();
                }
                catch (Exception)
                {
                    throw;

                }

                throw ex;
            }

           

        }

        async Task ParserBackendData()
        {

            while (true)
            {
                int msgLen;
                try
                {
                    var lengthBuffer = await IPCCommon.ReadAllAsync(stream, 4);
                    msgLen = BitConverter.ToInt32(lengthBuffer, 0);

                }
                catch (Exception ex)
                {
                    //
                    break;
                }

                byte[] buffer;
                try
                {
                    buffer = await IPCCommon.ReadAllAsync(stream, msgLen);

                }
                catch (Exception ex)
                {
                    //
                    break;
                }


                try
                {
                    var backendMsg = DkdcIpc.BackendToFrontendMessage.Parser.ParseFrom(buffer);
                    OnMsgHandler?.Invoke(backendMsg);

                }
                catch (Exception ex)
                {
                    throw;
                    //throw;
                }

               
            }

        }
    }
}
