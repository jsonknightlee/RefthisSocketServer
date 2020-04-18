using System;
using System.Windows;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;
using WatiN.Core.DialogHandlers;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ISBNParser.Classes;
using ISBNParser.Watin_Processor;
using OpenQA.Selenium.IE;
using OpenQA.Selenium;


namespace ISBNParser.Watin_Processor
{
        public class TempleteProfile
        {
            public static IE browser { get; set; }
            //public IJavaScriptExecutor js; 
            DataAccess da;
            string errorComments;

            public bool isErrorred;
            public int errorCount;

            //moved all the reused parameters/variables into a subclass that can be shared.
            public string errorPostion;

            // Selenium
            public static IWebDriver driver;
            public static InternetExplorerOptions ieo;
            public static IJavaScriptExecutor theo;

            public TempleteProfile()
            {
                da = new DataAccess();
                // browser = CreateIEWindow(0);
                initWebDriver();
                errorComments = "";

                if (browser == null)
                {
                    //da.ProcessLog("Unable to create browser");
                    //unable to open a new IE window
                    browser = CreateIEWindow(0);
                }
                //System.Windows.MessageBox.Show(errorComments);
            }

            public IE CreateIEWindow(int count)
            {
                IE temp;
                try
                {


                //temp = new IE(true);
                // temp = new IE("https://www.google.com", true);
                IE tempone = new IE("https://books.google.com");

                temp = tempone;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    da.RecordDetailedErrorData(e, 0, 1);

                    Thread.Sleep(250);
                    count++;
                    if (count > 100) return null;
                    temp = CreateIEWindow(count);
                }

                return temp;
            }

            public IE WaitForBrowser(IE ie, string URL = null)
            {
                int count = 0;
                string url = URL;

                while (count < 200)
                {
                    count++;
                    Thread.Sleep(1000);

                    try
                    {
                        //if (!ie.isBusy)
                        if (ie.Text.Contains("Google"))
                            return ie;
                    }
                    catch
                    {
                        ie = null;
                        break;
                    }
                }

                //waiting timed out or failed
                try
                {
                    if (ie != null && count >= 200)
                    {
                        url = ie.Url;
                        ie.Close();
                    }

                    ie = CreateIEWindow(0);
                    ie.GoTo(url);
                }
                catch
                {
                }

                return ie;
            }

            public IE GoToSpecialWait(IE ie, string URL, int tryCount = 0)
            {
                int count = 0;
                try
                {
                    if (ie == null) ie = new IE();
                    ie.GoToNoWait(URL);
                }
                catch
                { }

                while (count < 200)
                {
                    count++;
                    Thread.Sleep(1000);

                    try
                    {


                        return ie;
                    }
                    catch
                    {
                        ie = null;
                        break;
                    }
                }

                //waiting timed out or failed
                try
                {
                    if (tryCount > 3) return null;

                    if (ie != null && count >= 200)
                    {
                        ie.Close();
                    }

                    ie = new IE();
                    ie = GoToSpecialWait(ie, URL, ++tryCount);
                }
                catch
                {
                }

                return ie;
            }

            public void CloseBrowser()
            {
                try
                {
                    browser.Close();
                    browser = null;
                }
                catch
                {
                }
            }

            //Initialize the selenium webdriver
            public static string initWebDriver()
            {
                Teardown();
                return Setup();
            }

            //Tear down the selenium webdriver
            public static void Teardown()
            {
                try
                {
                    //si.ie.Close();
                    //closeIE();
                    //closeWebdriver();
                    if (driver != null)
                        driver.Quit();
                    theo = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //Ignore errors if unable to close the browser
                }
                //Assert.AreEqual("", verificationErrors.ToString());
            }
            //Setup the selenium webdriver.
            public static string Setup()
            {
                try
                {
                    if (driver != null)
                    {
                        driver.Close();
                        driver = null;
                    }
                }
                catch
                { }

                try
                {
                    Microsoft.Win32.RegistryKey rkHKLM = Microsoft.Win32.Registry.CurrentUser;
                    Microsoft.Win32.RegistryKey rkZoom = rkHKLM.CreateSubKey(@"Software\Microsoft\Internet Explorer\Zoom");
                    rkZoom.SetValue("ZoomFactor", 0x186A0);
                    string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                    ieo = new InternetExplorerOptions { IgnoreZoomLevel = true };
                    driver = new InternetExplorerDriver(filePath, ieo, TimeSpan.FromMinutes(5));
                    //si.d.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                    //si.d.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
                    //Selenium.SetSpeed("1000");
                    theo = driver as IJavaScriptExecutor;
                    // si.verificationErrors = new StringBuilder();
                    Thread.Sleep(1000);
                    //browser = Browser.AttachTo<IE>(Find.ByTitle(driver.Title));

                    browser = Browser.AttachTo<IE>(Find.ByTitle(driver.Title));

                    return "";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return string.Format("Method: Setup - {0}", e.Message);
                    //showExceptionMessage(e);
                }
            }


