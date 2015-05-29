using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace CreateAndExtractNuGetPackages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create NuGet Package via Code");
            ManifestMetadata metadata = new ManifestMetadata()
            {
                Authors = "Authors Name",
                Version = "1.0.0.0",
                Id = "NuGetId",
                Description = "NuGet Package Description goes here!",
            };

            PackageBuilder builder = new PackageBuilder();


            var path = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\DemoContent\\";

            builder.PopulateFiles(path, new[] { new ManifestFile { Source = "**", Target = "content" } });
            builder.Populate(metadata);

            using (FileStream stream = File.Open("test.nupkg", FileMode.OpenOrCreate))
            {
                builder.Save(stream);
            }

            Console.WriteLine("... and extract NuGet Package via Code");

            NuGet.ZipPackage package = new ZipPackage("test.nupkg");
            var content = package.GetContentFiles();

            Console.WriteLine("Package Id: " + package.Id);
            Console.WriteLine("Content-Files-Count: " + content.Count());

            Console.ReadLine();
        }
    }
}
