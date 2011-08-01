using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentInterfaces.Data;
using FluentInterfaces.Data.Filters;
using FluentInterfaces.Data.DataAccess.Dummy;
namespace FluentInterfaces.Services
{
    public class PersonService
    {
        private DummyPersonRepository PersonRep { get; set; }

        public PersonService()
        {
            this.PersonRep = new DummyPersonRepository();
        }

        public IList<Person> GetPersons(int age, string startsWith)
        {
            return this.PersonRep.GetPersons().WithAge(age).NameStartsWith(startsWith).ToList();
        }
    }
}
