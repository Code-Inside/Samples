using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;

namespace ObjectDumperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Person newPerson = new Person();
            newPerson.Age = 123;
            newPerson.Id = Guid.NewGuid();
            newPerson.Name = "hasdhaskjdh";
            ObjectDumper.DumpObject(newPerson, "newPerson", true);

            Employee emp = new Employee();
            emp.Age = 123;
            emp.Id = Guid.NewGuid();
            emp.Name = "hasdhaskjdh";
            emp.HiredOn = DateTime.Now;
            ObjectDumper.DumpObject(emp, "emp", true);

            Console.ReadLine();
        }
    }
}
