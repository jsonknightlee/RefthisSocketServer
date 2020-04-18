
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using WatiN.Core.Native.InternetExplorer;
using System.IO;

//using System.Windows.Automation;
//using System.Windows.Automation.Text;
//using System.Windows.Interop;
using System.Drawing;

namespace PearWebsites.Watin_Processing.Watin_Extensions
{
    class DL_File
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Win32 API call to control mouse. WatiN's click() function does not work on one of the menus. 
        // --------------------------------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        // ---------------------------------------------------------------------------------------------

        // FileFolder for temporarily saving PDFs 

        //// Returns file path it downloads to. 
        //public static string Save_Target_As(Element elem, IE ie, string subscriberID, int offset_X = 0, int offset_Y = 0)
        //{
        //    try
        //    {
        //        DateTime saveNow = DateTime.Now;
        //        string saveNow2 = saveNow.ToString("MMddyyyyHHmm");

        //        StaticFunctions.PositionMousePointerInMiddleOfElement(elem, ie, offset_X, offset_Y);
        //        int x = Cursor.Position.X;
        //        int y = Cursor.Position.Y;

        //        StaticFunctions.SetForegroundWindowIE(ie);
        //        mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x, y, 0, 0);

        //        StaticFunctions.PositionMousePointerInMiddleOfElement(ie.Body, ie); 
        //        Thread.Sleep(500);
        //        SendKeys.SendWait("{DOWN}");
        //        Thread.Sleep(500);
        //        SendKeys.SendWait("{DOWN}");
        //        Thread.Sleep(500);
        //        SendKeys.SendWait("{DOWN}");
        //        Thread.Sleep(500);
        //        SendKeys.SendWait("{DOWN}");
        //        Thread.Sleep(1000);
        //        SendKeys.SendWait("{ENTER}");
        //        Thread.Sleep(15000);

        //        var di = new DirectoryInfo(Directory.GetCurrentDirectory());
        //        string tempFolder = @"C:"; 

        //        bool success = false;

        //        while (success == false)
        //        {
        //            try
        //            {
        //                SendKeys.SendWait(tempFolder + @"\temp_" + subscriberID + "_" + saveNow2 + ".pdf");
        //                Thread.Sleep(1500);
        //                SendKeys.SendWait("{ENTER}");
        //                success = true;
        //            }
        //            catch
        //            {
        //                success = false;
        //            }
        //        }

        //        return tempFolder + @"\temp_" + subscriberID + "_" + saveNow2 + ".pdf";
        //    }
        //    catch
        //    {
        //        return null; 
        //    }
        //}
    }
}
