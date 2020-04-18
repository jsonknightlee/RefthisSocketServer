using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core.DialogHandlers;
using System.Threading;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using System.Runtime.InteropServices;
//using System.Windows.Automation;

namespace ISBNParser.Services
{
 
    public class Win7LogonHandler : BaseDialogHandler
    {
        private readonly string userName;
        private readonly string password;
        private static bool runonce;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        /// <summary>
        /// Initializes a new instance of the <see cref="LogonHandler"/> class.
        /// </summary>
        /// <param name="userName">Name of the user. Is required.</param>
        /// <param name="password">The password. If no password is required, it can be left blank (<c>null</c> or <c>String.Empty</c>). </param>
        public Win7LogonHandler(string userName, string password)
        {
            checkArgument("Username must be specified", userName, "username");

            this.userName = UtilityClass.EscapeSendKeysCharacters(userName);
            this.password = password == null ? String.Empty : UtilityClass.EscapeSendKeysCharacters(password);
            runonce = false;
        }

        /// <summary>
        /// Handles the logon dialog by filling in the username and password.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        public override bool HandleDialog(WatiN.Core.Native.Windows.Window window)
        {
            if (runonce) return false;

            if (CanHandleDialog(window))
            {
                runonce = true;
                // Find Handle of the "Frame" and then the combo username entry box inside the frame
                var systemCredentialsHwnd = GetSystemCredentialsHwnd(window);

                SetActiveWindow(window.Hwnd);
                Thread.Sleep(1000);

                SetForegroundWindow(window.Hwnd);
                Thread.Sleep(1000);

                var windowEnumarator = new WindowsEnumerator();
                var all = windowEnumarator.GetChildWindows(systemCredentialsHwnd);
                // Find input fields
                var edits = windowEnumarator.GetChildWindows(systemCredentialsHwnd, "Edit");

                Thread.Sleep(2000);
                // Enter userName
                var hwnd = new Hwnd(edits[0].Hwnd);
                hwnd.SetFocus();
                clearText(hwnd);
                hwnd.SendString(userName);

                hwnd.SendMessage(0x0102, '\t', 0);
                hwnd = null;
                Thread.Sleep(2000);

                // Enter password
                hwnd = new Hwnd(edits[1].Hwnd);
                hwnd.SetFocus();
                clearText(hwnd);
                hwnd.SendString(password);


                // Click OK button
                var windowButton = new WindowsEnumerator().GetChildWindows(window.Hwnd, w => w.ClassName == "Button" && new WinButton(w.Hwnd).Title == "OK").FirstOrDefault();
                if (windowButton != null) new WinButton(windowButton.Hwnd).Click();
                //new WinButton(1, window.Hwnd).Click();
                //new WinButton(0, window.Hwnd).Click();

                return true;
            }

            return false;
        }

        private void clearText(Hwnd item)
        {
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
            item.SendMessage(0x0102, '\b', 0);
        }

        /// <summary>
        /// Determines whether the specified window is a logon dialog.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if the specified window is a logon dialog; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandleDialog(WatiN.Core.Native.Windows.Window window)
        {
            // If a logon dialog window is claimsFolder_Found hWnd will be set.
            return GetSystemCredentialsHwnd(window) != IntPtr.Zero;
        }

        private IntPtr GetSystemCredentialsHwnd(WatiN.Core.Native.Windows.Window window)
        {
            var hwnd = NativeMethods.GetChildWindowHwnd(window.Hwnd, "SysCredential");

            // IE8 doesn't have the SysCredentails frame any more.
            if (hwnd == IntPtr.Zero)
                hwnd = NativeMethods.GetChildWindowHwnd(window.Hwnd, "DirectUIHWND");

            return hwnd;
        }

        private static void checkArgument(string message, string parameter, string parameterName)
        {
            if (UtilityClass.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(message, parameterName);
            }
        }
    }


}