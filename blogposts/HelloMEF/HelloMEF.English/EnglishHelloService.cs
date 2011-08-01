using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloMEF.App;
using System.ComponentModel.Composition;

namespace HelloMEF.English
{
    [Export(typeof(IHelloService))]
    public class EnglishHelloService : IHelloService
    {
        public string GetHelloMessage()
        {
            return "Hello World!";
        }
    }
}
