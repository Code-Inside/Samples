using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TfsApi.TestRuns
{
    class Program
    {
        static void Main(string[] args)
        {
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri("https://code-inside.visualstudio.com/DefaultCollection"));
            ITestManagementService testManagement = (ITestManagementService)tfs.GetService(typeof(ITestManagementService));

            // WIQL for Test: http://blogs.msdn.com/b/duat_le/archive/2010/02/25/wiql-for-test.aspx?Redirected=true
            string query = "SELECT * FROM TestRun WHERE TestRun.CompleteDate > '" + DateTime.Now.AddDays(-3).Year + "-" + DateTime.Now.AddDays(-3).Month + "-" + DateTime.Now.AddDays(-3).Day + "'";

            var testRuns = testManagement.QueryTestRuns(query);

            foreach (var testPlan in testRuns)
            {
                Console.WriteLine("------------------");
                Console.WriteLine("TestPlan-Title: {0}", testPlan.Title);
                Console.WriteLine("Overall State: {0}", testPlan.State);
                Console.WriteLine("Overall DateCreated: {0}", testPlan.DateCreated);
                Console.WriteLine("Overall PassedTests: {0}", testPlan.PassedTests);


                var testRunResults = testPlan.QueryResults();

                Console.WriteLine("Test Run Results:");
                foreach (var testRunResult in testRunResults)
                {
                    Console.WriteLine("  TestRun: {0}", testRunResult.TestCaseTitle);
                    Console.WriteLine("  State: {0}", testRunResult.State);

                    if (testRunResult.TestCaseId != 0)
                    {
                        var testCaseDetail = testManagement.GetTeamProject("Grocerylist").TestCases.Find(testRunResult.TestCaseId);

                        foreach (var steps in testRunResult.Iterations)
                        {
                            foreach (var action in steps.Actions)
                            {
                                var actionFromTestCase = (ITestStep)testCaseDetail.FindAction(action.ActionId);
                                Console.WriteLine("    " + actionFromTestCase.Title + " : " + action.Outcome);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No TestCase behind this Testrun");
                    }
                }

            }

            Console.ReadLine();
        }
    }
}
