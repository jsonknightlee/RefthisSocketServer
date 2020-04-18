using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WatiN.Core;

namespace ISBNParser.Services
{
    public class SelectListHandler
    { 
         //Collection<string> prevMatches;
        Collection<string> results;

    public SelectListHandler()
    {

        //prevMatches = new Collection<string>();
        results = new Collection<string>();
    }

    public void initSearch(WatiN.Core.SelectList dataList, string[] searchTerms)
    {
        results.Clear();
        //find all matching entries and add them to the results collection.
        foreach (string sItem in searchTerms)
        {
            if (sItem != null && sItem.Length > 0)
            {
                try
                {
                    foreach (Option op in dataList.Options.Where(e => e.Text != null && e.Text.ToUpper().Contains(sItem.ToUpper())))
                    {
                        if (results.FirstOrDefault(e => e == op.Text) == null)
                        {
                            results.Add(op.Text);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //most likely because the search term doesn't exist in the list at all.
                    //skip to the next item.
                }
            }
        }
    }

    public string searchDropDown(WatiN.Core.SelectList dataList)
    {

        if (results.Count > 0)
        {
            //take the first result; set the selectList to that option
            //then remove it from the results collection(so it doesn't turn up again).
            if (results.FirstOrDefault() != null)
            {
                string result = results.FirstOrDefault();

                dataList.Select(result);
                results.Remove(result);
                //instead move used results to a second list so can back up and retry them?

                return result;
            }
            return "";
        }
        else
        {
            return "";
        }
    }

    public bool hasFurtherMatches()
    {
        return (results.Count > 0);
    }
}
}