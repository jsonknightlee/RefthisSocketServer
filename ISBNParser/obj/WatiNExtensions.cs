using System;
using WatiN.Core;
using WatiN.Core.Native;

namespace WatiN.Core
{
    public static class WatiNExtensions
    {
        public static void ForceChange(this Element e)
        {
            //e.DomContainer.Eval("$('#" + e.Id + "').change();");
            try
            {
                e.DomContainer.Eval(string.Format("$({0}).change();", e.GetJavascriptElementReference()));
            }
            catch { }
        }

        public static bool isBusy(this IE ie)
        {
            if (ie.Visible && ie.ContainsText("Home"))
                return false;
            else
                return true;
        }

        public static void safeWaitUntilComplete(IE ie, int msTimeout)
        {
            int msSlept = 0;
            while (WatiN.Core.WatiNExtensions.isBusy(ie) && msSlept < msTimeout)
            {
                System.Threading.Thread.Sleep(100);
                msSlept += 100;
            }
            if (msSlept >= msTimeout)
            {
                throw new System.ApplicationException("method safeWaitUntilComplete timed out");
            }
        }

        [ElementTag("input", InputType = "text", Index = 0)]
        [ElementTag("input", InputType = "password", Index = 1)]
        [ElementTag("input", InputType = "textarea", Index = 2)]
        [ElementTag("input", InputType = "hidden", Index = 3)]
        [ElementTag("textarea", Index = 4)]
        [ElementTag("input", InputType = "email", Index = 5)]
        [ElementTag("input", InputType = "url", Index = 6)]
        [ElementTag("input", InputType = "number", Index = 7)]
        [ElementTag("input", InputType = "range", Index = 8)]
        [ElementTag("input", InputType = "search", Index = 9)]
        [ElementTag("input", InputType = "color", Index = 10)]
        [ElementTag("input", InputType = "date", Index = 11)]
        public class TextFieldExtended : TextField
        {
            public TextFieldExtended(DomContainer domContainer, INativeElement element)
                : base(domContainer, element)
            {
            }

            public TextFieldExtended(DomContainer domContainer, ElementFinder finder)
                : base(domContainer, finder)
            {
            }

            public static void Register()
            {
                Type typeToRegister = typeof(TextFieldExtended);
                ElementFactory.RegisterElementType(typeToRegister);
            }
        }
    }

}
