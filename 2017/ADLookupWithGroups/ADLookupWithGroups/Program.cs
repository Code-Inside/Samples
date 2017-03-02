using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ADLookupWithGroups
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ListAllGroupsViaTokenGroups:");

            List<string> result = new List<string>();

            try
            {
                result = ListAllGroupsViaTokenGroups("USERNAME", "DOMAIN");

                foreach (var group in result)
                {
                    Console.WriteLine(group);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.Read();
        }

  
        private static List<string> ListAllGroupsViaTokenGroups(string username, string domainName)
        {
            List<string> result = new List<string>();

            using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domainName))
            using (var searcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + domainContext.Name)))
            {
                searcher.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", username);
                SearchResult sr = searcher.FindOne();

                DirectoryEntry user = sr.GetDirectoryEntry();

                // access to other user properties, via user.Properties["..."]

                user.RefreshCache(new string[] { "tokenGroups" });

                for (int i = 0; i < user.Properties["tokenGroups"].Count; i++)
                {
                    SecurityIdentifier sid = new SecurityIdentifier((byte[])user.Properties["tokenGroups"][i], 0);
                    NTAccount nt = (NTAccount)sid.Translate(typeof(NTAccount));

                    result.Add(nt.Translate(typeof(NTAccount)).ToString() + " (" + sid + ")");
                }
            }

            return result;
        }

    }
}
