using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lib
{
    public static class ReadFromConfig
    {
        public static string Get()
        {
            return ConfigurationManager.AppSettings["ConfigKey"];
        }
    }
}
