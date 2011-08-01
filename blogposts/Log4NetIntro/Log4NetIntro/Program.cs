using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Log4NetIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof (Program));

            logger.Debug("Hello World!");
            logger.Error("D´oh!");
            Console.ReadLine();
        }
    }
}
