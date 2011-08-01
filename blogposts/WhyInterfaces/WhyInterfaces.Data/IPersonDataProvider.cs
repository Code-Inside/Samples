using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhyInterfaces.Data
{
    public interface IPersonDataProvider
    {
        List<Person> GetPersons();
        void AddPerson(Person person);
    }
}
