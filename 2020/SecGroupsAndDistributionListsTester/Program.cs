using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;

namespace SecGroupsAndDistributionListsTester
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter User:");
            var name = Console.ReadLine();

            var domain = Domain.GetComputerDomain();
            try
            {
                Console.WriteLine("ListAllGroupsViaLdapQuery:");

                Console.WriteLine($"Try to get all groups for {name} in {domain.Name}.");

                // Be aware that some "system level" groups are not part of the returned list
                // Use a comibination of this + tokenGroups
                ListAllGroupsViaLdapQuery(name, domain.Name);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.Read();
        }


        private static List<string> ListAllGroupsViaLdapQuery(string username, string domainName)
        {
            List<string> result = new List<string>();

            using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domainName))
            using (var searcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + domainContext.Name)))
            {
                searcher.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", username);
                SearchResult sr = searcher.FindOne();

                DirectoryEntry user = sr.GetDirectoryEntry();

                var dn = user.Properties["distinguishedname"];
                var x = GetGroupsForDistinguishedName(new DirectoryEntry("LDAP://" + domainContext.Name), dn.Value.ToString());

                foreach (var groupX in x)
                {
                    Console.WriteLine(groupX.ToString());
                }

            }

            return result;
        }

        private static DirectorySearcher CreateDirectorySearcher(string filter)
        {
            Domain domain = Domain.GetComputerDomain();

            DirectorySearcher searcher;

            string targetSearchRoot = "LDAP://" + domain.Name;


            searcher = new DirectorySearcher(new DirectoryEntry(targetSearchRoot));
            searcher.Filter = filter;

            searcher.SearchScope = SearchScope.Subtree;

            return searcher;
        }

        public class GroupResult
        {
            public string Name { get; set; }
            public string ObjectSid { get; set; }
            public int GroupType { get; set; }

            public override string ToString()
            {
                return $"{Name} ({ObjectSid}) - Type: {GroupType}";
            }

        }

        private static List<GroupResult> GetGroupsForDistinguishedName(DirectoryEntry domainDirectoryEntry, string distinguishedName)
        {
            var groups = new List<GroupResult>();
            if (!string.IsNullOrEmpty(distinguishedName))
            {
                var getGroupsFilterForDn = $"(&(objectClass=group)(member:1.2.840.113556.1.4.1941:= {distinguishedName}))";
                using (var dirSearch = CreateDirectorySearcher(getGroupsFilterForDn))
                {
                    using (var results = dirSearch.FindAll())
                    {
                        foreach (SearchResult result in results)
                        {
                            if (result.Properties.Contains("name") && result.Properties.Contains("objectSid") && result.Properties.Contains("groupType"))
                                groups.Add(new GroupResult() { Name = (string)result.Properties["name"][0], GroupType = (int)result.Properties["groupType"][0], ObjectSid = new SecurityIdentifier((byte[])result.Properties["objectSid"][0], 0).ToString() });
                        }
                    }
                }
            }

            return groups;
        }


    }
}
