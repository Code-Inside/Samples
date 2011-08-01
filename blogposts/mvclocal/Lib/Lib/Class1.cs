using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib
{
    public class Testclass
    {
        public string Test()
        {
            return System.Configuration.ConfigurationManager.AppSettings["Test"];
        }
    }
}
