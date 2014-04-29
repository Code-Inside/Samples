using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterRetweets
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://urls.api.twitter.com/1/urls/count.json?url=http://blog.codeinside.eu/2014/04/26/fix-excel-com-exception-code-2147467259-exception-from-hresult-0x80028018/#comments";

            // Be carefull with this code - use async/await - 
            // pure demo code inside a console application

            var client = new HttpClient();
            var result = client.GetAsync(url).Result;

            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.ReadLine();
        }
    }
}
