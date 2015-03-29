using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonConvertIssuesWithBaseClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            AFoo a = new AFoo();
            a.FooBarBuzz = "A";
            a.A = "Hello World";

            BFoo b = new BFoo();
            b.FooBarBuzz = "B";
            b.B = "Hello World";

            List<BaseFoo> allFoos = new List<BaseFoo>();
            allFoos.Add(a);
            allFoos.Add(b);

            var result = JsonConvert.SerializeObject(allFoos);

            // issue here:
            //Additional information: Could not create an instance of type ConsoleApplication6.BaseFoo. Type is an interface or abstract class and cannot be instantiated. Path '[0].A', line 1, position 6.
            //var test = JsonConvert.DeserializeObject<List<BaseFoo>>(result);
            JsonConverter[] converters = { new FooConverter()};

            var test = JsonConvert.DeserializeObject<List<BaseFoo>>(result, new JsonSerializerSettings() { Converters = converters });

        }
    }

    public class FooConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BaseFoo));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["FooBarBuzz"].Value<string>() == "A")
                return jo.ToObject<AFoo>(serializer);

            if (jo["FooBarBuzz"].Value<string>() == "B")
                return jo.ToObject<BFoo>(serializer);

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseFoo
    {
        public string FooBarBuzz { get; set; }
    }

    public class AFoo : BaseFoo
    {
        public string A { get; set; }
    }

    public class BFoo : BaseFoo
    {
        public string B { get; set; }
    }
}
