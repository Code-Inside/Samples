using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhyInterfaces.Data;

namespace WhyInterfaces.Test.Stubs
{
    public class TestPersonDataProvider : IPersonDataProvider
    {
        public List<Person> InMemoryPersonCollection { get; set; }

        public TestPersonDataProvider()
        {
            InMemoryPersonCollection = new List<Person>();
            for(int i=0; i<10; i++)
            {
                Person p = new Person();
                p.Age = i;
                p.Name = "Database" + i;

                InMemoryPersonCollection.Add(p);
            }
        }

        public List<Person> GetPersons()
        {
            return InMemoryPersonCollection;
        }

        public void AddPerson(Person person)
        {
            InMemoryPersonCollection.Add(person);
        }


    }
}
