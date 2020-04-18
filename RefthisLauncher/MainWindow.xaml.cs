using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RefthisSocketServer;
using RefthisSocketClient;
using System.Diagnostics;

namespace RefthisLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool socketOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
        
            StartProcess();
        }

        private void StartProcess()
        {
            socketOpen = true;
            while (socketOpen)
            {

                //  var path = AppDomain.CurrentDomain.BaseDirectory;
               var content =  Process.Start("RefthisSocketServer.exe");
                //AsynchronousSocketListener.Main(null);
                // for testing purses
                //SocketClient.Main();


                Console.WriteLine("DooWhapWobbleDiWhoop: " + content);
                socketOpen = false;

                




                //   Process.Start("RefthisSocketServer.exe");

                //Process.Start("C:\\Users\\test01\\Desktop\\Refthis\\Refthis!\\RefthisSocketClient\\RefthisSocketClient.exe");


            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // SocketListener.Stop();
            socketOpen = false;
        }
    }
}
