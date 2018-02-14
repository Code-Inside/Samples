using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace JintPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            InvokeFunctionFromDotNet();
            Console.ReadLine();
        }

        public static void SimpleStart()
        {
            var engine = new Jint.Engine();
            Console.WriteLine(engine.Execute("1 + 2 + 3 + 4").GetCompletionValue());
        }

        public static void Repl()
        {
            var engine = new Jint.Engine();

            while (true)
            {
                Console.Write("> ");
                var statement = Console.ReadLine();
                var result = engine.Execute(statement).GetCompletionValue();
                Console.WriteLine(result);
            }
        }

        public static void Handlebars()
        {
            var engine = new Jint.Engine();

            engine.Execute(File.ReadAllText("handlebars-v4.0.11.js"));

            engine.SetValue("context", new
            {
                cats = new[]
                {
                    new {name = "Feivel"},
                    new {name = "Lilly"}
                }
            });

            engine.SetValue("source", "{{#each cats}} {{name}} says meow!!!\n{{/each}}");

            engine.Execute("var template = Handlebars.compile(source);");

            var result = engine.Execute("template(context)").GetCompletionValue();

            Console.WriteLine(result);
        }

        public static void DefinedDotNetApi()
        {
            var engine = new Jint.Engine();

            engine.SetValue("demoJSApi", new DemoJavascriptApi());

            var result = engine.Execute("demoJSApi.helloWorldFromDotNet('TestTest')").GetCompletionValue();

            Console.WriteLine(result);
        }

        public static void AllowCrl()
        {
            var engine = new Engine(cfg => cfg.AllowClr());

            var result = engine.Execute("System.DateTime.Now").GetCompletionValue();

            Console.WriteLine(result);
        }

        public static void InvokeFunctionFromDotNet()
        {
            var engine = new Engine();

            var fromValue = engine.Execute("function jsAdd(a, b) { return a + b; }").GetValue("jsAdd");

            Console.WriteLine(fromValue.Invoke(5, 5));

            Console.WriteLine(engine.Invoke("jsAdd", 3, 3));
        }
    }

    public class DemoJavascriptApi
    {
        public string helloWorldFromDotNet(string name)

        {
            return $"Hello {name} - this is executed in {typeof(Program).FullName}";
        }
    }
}
