using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhyInterfaces.Data;

namespace WhyInterfaces.Service
{
    public interface IPersonService
    {
        List<Person> GetPersons();
        void AddPerson(Person p);
    }
}
