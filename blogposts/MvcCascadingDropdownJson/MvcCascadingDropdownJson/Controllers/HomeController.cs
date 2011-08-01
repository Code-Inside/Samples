using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCascadingDropdownJson.Models;

namespace MvcCascadingDropdownJson.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private HomeViewModel GetHomeViewModel(string country, string federalState, string city, string street)
        {
            HomeViewModel model = new HomeViewModel();

            IList<Country> countries = LocationRepository.GetCountries();
            foreach (Country countryItem in countries)
            {
                SelectListItem item = new SelectListItem();
                item.Text = countryItem.Name;
                item.Value = countryItem.Name;
                model.Countries.Add(item);
            }

            if(string.Empty != country)
            {
                IList<FederalStates> fedStates = LocationRepository.GetFederalStates(country);
                foreach (FederalStates fedItem in fedStates)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = fedItem.Name;
                    item.Value = fedItem.Name;
                    model.FederalStates.Add(item);
                }
            }

            if(string.Empty != federalState)
            {
                IList<City> cities = LocationRepository.GetCities(federalState);
                foreach (City cityItem in cities)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = cityItem.Name;
                    item.Value = cityItem.Name;
                    model.Cities.Add(item);
                }
            }

            if(string.Empty != city)
            {
                IList<string> streets = LocationRepository.GetStreets(city);
                foreach (string streetItem in streets)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = streetItem;
                    item.Value = streetItem;
                    model.Streets.Add(item);
                }
            }

            return model;
        }

        public ActionResult Index()
        {
            HomeViewModel model = this.GetHomeViewModel(string.Empty, string.Empty,string.Empty,string.Empty);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string country, string federalState, string city, string street)
        {
            ViewData.ModelState.AddModelError("Message", "Uppps...!");
            HomeViewModel model = this.GetHomeViewModel(country, federalState, city, street);
            return View(model);
        }

        public JsonResult GetFederalStatesJson(string country)
        {
            IList<FederalStates> fedStates = LocationRepository.GetFederalStates(country);
            return Json(fedStates);
        }

        public JsonResult GetCitiesJson(string federalState)
        {
            IList<City> cities = LocationRepository.GetCities(federalState);
            return Json(cities);
        }

        public JsonResult GetStreetsJson(string cityName)
        {
            IList<string> streets = LocationRepository.GetStreets(cityName);
            return Json(streets);
        }
    }
}
