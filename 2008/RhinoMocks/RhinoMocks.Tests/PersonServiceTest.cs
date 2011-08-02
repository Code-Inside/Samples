using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using RhinoMocks.Data.Interfaces;
using RhinoMocks.Data;
using RhinoMocks.Services;

namespace RhinoMocks.Tests
{
    /// <summary>
    /// Summary description for PersonServiceTest
    /// </summary>
    [TestClass]
    public class PersonServiceTest
    {
        public PersonServiceTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void PersonService_GetPersonByAge_Works()
        {
            MockRepository mock = new MockRepository();
            IPersonRepository rep = mock.StrictMock<IPersonRepository>();

            using (mock.Record())
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
                Expect.Call(rep.GetPersons()).Return(returnValues.AsQueryable());
            }
            using (mock.Playback())
            {
                PersonService service = new PersonService(rep);
                List<Person> serviceResults = service.GetPersonsByAge(20).ToList();

                Assert.AreNotEqual(0, serviceResults.Count);

                foreach (Person result in serviceResults)
                {
                    Assert.AreEqual(20, result.Age);
                }
            }
        }
    }
}
