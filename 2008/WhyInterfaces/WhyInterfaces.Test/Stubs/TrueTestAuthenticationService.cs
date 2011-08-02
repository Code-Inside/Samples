using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhyInterfaces.Service;

namespace WhyInterfaces.Test.Stubs
{
    public class TrueTestAuthenticationService : IAuthenticationService
    {
        public bool IsAuthenticated()
        {
            return true;
        }
    }
}
