using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsApi.ChangeSets
{
    class Program
    {
        static void Main(string[] args)
        {
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri("https://code-inside.visualstudio.com/DefaultCollection"));

            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));

            //Following will get all changesets since 365 days. Note : "DateVersionSpec(DateTime.Now - TimeSpan.FromDays(20))"
            System.Collections.IEnumerable history = vcs.QueryHistory("$/Grocerylist", 
                                                                      LatestVersionSpec.Instance,
                                                                      0,
                                                                      RecursionType.Full,
                                                                      null,
                                                                      new DateVersionSpec(DateTime.Now - TimeSpan.FromDays(365)),
                                                                      LatestVersionSpec.Instance,
                                                                      Int32.MaxValue,
                                                                      true,
                                                                      false);

            foreach (Changeset changeset in history)
            {
                Console.WriteLine("Changeset Id: " + changeset.ChangesetId);
                Console.WriteLine("Owner: " + changeset.Owner);
                Console.WriteLine("Date: " + changeset.CreationDate.ToString());
                Console.WriteLine("Comment: " + changeset.Comment);
                Console.WriteLine("-------------------------------------");
            }

            Console.ReadLine();
        }
    }
}
