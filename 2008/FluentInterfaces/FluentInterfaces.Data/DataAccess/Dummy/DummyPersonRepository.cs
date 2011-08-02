using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentInterfaces.Data.DataAccess.Dummy
{
    public class DummyPersonRepository 
    {
        public IQueryable<Person> GetPersons()
        {
            List<Person> returnValues = new List<Person>()
                {
                    new Person() { Age = 11, Name = "Bob" },
                    new Person() { Age = 22, Name = "Alice" },
                    new Person() { Age = 20, Name = "Robert" },
                    new Person() { Age = 40, Name = "Hans" },
                    new Person() { Age = 20, Name = "Peter" },
                    new Person() { Age = 20, Name = "Oli" },
                };

            return returnValues.AsQueryable();
        }
    }
}
