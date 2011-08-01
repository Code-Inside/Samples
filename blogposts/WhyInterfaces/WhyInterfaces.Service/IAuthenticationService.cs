using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhyInterfaces.Service
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated();
    }
}
