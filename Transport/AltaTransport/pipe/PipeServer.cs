using System.IO.Pipes;
using System.Diagnostics;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AltaTransport
{
    // Delegate for passing received message back to caller
    public delegate void DelegateMessage(object Reply);

    public class PipeServer
    {
        public event DelegateMessage PipeData;
        private string _pipeName;

        public void Listen(string PipeName)
        {
            try
            {
                // Set to class level var so we can re-use in the async callback method
                _pipeName = PipeName;
                // Create the new async pipe 
                var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Wait for a connection
                pipeServer.BeginWaitForConnection(WaitForConnectionCallBack, pipeServer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                var pipeServer = (NamedPipeServerStream)iar.AsyncState;
                // End waiting for the connection
                pipeServer.EndWaitForConnection(iar);

                var buffer = new byte[65000];

                // Read the incoming message
                pipeServer.Read(buffer, 0, buffer.Length);

                // Convert byte buffer to object
                var data = ByteArrayToObject(buffer);

                // Pass message back to calling form
                PipeData?.Invoke(data);

                // Kill original sever and create new wait server
                pipeServer.Close();
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Recursively wait for the connection again and again....
                pipeServer.BeginWaitForConnection(WaitForConnectionCallBack, pipeServer);
            } catch {
                // ignored
            }
        }


        private static object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);

            return obj;
        }
    }
}
