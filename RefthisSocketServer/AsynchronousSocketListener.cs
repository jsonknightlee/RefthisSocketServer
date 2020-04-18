using ISBNParser.Classes;
using ISBNParser.Watin_Processor;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;





namespace RefthisSocketServer
{


    // State object for reading client data asynchronously  
    public class StateObject
    {


        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener : TempleteProfile
    {
        public static IsbnItem item = null;
        public static UserItem userItem = null;
        public static List<UserItem> user = null;
        public static List<ReferenceItem> reference =  null;

        public static string data = "";

        // handler State
        public static StateObject state = new StateObject();

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static DataAccess da;

        public AsynchronousSocketListener()
        { 
            
        }

        // HH - Make StartListening() return void

        public static void StartListening(Thread childThread)
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            // IPAddress ipAddress = ipHostInfo.AddressList[2];
            // IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = ipHostInfo.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 51968);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    Thread.Sleep(5000);

                    Console.WriteLine(data.Length);

                    // TODO extend data to include the deviceID
                    if(data != "")
                    {
                        item = new IsbnItem();
                        userItem = new UserItem();
                        da = new DataAccess();

                        string[] arrISBN = data.Split(';');
                        string isbn = arrISBN[0].Trim();
                        string deviceID = arrISBN[1].Trim();

                        // set the fields to populate DB and transport over socket connection
                        // In future the user will populate this field so this will be
                        // conditional
                        userItem.DeviceID = deviceID;
                        userItem.Email = null;
                        userItem.UserName = null;
                        userItem.FirstName = null;
                        userItem.LastName = null;


                        // save to database
                        // TODO parse userID
                        user = da.RegisterUser(userItem, deviceID, user);

                        int userID = 0;

                        // Slowing it down for testing
                        Thread.Sleep(2000);


                        

                        // Get the UserID
                        foreach(UserItem userr in user)
                        {
                            userID = userr.UserID;
                        }

                        item.ISBN = isbn;
                        item.UserID = userID;
                        
                        
                        // Check if the ISBN exists while saving if exists,
                        //return that 
                        reference = da.SaveSingleISBN(item, isbn, reference);

                        // return the reference from above method

                        // this breaks out of processing - change ISBN
                        if (reference != null)
                        {
                            // Get the book reference
                            // Return reference
                            // Notify client via socket?
                            // Already in db
                            
          //                  return reference.ToString();
                        }
                        
                        
                        // We send the userID along so that we can link the user that created the reference to it.
                        childThread = new Thread(() => GetReference(isbn, userID));
                        childThread.SetApartmentState(ApartmentState.STA);
                        childThread.Start();
         //               return data;
                                

                    }
                   

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();

    //        return data;
          
        
        }

        public static void GetReference(string isbn, int userID)
        {
            try
            {

                da = new DataAccess();
                //     da.ProcessLog("starting processing all");
                Console.WriteLine("starting processing all");

                // Listen on Sockets

                // Launch Socket data

                GooBoo website = new GooBoo(isbn);
                Console.WriteLine("Here we go...");


                CloseAllIEWindows();


                //      da.ProcessLog("finished processing all");
                Console.WriteLine("finished processing all");



                //Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(ex.Message);
            }


            //Console.WriteLine("Press any key to continue...");

            //Console.ReadKey(true);

            //  System.Environment.Exit(0);
        }

        private static void CloseAllIEWindows()
        {
            throw new NotImplementedException();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();

                Console.WriteLine(content.Length);


                //  if (content.Length == 23) now 30          
                  if (content != "")            
                {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    // Run the webscraper
                    data = content;

                    GetData();

                    // Echo the data back to the client.  
                    // Send(handler, content);
                     Send(handler, "fuck you");
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
          
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static string GetData()
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            else
            {
                return data;
            }
           
        }
         
    }
    /*
    public class SocketListener
    {
        public static bool open = true;
             
        public static void Main(string[] args = null)
        {   
            
            StartServer();
           // return 0;
       
        }

     

        public static bool Run()
        {
            
            return open;
        }


        public static void Stop()
        {
            // TODO expand on this
            open = false;
           
        }

        public static void StartServer()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);



            try
            {

                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for a connection");

                // Socket handler = listener.Accept();
                Socket handler = null;

                listener.BeginAccept(AcceptCallback, listener);

                // New async test code
                void AcceptCallback(IAsyncResult result)
                {
                    Socket server = (Socket)result.AsyncState;
                    handler = server.EndAccept(result);

                    // client socket logic...

                    server.BeginAccept(AcceptCallback, server); // <- continue accepting connections
                }

               
                          


                string data = null;
                byte[] bytes = null;

                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);

                while (true)          
                {
                   
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.Length == 13)
                    {  
                        break;
                    }
                }

                Console.WriteLine("Text received: {0}", data);

                byte[] msg = Encoding.ASCII.GetBytes(data);
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();



            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Query completed!");

            //Console.WriteLine("\n Press any key to continue");
            //Console.ReadKey();
        }
    }
    */
}
