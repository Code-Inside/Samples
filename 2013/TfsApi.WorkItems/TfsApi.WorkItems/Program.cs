using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsApi.WorkItems
{
    class Program
    {
        static void Main(string[] args)
        {

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri("https://code-inside.visualstudio.com/DefaultCollection"));


            // http://stackoverflow.com/questions/10748412/retrieving-work-items-and-their-linked-work-items-in-a-single-query-using-the-tf/10757338#10757338
            var workItemStore = tfs.GetService<WorkItemStore>();

            var project = workItemStore.Projects["Grocerylist"];
            QueryHierarchy queryHierarchy = project.QueryHierarchy;
            var queryFolder = queryHierarchy as QueryFolder;
            QueryItem queryItem = queryFolder["My Queries"];
            queryFolder = queryItem as QueryFolder;

            if (queryFolder != null)
            {
                Dictionary<string, string> variables = new Dictionary<string, string>();
                variables.Add("project", "Grocerylist");

                var myQuery = queryFolder["New Query 1"] as QueryDefinition;
                if (myQuery != null)
                {
                    var wiCollection = workItemStore.Query(myQuery.QueryText, variables);
                    foreach (WorkItem workItem in wiCollection)
                    {
                        Console.WriteLine("WorkItem -----------------------------");
                        Console.WriteLine(workItem.Title);

                        foreach (Revision rev in workItem.Revisions)
                        {
                            Console.WriteLine("  - Revition: " + rev.Index);

                            foreach (Field f in rev.Fields)
                            {
                                if (!Object.Equals(f.Value, f.OriginalValue))
                                {
                                    Console.WriteLine("  - Changes: {0}: {1} -> {2}", f.Name, f.OriginalValue, f.Value);
                                }
                            }
                            Console.WriteLine("  ------------------");

                        }

                    }

                    Console.ReadLine();
                }
            }

        }
    }
}