            public void LogError(string errorMessage)
            {
                if (!errorComments.Contains(errorMessage + "|")) errorComments += errorMessage + "|";
            }

            public void RecordError(Exception e, string location)
            {
                string newError = "";

                //Ignore certain kinds of errors, related to connection issues.
                switch (e.GetType().ToString())
                {
                    case "WatiN.Core.Exceptions.TimeoutException":
                        //unavoidable
                        break;
                    case "System.UnauthorizedAccessException":
                        //something accessed out of order
                        break;
                    case "WatiN.Core.Exceptions.ElementNotFoundException":
                        //not in the place we think we are.
                        //errorCount++;
                        isErrorred = true;
                        newError = location + ": " + e.Message + " " + e.TargetSite;
                        break;
                    default:
                        if (e.Message.Contains("The RPC server is unavailable."))
                        {
                            //unavoidable
                        }
                        else
                        {
                            isErrorred = true;
                            newError = location + ": " + e.Message + " " + e.TargetSite;
                        }
                        break;
                }

                //Record more details on the exception (line number, method); suppress the same ones we normally did.
                //if (isErrorred) si.RecordDetailedErrorData(e, p, 1);

                if (newError.Length > 0)
                {
                    if (newError.Contains("Could not find INPUT (hidden) or INPUT (password) or INPUT (text) or INPUT (textarea) or TEXTAREA element tag element tag matching criteria"))
                    {
                        newError = newError.Replace("Could not find INPUT (hidden) or INPUT (password) or INPUT (text) or INPUT (textarea) or TEXTAREA element tag element tag matching criteria", "Missing element: ");
                    }

                    if (newError.Contains("at http"))
                    {
                        //don't need long URLs in here; just take up too much room.
                        int startIndex = newError.IndexOf("at http");
                        int endIndex = newError.IndexOf(" ", startIndex + 4);

                        if (endIndex != -1)
                        {
                            newError = newError.Substring(0, startIndex) + newError.Substring(endIndex);
                        }
                        else
                        {
                            newError = newError.Substring(0, startIndex);
                        }
                    }

                    if (!errorComments.Contains(newError))
                    {
                        if (errorPostion != null && errorPostion.Length > 0)
                        {
                            newError = " loc: " + errorPostion;
                        }

                        LogError(newError);
                    }
                }
            }


            public void RecordError_FillInFields(Exception e)
            {
                RecordError(e, "Search");
            }



            public bool HandleError_GoToWebsite(Exception e)
            {
                if (browser == null) browser = new IE();
                errorCount++;

                RecordError(e, "Login");


                if (errorCount > 3)
                {
                    //too many errors, site will need looking at.

                    return true;
                }

                return false;
            }

            public void ClearInsertData()
            {
                isErrorred = false;
            }

            public void LoadWebsite(string websiteAddr)
            {
                try
                {
                    browser.GoTo(websiteAddr);
                }
                catch { }

            //    if (browser.isBusy()) Thread.Sleep(2500);
                if (browser.Text == null || browser.Text.Length < 100) Thread.Sleep(2500);
             //   if (browser.isBusy()) Thread.Sleep(2500);
                if (browser.Text == null || browser.Text.Length < 100) Thread.Sleep(2500);

                if (browser.Text != null && browser.Text.Contains("There is a problem with this website's security certificate"))
                {
                    browser.Link(Find.ByName("overridelink")).Click();
                    errorComments += "Site has certficate errors" + Environment.NewLine;
                }
            }


            public void CheckForPageNotDisplayedError()
            {
                //When sites are overloaded, can get html errors back instead of data(503 errors normally).
                //In most cases a page refresh loads back what we want.

                Thread.Sleep(100);

                if (browser.Text.Length < 500)
                {
                    Thread.Sleep(1000);
                    browser.Refresh();
                    Thread.Sleep(2000);
                }
            }

            public IE WaitForBrowserWindow(string property, string value)
            {
                return WaitForBrowserWindow(property, value, null);

            }
            public IE WaitForBrowserWindow(string property, string value1, string value2)
            {
                IE browser = null;

                int tryCount = 0;
                while (tryCount < 80 && browser == null)
                {
                    foreach (IE test in IE.InternetExplorers())
                    {
                        switch (property.ToLower())
                        {
                            case "url":
                                if (test.Url == value1) browser = test;
                                if (value2 != null && test.Url == value2) browser = test;
                                break;
                            case "title":
                                if (test.Title == value1) browser = test;
                                if (value2 != null && test.Title == value2) browser = test;
                                break;
                        }
                    }

                    Thread.Sleep(500);
                    tryCount++;
                }

                return browser;
            }

