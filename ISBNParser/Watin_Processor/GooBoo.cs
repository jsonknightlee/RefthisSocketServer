using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text.RegularExpressions;
using ISBNParser.Classes;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace ISBNParser.Watin_Processor
{
    public class GooBoo: TempleteProfile
    { 

    const string URL = @"https://books.google.com?hl=en";
    const int JobID = 0;
    const int WebsiteID = 1;
   // const int InShopID = 0;
   // const string STORENAME = "Table Bay Mall";

    // Check if ISBN exists first?
    List<IsbnItem> isbnList;
    DateTime start;
    static IE productItem;
    int productCount = 0;

    DataAccess da = new DataAccess();

    public GooBoo(string isbn)
    {
        try
        {
            initWebDriver();
            Settings.Instance.AutoMoveMousePointerToTopLeft = false;
            isbnList = new List<IsbnItem>();
            start = DateTime.Now;


            

         //   maxDays = da.GetMostRecentPostDate(WebsiteID);
            GoToWebsite();

            NavigateToBook(isbn);
            Search(isbn);
            FilterResults();
            WriteResults();

            browser.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void NavigateToBook(string isbn)
    {

        browser.WaitForComplete();

        if (!browser.Text.Contains("full-text books")) Thread.Sleep(1000);

            TextField q = browser.TextField(Find.ByName("q"));
            q.Value = isbn;

            Div divBtnSearch = browser.Div(Find.ById("oc-search-button-box"));
            divBtnSearch.Buttons[0].Click();


        //TODO if not on the correct page then we have to load IE window again

     //  browser.WaitForComplete();
     //  Thread.Sleep(1000);

    }

    public void GoToWebsite()
    {
        try
        {
            GoToSpecialWait(browser, URL, 0);
            //Thread.Sleep(250);
            //ieDetail = WaitForBrowser(ieDetail);




            //if (browser.isBusy())
               // Thread.Sleep(4000);
        }
        catch (System.Runtime.InteropServices.COMException ce)
        {
            Console.WriteLine(ce.Message);
            //browser is locked up.
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            da.RecordDetailedErrorData(e, WebsiteID, 1);
        }
    }

        public void Search(string isbn)
        {
            bool result = false;
            // SELENIUM

            //   try
            //   {
            //       // If Navigation is required enter it here
            //       string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            //       ieo = new InternetExplorerOptions();
            //       driver = new InternetExplorerDriver(filePath, ieo, TimeSpan.FromMinutes(5));
            //       driver = (IWebDriver)Browser.AttachTo<IE>(Find.ByTitle(browser.Title));
            //   }
            //   catch(Exception e)
            //   {
            //       Console.WriteLine(e.Message);
            //   }


            try
            {
               // Div container = browser.Div(Find.ByClass("srg"));

                DivCollection bookDivs = browser.Divs.Filter(Find.ByClass("g"));



                if (!result)
                {
                    
                    foreach(Div div in bookDivs)
                    {
                        div.Links[0].Click();

                        Table isbnTable = browser.Table(Find.ById("metadata_content_table"));

                        if (isbnTable.OuterText.Contains(isbn))
                        {
                            result = true;

                            PullReferences();
                        }

                    }
                }



            }
            catch (System.Runtime.InteropServices.COMException ce)
            {
                Console.WriteLine(ce.Message);
                //browser is locked up.
                return;
            }
            catch (Exception e)
            {
                //  productItem.Close();
                da.RecordDetailedErrorData(e, WebsiteID, 1);
                //  isErrorred = true;

                //   if (productItem != null)
                //       productItem.ForceClose();
            }
            finally
            {
                //if (CheckIfTooManyErrors()) allPagesDone = true;
            }

        }

    public void SaveImage(string filePath, string name)
    {
        string dirPath = @"C:\Users\test01\Desktop\Images";
        string fileName = @"C:\Users\test01\Desktop\Images\" + name + ".jpg";

        if (File.Exists(fileName))
            return;

        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Create a new directory
            DirectoryInfo imageDirectory = Directory.CreateDirectory(dirPath);
            Console.WriteLine($"Directory '{Path.GetFileName(dirPath)}' was created successfully in {Directory.GetParent(dirPath)}");

            // Image I'm trying to download from the web
            //string filePath = @"http://ilarge.lisimg.com/image/12056736/1080full-jessica-clements.jpg";

            using (WebClient _wc = new WebClient())
            {
                _wc.DownloadFile(new Uri(filePath), fileName);
                _wc.Dispose();
            }

            Console.WriteLine("\nFile successfully saved.");
        }

        catch (Exception e)
        {
            while (e != null)
            {
                productItem.Close();
                da.RecordDetailedErrorData(e, WebsiteID, 1);
                Console.WriteLine(e.Message + " Line: 438");
                isErrorred = true;

                if (productItem != null)
                    productItem.ForceClose();
            }
        }

        if (System.Diagnostics.Debugger.IsAttached)
        {
            // Console.WriteLine("Press any key to continue . . .");
            // Console.ReadKey(true);
        }
    }
    public void ClearAllFields(IsbnItem item)
    {
        //   race.Brand = "";
        //   race.Comments = "";
        //   race.Description = "";
        //   race.Price = "";
        //   race.PriceNonSpecial = "";
        //   race.MainBarcode = "";
        //   race.ProductImage = "";
        //   race.ProductImageLg = "";
        //   race.ProductInstructions = "";
        //   race.SavedAmount = "";
        //   race.TagLine = "";
        //   race.URL = "";
        //   race.UnitOfMeasure = "";
        //   race.StoreName = "";
        //   race.ListingAge = "";
        //   race.isPosted = false;
        //   race.isSaved = false;
        //   
        //   race.InsertDate = DateTime.Now;
        //   race.PostDate = DateTime.Now;
        //
        //    race.WebsiteID = 0;
        //    race.StoreName = "";
        //    race.JobID = JobID;
        //    race.InShopID = InShopID;


    }

    //  public void RecordListingDetails(Link details, JobListing job)
    //  {
    //      if (ieDetail == null) ieDetail = new IE();
    //      try
    //      {
    //          GoToSpecialWait(ieDetail, details.Url);
    //      }
    //      catch (Exception e)
    //      {
    //          da.RecordDetailedErrorData(e, WebsiteID, 1);
    //      }
    //
    //      DivCollection divDescriptions = ieDetail.Divs.Filter(Find.ByClass("generic-details-text"));
    //      foreach (Div div in divDescriptions)
    //      {
    //          job.Description += div.Text;
    //          job.Contact += extractEmailFromText(div.Text);
    //      }
    //
    //      //This is here just in case we can't parse the number of the jobid
    //      try { job.JobID = int.Parse(ieDetail.Label(Find.ByText("Job ID:")).NextSibling.Text); }
    //      catch { Thread.Sleep(100); }
    //      job.URL = ieDetail.Url;
    //      job.WebsiteID = WebsiteID;
    //  }

    public void FilterResults()
    {

    }

        public void PullReferences()
        {

            LinkCollection refOps = browser.Links.Filter(Find.ByClass("gb-button "));

            string url = refOps[0].Url;
            var strContent = "";

            var webRequest = WebRequest.Create(@url);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }


            //           @book{ grisham1998rainmaker,
            // title ={ The Rainmaker},
            // author ={ Grisham, J.},
            // isbn ={ 9780099271277},
            // url ={
            //               https://books.google.de/books?id=s4B5PwAACAAJ},
            //                   year ={ 1998},
            // publisher ={ Arrow}
            //               }

            string reference = "";

            if (strContent.Contains("lccn={"))
            {
                string[] arrPart1 = strContent.Split(new string[] { "title" }, StringSplitOptions.None);

                string formatReference = arrPart1[1].Replace("\n", "").Replace("author={", "").Replace("isbn={", "").Replace("lccn={", "").Replace("year={", "").Replace("publisher={", "").Replace("={", "");

                string[] arrPart2 = formatReference.Split(new string[] { "}," }, StringSplitOptions.None);

                string publisher = arrPart2[6].Replace("}}", "").Trim();

                // beware we do not have subtitle or edition included yet
                reference = String.Format("{0} ({1}). {2}. {3}.", arrPart2[1].Trim(), arrPart2[5].Trim(), arrPart2[0].Trim(), publisher);
            }
            else
            {
                string[] arrPart1 = strContent.Split(new string[] { "title" }, StringSplitOptions.None);

                string formatReference = arrPart1[1].Replace("\n", "").Replace("author={", "").Replace("isbn={", "").Replace("year={", "").Replace("publisher={", "").Replace("={", "");

                string[] arrPart2 = formatReference.Split(new string[] { "}," }, StringSplitOptions.None);

                string publisher = arrPart2[5].Replace("}}", "").Trim();

                // beware we do not have subtitle or edition included yet
                reference = String.Format("{0} ({1}). {2}. {3}.", arrPart2[1].Trim(), arrPart2[4].Trim(), arrPart2[0].Trim(), publisher);
            }



            MessageBox.Show(reference);
            
            // Can use this to download?
            using (var client = new WebClient())
            {
                client.DownloadFile(url, "bibtex.txt");
            }

        }

    public void WriteResults()
    {
      //  foreach (IsbnItem pro in RaceDetails)
      //      da.SaveRaceItem(pro);
      //
      //  da.ProcessLog("Saved job listings", RaceDetails.Count, WebsiteID, start);
      //  Console.WriteLine("Checkers page one Written");
    }
}
}