using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcDropDownListHelper.Models
{
    public class FooViewModel
    {
        public string TimeZone { get; set; }

        public IEnumerable<SelectListItem> TimeZones
        {
            get
            {
                return TimeZoneInfo
                    .GetSystemTimeZones()
                    .Select(t => new SelectListItem
                    {
                        Text = t.DisplayName,
                        Value = t.Id
                    });
            }
        }
    }
}