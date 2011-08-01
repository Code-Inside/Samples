using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RhinoMocks.Data;
using RhinoMocks.Data.Interfaces;
using RhinoMocks.Data.Filters;

namespace RhinoMocks.Services
{
    public class PersonService
    {
        private IPersonRepository PersonRep { get; set; }

        public PersonService(IPersonRepository rep)
        {
            this.PersonRep = rep;
        }

        public IList<Person> GetPersonsByAge(int age)
        {
            return this.PersonRep.GetPersons().WithAge(age).ToList();
        }
    }
}
