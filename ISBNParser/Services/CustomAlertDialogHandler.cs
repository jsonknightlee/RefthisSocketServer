using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Diagnostics;
using WatiN.Core.Native.Windows;
using WatiN.Core.DialogHandlers;

namespace ISBNParser.Services
{

    class CustomAlertDialogHandler
    {
        // Finds the dialog box by title. 
        public static string Execute(WatiN.Core.Element button, string dialogBoxTitle)
        {
            button.ClickNoWait();
            IList<Window> windows;
            WatiN.Core.Native.InternetExplorer.WindowsEnumerator wE = new WatiN.Core.Native.InternetExplorer.WindowsEnumerator();
            windows = wE.GetWindows(null);
            string result = "";
            foreach (Window w in windows)
            {
                if (w.Title == dialogBoxTitle)
                {
                    result = w.Message;
                    w.ForceClose();
                    break;
                }
            }
            return result;
        }

        //Finds and handles double pop up dialogs on logout button click that need delay
        public static string ExecuteDouble(WatiN.Core.Element button, string dialogBoxTitle)
        {
            button.ClickNoWait();
            Thread.Sleep(1000);
            IList<Window> windows;
            WatiN.Core.Native.InternetExplorer.WindowsEnumerator wE = new WatiN.Core.Native.InternetExplorer.WindowsEnumerator();
            windows = wE.GetWindows(null);
            string result = "";
            foreach (Window w in windows)
            {
                if (w.Title == dialogBoxTitle)
                {
                    result = w.Message;
                    w.ForceClose();
                    break;
                }
            }
            Thread.Sleep(1000);
            return result;
        }
    }
}