            public bool WaitForItemToExist(Element itemToCheckFor, int time_out_seconds = 99, string itemToCheckForID = null, IE browser = null)
            {
                int waitDone = 0;

                while (waitDone < time_out_seconds)
                {
                    Thread.Sleep(1000);
                    if (itemToCheckFor.Exists)
                    {
                        return true;
                    }
                    else if (itemToCheckForID != null && browser != null)
                    {
                        itemToCheckFor = browser.Element(Find.ById(itemToCheckForID));
                    }
                    waitDone++;
                }
                return false;
            }

            public int WaitForItemToExistCounter(Element itemToCheckFor, int time_out_seconds = 30, string itemToCheckForID = null, IE browser = null)
            {
                int waitDone = 0;

                while (waitDone < time_out_seconds)
                {
                    Thread.Sleep(3000);
                    if (itemToCheckFor.Exists)
                    {
                        return -1;
                    }
                    else if (itemToCheckForID != null && browser != null)
                    {
                        itemToCheckFor = browser.Element(Find.ById(itemToCheckForID));
                    }
                    waitDone++;
                }
                return -1;
            }

            public bool WaitForEitherItemToExist(Element itemToCheckFor1, Element itemToCheckFor2, int time_out_seconds = 99)
            {
                //From some searches, we can end up in more than one place.
                //If no results, often on the original search page.
                //If multiple results, a page with a table containing the list.
                //If a single result matches, straight to the page for that specific claim.

                int waitDone = 0;

                while (waitDone < time_out_seconds)
                {
                    Thread.Sleep(1000);
                    if (itemToCheckFor1.Exists || itemToCheckFor2.Exists)
                    {
                        return true;
                    }
                    waitDone++;
                }
                return false;
            }

            public bool WaitUntilDoesntExist(Element itemToCheckFor, int time_out_seconds = 99)
            {
                int waitDone = 0;

                while (waitDone < time_out_seconds)
                {
                    Thread.Sleep(1000);
                    if (!itemToCheckFor.Exists)
                    {
                        return true;
                    }
                    waitDone++;
                }
                return false;
            }

            public string getValueByCellLabel(Element FindItem)
            {
                if (FindItem.Exists)
                {
                    if (FindItem.NextSibling == null)
                    {
                        if (FindItem.Parent.NextSibling != null)
                        {
                            FindItem = FindItem.Parent;
                        }
                    }

                    if (FindItem.NextSibling != null && FindItem.NextSibling.Text != null && FindItem.NextSibling.Text.Length > 0)
                    {
                        return FindItem.NextSibling.Text;
                    }
                }
                return "";
            }

            public string getValueFromWithinLabeledCell(Element FindItem)
            {
                string result = "";
                //At least one website only has one TableCell per row, with both the label and the value in that single cell.

                if (FindItem.Exists)
                {
                    result = FindItem.Text;
                    int offset = result.IndexOf(':');

                    if (offset == -1)
                    {
                        offset = result.LastIndexOf(' ');
                    }

                    if (offset > 0)
                    {
                        result = FindItem.Text.Substring(offset + 1).Trim();
                    }

                }

                return result;
            }

            public bool Click(IE currIE, WatiN.Core.Element currItem)
            {
                //We've been having intermittent problems with a few sites that would hit 'this page cannot be displayed' messages.
                //Broke out the quick code to force a refresh when the page did not get loaded properly; so can be reused easier.
                try
                {
                    if (currIE.Html.Length < 100)
                    {
                        currIE.Refresh();
                    }

                    currItem.Click();

                    if (currIE.Html.Length < 100)
                    {
                        currIE.Refresh();
                    }
                    return true;
                }
                catch
                { }

                return false;
            }

            public bool ClickLongWait(IE currIE, WatiN.Core.Element currItem)
            {
                //Some sites take far longer than the default IE wait to respond.

                try
                {
                    currItem.ClickNoWait();

                    int max = 0;
                    Thread.Sleep(500);


               //     while (browser.isBusy() && max < 60)
               //     {
               //         Thread.Sleep(500);
               //         max++;
               //     }


                    return true;
                }
                catch
                { }

                return false;
            }

