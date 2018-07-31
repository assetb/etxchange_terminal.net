using System;
using System.IO.Pipes;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AltaTransport
{
    public class PipeClient
    {
        public void Send(object sentObj, string PipeName, int TimeOut = 1000)
        {
            try
            {
                var pipeStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.Asynchronous);

                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect(TimeOut);
                Debug.WriteLine("[Client] Pipe connection established");

                byte[] _buffer = ObjectToByteArray(sentObj);
                pipeStream.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeStream);
            }
            catch (TimeoutException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private static byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        private static void AsyncSend(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                var pipeStream = (NamedPipeClientStream)iar.AsyncState;

                // End the write
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
