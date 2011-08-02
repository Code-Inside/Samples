using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhyInterfaces.Data;

namespace WhyInterfaces.Service
{
    public class PersonService : IPersonService
    {
        private IAuthenticationService authentication;
        private IPersonDataProvider personProvider;

        public PersonService(IAuthenticationService authSrv, IPersonDataProvider provider)
        {
            authentication = authSrv;
            personProvider = provider;
        }

        public List<Person> GetPersons()
        {
            if (authentication.IsAuthenticated())
            {
                return personProvider.GetPersons();
            }
            
            return null;
        }

        public void AddPerson(Person p)
        {
            if (authentication.IsAuthenticated())
            {
                personProvider.AddPerson(p);
            }
        }

    }
}
