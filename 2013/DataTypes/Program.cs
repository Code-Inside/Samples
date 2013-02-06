using System.Collections.Generic;

namespace DataTypes
{
    using System;

    internal class Program
    {
        #region Methods

        private static void Main()
        {
            Console.WriteLine(DataSize.GetFrom(0));
            Console.WriteLine(DataSize.GetFrom(1025));
            Console.WriteLine(DataSize.GetFrom(1) * 1024);
            Console.WriteLine(DataSize.GetFrom(1024 * 1024));
            Console.WriteLine(DataSize.GetFrom(1024) + DataSize.GetFrom(1024));
            Console.WriteLine(DataSize.GetFrom(10000000));
            Console.WriteLine(DataSize.GetFrom(10000000).GetString(DataSizeMetricUnit.MegaByte));
            DataSize value = DataSize.GetFrom(950);
            
            DataSize value2 = DataSize.GetFrom(950);

            DataSize valueFromLong = 1000L;
            
            Console.WriteLine(value);
            Console.WriteLine(DataSize.GetFrom(10000000) > value);
            Console.WriteLine(value2 > value);
            Console.WriteLine(value2 == value);



            List<DataSize> list = new List<DataSize>() { 2048L, 5L, 91L, 100L, 700L, 1024L, 128L };
            list.Sort();

            Console.ReadKey();
        }

        #endregion
    }
}