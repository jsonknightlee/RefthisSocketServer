using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["RefthisEndPoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();

                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (SocketException)
            {
                Trace.Write("Echo server could not start", "Error");
                return;
            }
            finally
            {
                this.runCompleteEvent.Set();
            }

            while (true)
            {
                IAsyncResult result = listener.BeginAcceptTcpClient(HandleAsyncConnection, listener);
                connectionWaitHandle.WaitOne();
            }
        }

        private void HandleAsyncConnection(IAsyncResult result)
        {
            // Accept connection
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);
            connectionWaitHandle.Set();

            // Accepted connection
            Guid clientId = Guid.NewGuid();
            Trace.WriteLine("Accepted connection with ID " + clientId.ToString(), "Information");

            // Setup reader/writer
            NetworkStream netStream = client.GetStream();
            StreamReader reader = new StreamReader(netStream);
            StreamWriter writer = new StreamWriter(netStream);
            writer.AutoFlush = true;

            // Show application
            string input = string.Empty;
            while (input != "9")
            {
                // Show menu
                writer.WriteLine("…");

                input = reader.ReadLine();
                writer.WriteLine();

                // Do something
                if (input == "1")
                {
                    writer.WriteLine("Current date: " + DateTime.Now.ToShortDateString());
                }
                else if (input == "2")
                {
                    writer.WriteLine("Current time: " + DateTime.Now.ToShortTimeString());
                }

                writer.WriteLine();
            }

            // Done!
            client.Close();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
