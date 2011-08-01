using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoNot.Fear.UnitTests.Data
{
    public class DataManager
    {
        public bool ConnectToData()
        {
            return true;
        }

        public List<int> GetData()
        {
            return new List<int>() {1, 2, 3, 4, 5, 6, 7 };
        }
    }
}
