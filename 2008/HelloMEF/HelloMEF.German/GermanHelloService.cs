using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloMEF.App;
using System.ComponentModel.Composition;

namespace HelloMEF.German
{
    public class GermanHelloService
    {
        [Export(typeof(IHelloService))]
        public class EnglishHelloService : IHelloService
        {
            public string GetHelloMessage()
            {
                return "Hallo Welt!";
            }
        }
    }
}
