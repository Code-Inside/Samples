using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocks.Data.Interfaces
{
    public interface IPersonRepository
    {
        IQueryable<Person> GetPersons();
    }
}