            public string ClickHandleAlertDialog(IE currIE, WatiN.Core.Element currItem)
            {
                //Multiple sites have dialog messages popup in response to clicking a button or link.
                //This just collects the standard way of handling it in a single place for easy reuse.
                AnyDialogHandler adhdl = new WatiN.Core.DialogHandlers.AnyDialogHandler();
                string errormsg = "";

                currIE.AddDialogHandler(adhdl);

                try
                {
                    Thread.Sleep(500);

                    int count = 0;

                    if (currItem.Exists == true)
                    {
                        currItem.ClickNoWait();

                        //Backup wait: in case of extremely slow page loads.

                        //Add more time to wait load page
                        Thread.Sleep(4000);
                        while (count < 50)
                        {
                            Thread.Sleep(500);

                            if (adhdl.Exists() || adhdl.msg.Length > 0)
                            {
                                // Capture alert box text.
                                errormsg = adhdl.msg;
                                adhdl.CloseDialog();
                                return errormsg;
                            }

                       //     if (!browser.isBusy())
                       //     {
                       //         //page has finished loading
                       //         break;
                       //     }

                            count++;
                        }

                        return errormsg;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // Throws the error back 
                    throw;
                }
                finally
                {
                    try
                    {
                        currIE.RemoveDialogHandler(adhdl);
                    }
                    catch
                    {
                    }
                }
            }

            public string ClickHandleOKCancelDialog(IE currIE, WatiN.Core.Element currItem, string strButtonToClick)
            {
                //Multiple sites have dialog messages popup in response to clicking a button or link.
                //This just collects the standard way of handling it in a single place for easy reuse.
                WatiN.Core.DialogHandlers.ConfirmDialogHandler adhdl = new WatiN.Core.DialogHandlers.ConfirmDialogHandler();
                string errormsg = "";

                currIE.AddDialogHandler((WatiN.Core.DialogHandlers.ConfirmDialogHandler)adhdl);

                int count = 0;

                try
                {
                    Thread.Sleep(500);
                    if (currItem.Exists == true)
                    {
                        currItem.ClickNoWait();
                        Thread.Sleep(1000);

                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();

                        while (stopwatch.Elapsed.TotalMilliseconds < 1000d)
                        {
                            Thread.Sleep(100);
                            if (adhdl.Exists() || adhdl.Title.Length > 0)
                            {
                                // Capture alert box text.
                                errormsg = adhdl.Title;

                                return errormsg;
                            }
                        }
                        stopwatch.Stop();
                        if (stopwatch.Elapsed.TotalMilliseconds >= 1000d)
                        {
                            //Stopwatch expiring means no dialog was found.
                            //If a dialog did exist, and the dialogWatcher didn't find it, it would be blocking IE from any WatiN controls working.

                            //errormsg = "Had to go to CustomDialogHandler";
                            //CPlusPlusCalls.CustomAlertDialogHandler.Execute(si); 
                        }


                        //Backup wait: in case of extremely slow page loads.

                        if (errormsg.Length == 0) //frames will make this just keep waiting forever
                        {
                            Thread.Sleep(500);

                         //  while (browser.isBusy() && count < 50)
                         //  {
                         //      Thread.Sleep(500);
                         //      count++;
                         //  }
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }

                        return errormsg;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "error in processing";
                }
                finally
                {
                    if (strButtonToClick == "OK")
                    {
                        adhdl.OKButton.Click();
                    }
                    else
                    {
                        adhdl.CancelButton.Click();
                    }
                    currIE.RemoveDialogHandler(adhdl);
                }
            }

            public string ClickHandleVBAlertDialog(IE currIE, WatiN.Core.Element currItem)
            {
                //Multiple sites have dialog messages popup in response to clicking a button or link.
                //This just collects the standard way of handling it in a single place for easy reuse.
                WatiN.Core.DialogHandlers.VbScriptMsgBoxDialogHandler adhdl = new WatiN.Core.DialogHandlers.VbScriptMsgBoxDialogHandler(WatiN.Core.DialogHandlers.VbScriptMsgBoxDialogHandler.Button.OK);
                string errormsg = "";

                currIE.AddDialogHandler(adhdl);

                try
                {
                    currItem.ClickNoWait();

                    try
                    {
                        adhdl.WaitUntilHandled(5);

                        if (adhdl.HasHandledDialog)
                        {
                            errormsg = "Unknown error";
                        }
                    }
                    catch
                    {

                    }

                    currIE.RemoveDialogHandler(adhdl);
                    adhdl = null;


                    return errormsg;
                }
                catch
                {
                    return "error in processing";
                }
            }

            public string ClickHandleGoToSite(IE currIE, string URL)
            {
                //Multiple sites have dialog messages popup in response to clicking a button or link.
                //This just collects the standard way of handling it in a single place for easy reuse.
                WatiN.Core.DialogHandlers.AnyDialogHandler adhdl = new WatiN.Core.DialogHandlers.AnyDialogHandler();
                string errormsg = "";

                if (currIE == null) currIE = new IE();

                currIE.AddDialogHandler(adhdl);

                try
                {
                    currIE.GoToNoWait(URL);


                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    while (stopwatch.Elapsed.TotalMilliseconds < 50000d)
                    {
                        if (adhdl.Exists())
                        {
                            // Capture alert box text.
                            errormsg = adhdl.Message;


                            adhdl.OKButton.Click();
                            break;
                        }

                     //   if (!browser.isBusy())
                     //   {
                     //       if (stopwatch.Elapsed.TotalMilliseconds < 5000d)
                     //       {
                     //           break;
                     //       }
                     //   }
                    }
                    stopwatch.Stop();

                    adhdl = null;


                    return errormsg;
                }
                catch
                {
                    return "error in processing";
                }
                finally
                {
                    currIE.RemoveDialogHandler(adhdl);
                }
            }


            //Type text much more quickly than Watin's function, by using SendKeys.
            public void FastType(WatiN.Core.Element currItem, String keys)
            {
                FastType(browser, currItem, keys);
            }

            public void FastType(IE currIE, WatiN.Core.Element currItem, String keys)
            {
                //Functions.SetForegroundWindowIE(currIE);
                currItem.Focus();
                System.Windows.Forms.SendKeys.SendWait(keys);
            }

            public bool CheckIfTooManyErrors()
            {
                if (browser == null) return false;

                //if (browser.DialogWatcher != null && browser.DialogWatcher.Count > 0)
                //{
                //    browser.DialogWatcher.Clear();
                //}

                if (isErrorred)
                {
                    errorCount++;

                    //more than 3 errors in a row (excluding those that can be ignored).
                    if (errorCount > 4)
                    {
                        errorComments = "Too many errors on this site: (" + errorCount + ")  " + errorComments;

                        return true;
                    }
                }
                else
                {
                    errorCount = 0;
                }

                return false;
            }

            public void closeIE()
            {
                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("IEXPLORE");
                foreach (System.Diagnostics.Process proc in procs)
                {
                    proc.Kill(); // Close it down.
                }
            }
            public void closeWebdriver()
            {
                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("IEDriverServer");
                foreach (System.Diagnostics.Process proc in procs)
                {
                    proc.Kill(); // Close it down.
                }
            }

            public string GetMonth(int value)
            {
                string Month = "";
                switch (value)
                {
                    case 1:
                        Month = "Jan";
                        break;
                    case 2:
                        Month = "Feb";
                        break;
                    case 3:
                        Month = "Mar";
                        break;
                    case 4:
                        Month = "Apr";
                        break;
                    case 5:
                        Month = "May";
                        break;
                    case 6:
                        Month = "Jun";
                        break;
                    case 7:
                        Month = "Jul";
                        break;
                    case 8:
                        Month = "Aug";
                        break;
                    case 9:
                        Month = "Sep";
                        break;
                    case 10:
                        Month = "Oct";
                        break;
                    case 11:
                        Month = "Nov";
                        break;
                    case 12:
                        Month = "Dec";
                        break;
                    default:
                        break;
                }

                return Month;
            }

            public bool DatesCoincide(int ourStartDay, int ourStartMonth, int ourStartYear, int ourEndDay, int ourEndMonth, int ourEndYear, string theirStringStartDate, string theirStringEndDate)
            {
                try
                {
                    DateTime ourStartDate = new DateTime(ourStartYear, ourStartMonth, ourStartDay);
                    DateTime ourEndDate = new DateTime(ourEndYear, ourEndMonth, ourEndDay);
                    DateTime theirStartDate = DateTime.Parse(theirStringStartDate);
                    DateTime theirEndDate = DateTime.Parse(theirStringEndDate);

                    // Checking symmetrically for overlap, that is either their dates must fall within ours or viceversa
                    if ((theirStartDate.Ticks >= ourStartDate.Ticks && theirStartDate.Ticks <= ourEndDate.Ticks) &&
                        (theirEndDate.Ticks >= ourStartDate.Ticks && theirEndDate.Ticks <= ourEndDate.Ticks))
                    {
                        return true;
                    }
                    else if ((ourStartDate.Ticks >= theirStartDate.Ticks && ourStartDate.Ticks <= theirEndDate.Ticks) &&
                        (ourEndDate.Ticks >= theirStartDate.Ticks && ourEndDate.Ticks <= theirEndDate.Ticks))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false; // May want to return something different to indicate bad date input (as in need different return type), but for now just return false
                }
            }
            public bool DatesCoincide(string ourStringStartDate, string ourStringEndDate, string theirStringStartDate, string theirStringEndDate)
            {
                try
                {
                    DateTime ourStartDate = DateTime.Parse(ourStringStartDate);
                    DateTime ourEndDate = DateTime.Parse(ourStringEndDate);
                    DateTime theirStartDate = DateTime.Parse(theirStringStartDate);
                    DateTime theirEndDate = DateTime.Parse(theirStringEndDate);

                    // Checking symmetrically for overlap, that is either their dates must fall within ours or viceversa
                    if ((theirStartDate.Ticks >= ourStartDate.Ticks && theirStartDate.Ticks <= ourEndDate.Ticks) &&
                        (theirEndDate.Ticks >= ourStartDate.Ticks && theirEndDate.Ticks <= ourEndDate.Ticks))
                    {
                        return true;
                    }
                    else if ((ourStartDate.Ticks >= theirStartDate.Ticks && ourStartDate.Ticks <= theirEndDate.Ticks) &&
                        (ourEndDate.Ticks >= theirStartDate.Ticks && ourEndDate.Ticks <= theirEndDate.Ticks))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false; // May want to return something different to indicate bad date input (as in need different return type), but for now just return false
                }
            }

            //public bool ComparePatientName(string pfname, string webfname, string plname, string weblname)
            //{
            //    if (string.Compare(pfname, webfname, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreSymbols) == 0)
            //    {
            //        if (string.Compare(plname, weblname, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreSymbols) == 0)
            //        {
            //            return true;
            //        }
            //    }

            //    return false;
            //}

            public bool CheckPatientName(string pfname, string plname, string webfname, string weblname)
            {
                pfname = Regex.Replace(pfname, "[^a-zA-Z]", "");
                plname = Regex.Replace(plname, "[^a-zA-Z]", "");
                webfname = Regex.Replace(webfname, "[^a-zA-Z]", "");
                weblname = Regex.Replace(weblname, "[^a-zA-Z]", "");

                string fullwebname = webfname.ToUpper() + " " + weblname.ToUpper();
                string fullpname = pfname.ToUpper() + " " + plname.ToUpper();

                if ((fullwebname.Contains(pfname.ToUpper()) && fullwebname.Contains(plname.ToUpper()))
                    || (fullpname.Contains(webfname.ToUpper()) && fullpname.Contains(weblname.ToUpper())))
                    return true;
                else
                    return false;
            }

            public string[] ParseCommaName(string name)
            {
                string[] nameTokens = new string[2];

                if (name.Contains(","))
                {
                    nameTokens[0] = name.Remove(0, name.IndexOf(",") + 1).Trim();
                    nameTokens[1] = name.Remove(name.IndexOf(",")).Trim();
                }
                // Otherwise just try to parse it as having a space
                else
                {
                    name = name.Trim();

                    if (name.Contains(" "))
                    {
                        nameTokens[1] = name.Remove(0, name.IndexOf(" ") + 1).Trim();
                        nameTokens[0] = name.Remove(name.IndexOf(" ")).Trim();
                    }
                    else
                    {
                        nameTokens[0] = name;
                        nameTokens[1] = "";
                    }
                }
                return nameTokens;
            }


            public bool CompareTotalChargesExact(string patientTotCharges, string websiteTotCharges)
            {
                double dPatTot = 0;
                double dsiteTot = 0;

                if ((patientTotCharges == null && websiteTotCharges != null) ||
                    (patientTotCharges != null && websiteTotCharges == null))
                {
                    return false;
                }

                if (!Double.TryParse(patientTotCharges.Replace("$", ""), out dPatTot))
                {
                    return false;
                }

                if (!Double.TryParse(websiteTotCharges.Replace("$", "").Replace(",", ""), out dsiteTot))
                {
                    return false;
                }

                if (dsiteTot != dPatTot)
                {
                    return false;
                }

                return true;
            }

            public bool CompareTotalCharges10Percent(string patientTotCharges, string websiteTotCharges)
            {
                double dPatTot = 0;
                double dsiteTot = 0;

                if (!Double.TryParse(patientTotCharges.Replace("$", ""), out dPatTot))
                {
                    return false;
                }

                if (!Double.TryParse(websiteTotCharges.Replace("$", ""), out dsiteTot))
                {
                    return false;
                }

                if (dsiteTot != dPatTot)
                {
                    double ratio = 0;
                    ratio = (dsiteTot - dPatTot) / dsiteTot;

                    if (ratio > 0.1 || ratio < -0.1) return false;
                }

                return true;
            }

            public WatiN.Core.Frame FindFrame(IE context, string attributeName, string attributeValue)
            {
                int timeout = 0;

                while (timeout < 50)
                {
                    foreach (WatiN.Core.Frame frame in context.Frames)
                    {
                        if (frame.GetAttributeValue(attributeName) != null)
                        {
                            if (frame.GetAttributeValue(attributeName).Contains(attributeValue)) // May not wish to use Contains in some cases
                            {

                                return frame;
                            }
                        }
                    }
                    Thread.Sleep(500);
                    timeout++;
                }

                return null;
            }

            public WatiN.Core.Frame FindFrame(Frame context, string attributeName, string attributeValue)
            {
                int timeout = 0;

                while (timeout < 50)
                {
                    foreach (WatiN.Core.Frame frame in context.Frames)
                    {
                        if (frame.GetAttributeValue(attributeName) != null)
                        {
                            if (frame.GetAttributeValue(attributeName).Contains(attributeValue)) // May not wish to use Contains in some cases
                            {

                                return frame;
                            }
                        }
                    }
                    Thread.Sleep(500);
                    timeout++;
                }

                return null;
            }

            public string parseTable(WatiN.Core.Table tblData, bool captureTableHeaders)
            {
                string result = "";

                if (tblData.OwnTableRows.Count > 0)
                {

                    if (captureTableHeaders)
                    {
                        if (tblData.InnerHtml.Contains("<th>"))
                        {
                            foreach (TableRow tr in tblData.OwnTableRows)
                            {
                                if (tr.InnerHtml.Contains("<th>"))
                                {
                                    foreach (Element ele in tr.Children())
                                    {
                                        if (ele.TagName == "th")
                                        {
                                            if (ele.Text != null) result += ele.Text.Replace("\n", "").Replace("\r", "");
                                        }
                                        result += "|";
                                    }
                                    result += Environment.NewLine;
                                    break;
                                }
                            }
                        }
                    }


                    foreach (TableRow tr in tblData.OwnTableRows)
                    {
                        foreach (TableCell tc in tr.OwnTableCells)
                        {
                            if (tc.Text != null) result += tc.Text.Replace("\n", "").Replace("\r", "");
                            result += "|";
                        }
                        result += Environment.NewLine;
                    }
                }

                return result;
            }

            public string NumberAndDecimalString(string str)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || c == '.')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }

