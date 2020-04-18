using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNParser.Classes
{
    public class ReferenceItem
    {
       public int RefID { get; set; }
      public string Title { get; set; }
      public string Author { get; set; }
      public string ISBN { get; set; }
      public string Url { get; set; }
      public string YearPublished { get; set; }
      public string Publisher { get; set; }
      public string StudentID { get; set; }
      public int UserID { get; set; }
      public DateTime InsertDate { get; set; }
    }
}
