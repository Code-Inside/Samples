using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace CreateOrUpdateGitHubFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var ghClient = new GitHubClient(new ProductHeaderValue("Octokit-Test"));
                ghClient.Credentials = new Credentials("ACCESSTOKEN");

                // github variables
                var owner = "OWNER";
                var repo = "REPO";
                var branch = "BRANCH";

                var targetFile = "_data/test.txt";

                try
                {
                    // try to get the file (and with the file the last commit sha)
                    var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

                    // update the file
                    var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                       new UpdateFileRequest("API File update", "Hello Universe! " + DateTime.UtcNow, existingFile.First().Sha, branch));
                }
                catch (Octokit.NotFoundException)
                {
                    // if file is not found, create it
                    var createChangeSet = await ghClient.Repository.Content.CreateFile(owner,repo, targetFile, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch));
                }

                
                
            }).Wait();
        }
    }
}
