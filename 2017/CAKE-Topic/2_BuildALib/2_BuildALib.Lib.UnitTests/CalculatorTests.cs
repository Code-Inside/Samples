using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace _2_BuildALib.Lib.UnitTests
{
    public class CalculatorTests
    {
        [Fact]
        public void PassingTest()
        {
            Calculator sut = new Calculator();
            Assert.Equal(4, sut.Add(2, 2));
        }

        [Fact]
        public void PassingTest2()
        {
            Calculator sut = new Calculator();
            Assert.Equal(40, sut.Add(20, 20));
        }

    }
}
