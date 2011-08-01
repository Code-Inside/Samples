using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhyInterfaces.Service;

namespace WhyInterfaces.Test.Stubs
{
    class FalseTestAuthenticationService : IAuthenticationService
    {
        public bool IsAuthenticated()
        {
            return false;
        }
    }
}
