using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;
using System.Threading;
using ISBNParser.Classes;
using ISBNParser.Watin_Processor;
using RefthisSocketServer;
using System.Diagnostics;



namespace ISBN
{
    public static class Program
    {
        static Thread threadMain;
        static Thread childThread;
        
        public static DataAccess da;
        static string data = "";

        static int counter = 0;
        public static string content = "";

        static void Main(string[] args)
        {


            counter = 0;
                
                Settings.Instance.AutoMoveMousePointerToTopLeft = false;

                threadMain = new Thread(() => Startup(data, childThread));

                threadMain.SetApartmentState(ApartmentState.STA);

                threadMain.Start();

            
            


                /* Disabled Original code

                //      Console.WriteLine("Start this shit....");
                //      content = Startup(data);



                //    while(content != "")
                //    {
                //
                //        Settings.Instance.AutoMoveMousePointerToTopLeft = false;
                //
                //        threadMain = new Thread(() => RunAllSites(content));
                //
                //        threadMain.SetApartmentState(ApartmentState.STA);
                //
                //        threadMain.Start();
                //
                //        content = "";
                //    }

    */


        }

        private static string Startup(string data, Thread childThread)
        {
            
            if(counter == 0)
            {
                //initialize the socket listener
                AsynchronousSocketListener.StartListening(childThread);
            }
           // else
           // {

           // }
           // else
           // {
           //     AsynchronousSocketListener.Main(null);
           // }


           counter++;
           data =  AsynchronousSocketListener.GetData();
           return data;
        }

        
        public static void RunAllSites(string isbn)
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



        public static void CloseAllIEWindows()
        {
            try
            {
                for (int x = IE.InternetExplorers().Count() - 1; x > 0; x--)
                {
                    IE test = IE.InternetExplorers()[x];
                    try
                    {
                        test.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error closing IE windows: " + e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing IE windows: " + ex.Message);
            }
        }
    }
}
