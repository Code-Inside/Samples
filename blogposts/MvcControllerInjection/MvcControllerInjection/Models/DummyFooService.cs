using System;

namespace MvcControllerInjection.Models
{
    public class DummyFooService : IFooService
    {
        public string Bar()
        {
            return "DummyFooBar";
        }
    }
}