            public string getMostCommonListItem(string input)
            {
                if (input.Trim().Length == 0) return "";

                string result = input.Trim().Substring(0, input.Length - 1);
                //remove extra '|' from end to avoid empty string in list.

                string[] list = result.Split('|');

                if (list.Count() == 1) return list[0].Trim();

                var test = list
                            .GroupBy(x => x)
                            .Select(x => new { Name = x.Key, Count = x.Count() })
                            .OrderByDescending(y => y.Count);

                result = test.First().Name.Trim();

                return result;
            }


            public string GetTopStatusFromList(string values, out string Rest)
            {
                //From a pipe delimeted list, extract one item as the most important, and format the rest for the comment field.
                // For now, for HMA we are promoting denials.
                // From discussion with Shalene and Maria (with Mollie's agreement), this is probably a bad idea.

                string[] items = values.ToUpper().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                string result = "";
                Rest = "";

                int iMainResponse = 0; //assume first result gets top billing; override if there is a denial.

                //if (App.currentDB.ID == (int) App.iDatabaseAccess.HMA)
                //{
                //    if (values.Contains("DENIED") || values.Contains("DENIAL") || values.Contains("WAITING") || values.Contains("F2"))
                //    {
                //        //want to prioritize anything with denials to the top.
                //        for (int x = 0; x < items.Length; x++)
                //        {
                //            if (items[x].Contains("DENIED") || items[x].Contains("DENIAL") || items[x].Contains("WAITING") || values.Contains("F2"))
                //            {
                //                iMainResponse = x;
                //                break;
                //            }
                //        }
                //    }
                //}

                string data = "";
                for (int x = 0; x < items.Length; x++)
                {
                    if (x == iMainResponse)
                    {
                        result = items[x];
                    }
                    else
                    {
                        if (items[x].Trim().Length > 0)
                        {
                            data += items[x] + "; ";
                        }
                    }
                }
                if (data.Length > 2)
                {
                    //strip off extra separator.
                    Rest = data.Substring(0, data.Length - 2);
                }

                return result;
            }


