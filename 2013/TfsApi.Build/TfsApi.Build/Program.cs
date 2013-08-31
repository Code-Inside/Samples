using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;

namespace TfsApi.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            // Auth with UserName & Password (Microsoft Acc):
            //BasicAuthCredential basicCred = new BasicAuthCredential(new NetworkCredential("xxx@hotmail.com", "pw"));
            //TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred);
            //tfsCred.AllowInteractive = false;
            //
            //TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri("https://code-inside.visualstudio.com/DefaultCollection"), tfsCred);

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri("https://code-inside.visualstudio.com/DefaultCollection"));

            IBuildServer buildServer = (IBuildServer)tfs.GetService(typeof(IBuildServer));

            var builds = buildServer.QueryBuilds("DrinkHub");

            foreach (IBuildDetail build in builds)
            {
                var result = string.Format("Build {0}/{3} {4} - current status {1} - as of {2}",
                    build.BuildDefinition.Name,
                    build.Status.ToString(),
                    build.FinishTime,
                    build.LabelName,
                    Environment.NewLine);

                System.Console.WriteLine(result);
            }

            // Detailed via http://www.incyclesoftware.com/2012/09/fastest-way-to-get-list-of-builds-using-ibuildserver-querybuilds-2/

            var buildSpec = buildServer.CreateBuildDetailSpec("DrinkHub", "Main.Continuous");
            buildSpec.InformationTypes = null;
            var buildDetails = buildServer.QueryBuilds(buildSpec).Builds;

            Console.WriteLine(buildDetails.First().Status);

            Console.ReadLine();
        }
    }
}
