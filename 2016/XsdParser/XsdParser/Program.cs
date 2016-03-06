using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XsdParser
{
    /// <summary>
    /// Code based on https://msdn.microsoft.com/en-us/library/ms255932(v=vs.110).aspx
    /// </summary>
    class Program
    {
        public static void AnalyseSchema(XmlSchemaSet set)
        {
            // Retrieve the compiled XmlSchema object from the XmlSchemaSet
            // by iterating over the Schemas property.
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                customerSchema = schema;
            }

            // Iterate over each XmlSchemaElement in the Values collection
            // of the Elements property.
            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                RecursiveElementAnalyser(" ", element);
            }

        }

        public static void RecursiveElementAnalyser(string prefix, XmlSchemaElement element)
        {
            string elementName = prefix + element.Name;

            string dataType = element.ElementSchemaType.TypeCode.ToString();

            Console.WriteLine(elementName + " (" + dataType + ")");

            // Get the complex type of the Customer element.
            XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;

            if (complexType != null)
            {
                // If the complex type has any attributes, get an enumerator 
                // and write each attribute name to the console.
                if (complexType.AttributeUses.Count > 0)
                {
                    IDictionaryEnumerator enumerator =
                        complexType.AttributeUses.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        XmlSchemaAttribute attribute =
                            (XmlSchemaAttribute)enumerator.Value;

                        string attrDataType = attribute.AttributeSchemaType.TypeCode.ToString();

                        string attrName = string.Format(prefix + "(Attr:: {0}({1}))", attribute.Name, attrDataType);

                        Console.WriteLine(attrName);
                    }
                }

                // Get the sequence particle of the complex type.
                XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

                // Iterate over each XmlSchemaElement in the Items collection.
                foreach (XmlSchemaElement childElement in sequence.Items)
                {
                    RecursiveElementAnalyser(prefix + " ", childElement);
                }
            }
        }

        static void Main(string[] args)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(File.ReadAllText("Schema.xsd"))));
            schemas.Compile();
            AnalyseSchema(schemas);
            Console.ReadLine();
        }
    }
}
