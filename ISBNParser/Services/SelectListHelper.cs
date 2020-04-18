using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;

namespace ISBNParser.Services
{
    class SelectListHelper
    {
        public List<string> names;
        public List<string> values;

        public SelectListHelper(SelectList sel)
        {
            string text = sel.InnerHtml;
            names = new List<string>();
            values = new List<string>();

            //With Windows Server 2012, SelectLists are not loading the Options correctly.

            //const string boundry1 = "<option";
            const string valueBoundry = " value=\"";

            string[] OptionList = text.Split(new string[] { "<option" }, StringSplitOptions.None);
            int pos1;
            int pos2;
            string temp;

            foreach (string curOption in OptionList)
            {
                if (curOption.Trim().Length > 0)
                {
                    if (curOption.Contains(valueBoundry))
                    {
                        pos1 = curOption.IndexOf(valueBoundry) + valueBoundry.Length;
                        pos2 = curOption.IndexOf("\"", pos1);
                        temp = curOption.Substring(pos1, (pos2 - pos1)).Trim();

                        values.Add(temp);
                    }
                    else
                    {
                        values.Add("");
                    }


                    pos1 = curOption.IndexOf(">") + 1;
                    pos2 = curOption.IndexOf("<", pos1);
                    temp = curOption.Substring(pos1, (pos2 - pos1)).Trim();

                    names.Add(temp);
                }
            }

        }
    }
}
