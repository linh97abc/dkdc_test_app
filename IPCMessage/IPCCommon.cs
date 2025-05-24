using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCMessage
{
    public delegate void IPCExceptionHandler(Exception ex);
    public delegate void IPCCommonCallback();

    class IPCCommon
    {
        public static string PipeName = "vtacTestIPC";

        public static async Task<byte[]> ReadAllAsync(Stream stream, int len)
        {
            byte[] buffer = new byte[len];
            int offset = 0;


            while (len > 0)
            {
                var readLen = await stream.ReadAsync(buffer, offset, len);
                if (readLen <= 0)
                {
                    throw new Exception("IPC disconnect");
                }
                offset += readLen;
                len -= readLen;
            }

            return buffer;
        }
    }
}