            public Table FindLowestTableWithText(string txt, Table parentTable)
            {
                Table ret_Table = null;

                foreach (var tbl in parentTable.Tables)
                {
                    if (String.IsNullOrEmpty(tbl.Text) == false &&
                        tbl.Text.Contains(txt))
                    {
                        var ret_Table_Temp = FindLowestTableWithText(txt, tbl);
                        if (ret_Table_Temp == null)
                        {
                            return ret_Table;
                        }
                        else
                        {
                            ret_Table = ret_Table_Temp;
                        }
                    }
                }

                if (ret_Table == null)
                {
                    return parentTable;
                }
                else
                {
                    return ret_Table;
                }
            }

            public Table FindLowestTableWithText(string txt, Div parent)
            {
                Table ret_Table = null;

                foreach (var tbl in parent.Tables)
                {
                    if (String.IsNullOrEmpty(tbl.Text) == false &&
                        tbl.Text.Contains(txt))
                    {
                        var ret_Table_Temp = FindLowestTableWithText(txt, tbl);
                        if (ret_Table_Temp == null)
                        {
                            return ret_Table;
                        }
                        else
                        {
                            ret_Table = ret_Table_Temp;
                        }
                    }
                }

                if (ret_Table == null)
                {
                    return null;
                }
                else
                {
                    return ret_Table;
                }
            }

