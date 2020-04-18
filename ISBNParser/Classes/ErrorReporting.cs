using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNParser.Classes
{
    public class ErrorReporting
    {
        public int WebsiteID { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string TargetFile { get; set; }
        public string TargetMethod { get; set; }
        public int TargetLine { get; set; }

        public string InnerExceptionType { get; set; }
        public string InnerMessage { get; set; }

        public int Priority { get; set; }

        public ErrorReporting()
        {

        }

        public void LogThis()
        {
            //[usp_LogAutoStatusError]
        }
    }
}
