using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCascadingDropdownJson.Models
{
    public class HomeViewModel
    {
        public IList<SelectListItem> Countries { get; set; }
        public IList<SelectListItem> FederalStates { get; set; }
        public IList<SelectListItem> Cities { get; set; }
        public IList<SelectListItem> Streets { get; set; }

        public HomeViewModel()
        {
            this.Countries = new List<SelectListItem>();
            this.Countries.Add(new SelectListItem() { Text = "Please choose...", Value = ""});
            this.FederalStates = new List<SelectListItem>();
            this.FederalStates.Add(new SelectListItem() { Text = "Please choose...", Value = "" });
            this.Cities = new List<SelectListItem>();
            this.Cities.Add(new SelectListItem() { Text = "Please choose...", Value = "" });
            this.Streets = new List<SelectListItem>();
            this.Streets.Add(new SelectListItem() { Text = "Please choose...", Value = "" });
        }
    }
}
