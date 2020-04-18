using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using System.Threading;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using System.Runtime.InteropServices;
using WatiN.Core.DialogHandlers;


namespace WatiN.Core.DialogHandlers
{
    public class AnyDialogHandler : JavaDialogHandler
    {
        //string type = "";
        public string msg = "";
        private Window _window = null;

        public override bool CanHandleDialog(Window window)
        {
            //Stack up the various criteria from other dialog handlers
            bool criteria1 = (!ButtonWithId1Exists(window.Hwnd)) && (window.StyleInHex == "94C801C5"); //alert and confirm dialog handler
            bool criteria2 = (window.StyleInHex == "94C801C5"); //alert dialog handler
            bool criteria3 = ((window.StyleInHex == "94C801C5" || window.StyleInHex == "94C803C5") && ButtonWithId1Exists(window.Hwnd)); //confirm close
            bool criteria4 = (window.StyleInHex == "94C800C4"); //prompt dialog handler
            bool criteria5 = (window.StyleInHex == "8C880044") || (window.StyleInHex == "94C801C5"); //alert box used on AHIN website
            bool criteria6 = (window.StyleInHex == "0068221C"); //HDMA login message box
            return (criteria1 || criteria2 || criteria3 || criteria4 || criteria5 || criteria6);
        }

        protected override int getOKButtonID()
        {
            return 2;
        }

        public override bool HandleDialog(Window window)
        {
            msg = window.Message;
            _window = window;
            //window.ForceClose();

            return true;
        }

        public void CloseDialog()
        {
            if (msg == "The webpage you are viewing is trying to close the tab.\n\nDo you want to close this tab?" ||
                msg == "The webpage you are viewing is trying to close the window.\n\nDo you want to close this window?")
            // It does not seem as though this dialog is properly handled, resulting in an uncaught exception
            {
                WinButton yesButton = new WinButton(6, _window.Hwnd); // Not sure if ID will always be 6, but with the current example it is
                if (yesButton.Exists())
                {
                    yesButton.Click();
                }
            }
            if (_window != null) _window.ForceClose();
        }
    }

    //Another suggested dialogHandler replacement; haven't tried it out.
    //There was one reference here that wasn't working properly; it may have to be added to the WatiN library project instead to be usable.
    //public class JavaScriptAlertDialogHandler : BaseDialogHandler
    //{
    //    private readonly Action _onAlert;
    //    public string msg = "";

    //    public JavaScriptAlertDialogHandler(Action onAlert)
    //    {
    //        _onAlert = onAlert;
    //    }

    //    public override bool HandleDialog(Window window)
    //    {
    //        if (CanHandleDialog(window))
    //        {
    //            //_onAlert(window.Message);
    //            msg = window.Message;
    //            new WinButton(GetOKButtonId(), window.Hwnd).Click();
    //            return true;
    //        }
    //        return false;
    //    }

    //    public override bool CanHandleDialog(Window window)
    //    {
    //        return (window.StyleInHex == "94C801C5" && !ButtonWithId1Exists(window.Hwnd));
    //    }

    //    private static int GetOKButtonId()
    //    {
    //        return 2;
    //    }

    //    protected static bool ButtonWithId1Exists(IntPtr windowHwnd)
    //    {
    //        var button = new WinButton(1, windowHwnd);
    //        return button.Exists();
    //    }
    //}
}