            //Eval javascript, returning an exception if applicable rather than throwing one
            public Object EvalNoThrow(string jsToExecute)
            {
                try
                {
                    Object returnValue = null;
                    //  si.js.ExecuteScript(jsToExecute);
                    return returnValue;
                }
                catch (Exception e)
                {
                    return e;
                }
            }


            public string formatAsCurrency(String currency)
            {
                Decimal currencyAsDec = Convert.ToDecimal(currency);
                String strReturn = "$" + String.Format("{0:n}", currencyAsDec);
                return strReturn;
            }

            //Created for profiles that seem to add space between dollar sign and the number. This will eliminate the dollar sign
            public string formatAsCurrencyNoSign(String currency)
            {
                Decimal currencyAsDec = Convert.ToDecimal(currency);
                String strReturn = String.Format("{0:n}", currencyAsDec);
                return strReturn;
            }

            //For now this function will dump the exception stack trace and exception message to the user.
            //This is very useful if initialization of components like Selenium fails because the error message will usually tell you what went wrong and what to fix.
            //We should add some formal error logging as well.
            public void showExceptionMessage(Exception e)
            {
                // MessageBoxResult msg = MessageBox.Show("Error. Stack trace and message:"+ e.StackTrace + "\n" + e.Message);  
            }

            public WatiN.Core.Table Wait_Until_Table_Containing_Terms_Exists(IE browser, List<string> terms, int timeout = 10)
            {
                int time = 0;
                while (time < timeout)
                {
                    foreach (var table in browser.Tables)
                    {
                        int term_match_count = 0;
                        for (int i = 0; i < terms.Count; i++)
                        {
                            if (String.IsNullOrEmpty(table.Text) == false && table.Text.Contains(terms[i]))
                            {
                                term_match_count++;
                            }
                        }

                        if (term_match_count == terms.Count)
                        {
                            var lowest_table = FindLowestTableWithText(terms[0], table);
                            return lowest_table;
                        }
                    }
                    time++;
                    Thread.Sleep(1000);
                }

                return null;
            }

