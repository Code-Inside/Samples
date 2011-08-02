using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCascadingDropdownJson.Models
{
    public class LocationRepository
    {
        public static IList<Country> GetCountries()
        {
            List<Country> countries = new List<Country>();

            for (int i = 0; i < 5; i++)
            {
                Country country = new Country();
                country.Name = "Country " + i.ToString();
                countries.Add(country);
            }

            return countries;
        }

        public static IList<FederalStates> GetFederalStates(string countryName)
        {
            List<FederalStates> states = new List<FederalStates>();

            for (int i = 0; i < 5; i++)
            {
                FederalStates state = new FederalStates();
                state.Name = countryName + " - FederalState " + i.ToString();
                states.Add(state);
            }

            return states;
        }

        public static IList<City> GetCities(string federalStateName)
        {
            List<City> cities = new List<City>();

            for (int i = 0; i < 5; i++)
            {
                City city = new City();
                city.Name = federalStateName + " - City " + i.ToString();
                cities.Add(city);
            }

            return cities;
        }

        public static IList<string> GetStreets(string cityName)
        {
            List<string> streets = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                string street = cityName + " - Street " + i.ToString();
                streets.Add(street);
            }

            return streets;
        }
    }
}
