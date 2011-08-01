using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebHistory.Model;

namespace WebHistory.Service
{
    public interface IArchiveService
    {
        Website Load(string url);
    }
}
