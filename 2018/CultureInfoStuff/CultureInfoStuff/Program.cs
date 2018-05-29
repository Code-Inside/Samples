using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CultureInfoStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            var culture = new CultureInfo("de-CH");

            Console.WriteLine("de-CH Group Separator");
            Console.WriteLine(
                $"{culture.NumberFormat.CurrencyGroupSeparator} - CharCode: {(int) char.Parse(culture.NumberFormat.CurrencyGroupSeparator)}");
            Console.WriteLine(
                $"{culture.NumberFormat.NumberGroupSeparator} - CharCode: {(int) char.Parse(culture.NumberFormat.NumberGroupSeparator)}");

            var cultureFromFramework = CultureInfo.GetCultureInfo("de-CH");

            Console.WriteLine("de-CH Group Separator from Framework");
            Console.WriteLine(
                $"{cultureFromFramework.NumberFormat.CurrencyGroupSeparator} - CharCode: {(int)char.Parse(cultureFromFramework.NumberFormat.CurrencyGroupSeparator)}");
            Console.WriteLine(
                $"{cultureFromFramework.NumberFormat.NumberGroupSeparator} - CharCode: {(int)char.Parse(cultureFromFramework.NumberFormat.NumberGroupSeparator)}");
        }
    }
}