            public Dictionary<string, int> Get_Tbl_Hdr_Dict(WatiN.Core.TableRow hdr_row)
            {
                var ret_dict = new Dictionary<string, int>();
                var elements = new List<WatiN.Core.Element>();
                if (hdr_row.TableCells.Count == 0)
                {
                    elements = hdr_row.ElementsWithTag("th").ToList();
                }
                else
                {
                    foreach (var t in hdr_row.TableCells)
                    {
                        elements.Add(t);
                    }
                }

                for (int i = 0; i < elements.Count; i++)
                {
                    ret_dict.Add(elements[i].Text, i);
                }

                return ret_dict;
            }

            public string getBCBSPrefix(string sUserID)
            {
                string sPrefix = sUserID.Trim().Substring(0, 3);

                if (Regex.IsMatch(sPrefix, "[a-zA-Z][a-zA-Z][a-zA-Z]"))
                {
                    //sPrefix = sUserID.Substring(3).Trim();
                }
                else if (Regex.IsMatch(sPrefix, "[a-zA-Z][a-zA-Z][0-9]{1}"))
                {
                    sPrefix = sPrefix.Substring(0, 2);
                }
                else if (Regex.IsMatch(sPrefix, "[a-zA-Z][0-9]{2}"))
                {
                    sPrefix = sPrefix.Substring(0, 1);
                }
                else
                {
                    sPrefix = "";
                }

                return sPrefix;
            }

            public string extractURLFromText(string input)
            {
                if (string.IsNullOrEmpty(input)) return null;

                if (input.Contains("http://") || input.Contains("https://") || input.Contains("www."))
                {
                    int pos = 0;
                    pos = input.IndexOf("http://");
                    if (pos <= 0) pos = input.IndexOf("https://");
                    if (pos <= 0) pos = input.IndexOf("www.");

                    if (pos > 0)
                    {
                        int pos2 = input.IndexOf("<", pos);

                        string temp = input.Substring(pos, (pos2 - pos)).Trim();

                        return temp;
                    }
                }

                return null;
            }

            public string extractEmailFromText(string data)
            {
                if (string.IsNullOrEmpty(data)) return null;

                //instantiate with this pattern 
                Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                    RegexOptions.IgnoreCase);
                //find items that matches with our pattern
                MatchCollection emailMatches = emailRegex.Matches(data);

                StringBuilder sb = new StringBuilder();

                foreach (Match emailMatch in emailMatches)
                {
                    //from code instructions/labels
                    if (emailMatch.Value == "abc@xyz.com") continue;

                    sb.Append(emailMatch.Value + "; ");
                }
                //store to file
                return sb.ToString();
            }

            public int getPostingAge(string message)
            {
                int days = 2; //need default for unknown values
                int daycount = 1;

                message = message.ToLower();

                if (message.Contains("today"))
                {
                    return 0;
                }
                else if (message.Contains("hours ago"))
                {
                    return 0;
                }
                else if (message.Contains("yesterday"))
                {
                    return 1;
                }
                else if (message.ToLower().Contains("week"))
                {
                    message = message.Replace(" weeks ago", "");
                    message = message.Replace(" week ago", "");
                    daycount = 7;
                }
                else if (message.ToLower().Contains("day"))
                {
                    message = message.Replace(" days ago", "");
                    message = message.Replace(" day ago", "");
                    daycount = 1;
                }
                else if (message.ToLower().Contains("month"))
                {
                    message = message.Replace(" months ago", "");
                    message = message.Replace(" month ago", "");
                    daycount = 30;
                }
                else
                {
                }

                if (message.Contains('+')) message.Replace("+", "");

                Int32.TryParse(message, out days);
                days = days * daycount;

                return days;
            }
        }
    }
