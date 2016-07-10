using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LibWithoutWpfStuff
{
    public static class UsingJsonNet
    {
        public static string ToJson(List<string> toJson)
        {
            return JsonConvert.SerializeObject(toJson);
        }
    }
}
