using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace XmlAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Einfaches Beispiel:");
            // Simple Write
            Console.WriteLine("Schreiben...");
            XmlSerializer _xmlGenSimpleWrite = new XmlSerializer(typeof(string));
            _xmlGenSimpleWrite.Serialize(Console.Out, "www.code-inside.de");
            Console.WriteLine("");
            // Simple Read
            Console.WriteLine("Lesen...");
            XmlSerializer _xmlGenSimpleRead = new XmlSerializer(typeof(string));
            Console.WriteLine(_xmlGenSimpleRead.Deserialize(new StringReader("<string>www.Code-Inside.de</string>")));
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Komplexes Beispiel:");
            // Complex Write
            Console.WriteLine("Datei schreiben...");

            string _path = Path.Combine(Application.StartupPath, "test.xml");

            using (StreamWriter _xmlFile = new StreamWriter(_path))
            {
                Console.WriteLine("Blogeintrag 'XML erstellen mit XmlAttributen' erstellen.");
                XmlSerializer _xmlGen = new XmlSerializer(typeof(BlogEntry));
                BlogEntry _myBlogEntry = new BlogEntry();
                _myBlogEntry.Title = "XML erstellen mit XmlAttributen";
                _myBlogEntry.Content = "Es gibt viele Möglichkeiten...";

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("Kommentar anfügen: " + i);
                    _myBlogEntry.Comments.Add(new BlogComment("Paul", "Cooole Sache" + i));
                    _myBlogEntry.Comments[i].Comments.Add(new BlogComment("Tim", "Finde ich auch Paul"));
                }

                _xmlGen.Serialize(_xmlFile, _myBlogEntry);
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Datei laden...");
            // Complex Read
            using (StreamReader _xmlFile = new StreamReader(_path))
            {
                XmlSerializer _xmlGen = new XmlSerializer(typeof(BlogEntry));
                BlogEntry _myBlogEntry =(BlogEntry)_xmlGen.Deserialize(_xmlFile);
                Console.WriteLine("Blogeintrag '" + _myBlogEntry.Title + "' geladen:");
                foreach (BlogComment _comments in _myBlogEntry.Comments)
                {
                    Console.WriteLine("Kommentar: '" + _comments.Name + ": " + _comments.Content + "'");
                }
                
            }

            Console.ReadLine();
        }
    }
    
}
