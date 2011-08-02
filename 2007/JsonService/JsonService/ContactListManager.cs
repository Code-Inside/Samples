using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace JsonService
{
    public class ContactListManager
    {
        public List<Contact> GetAllContacts()
        {
            List<Contact> returnList = new List<Contact>();

            for (int i = 0; i < 500; i++)
            {
                Contact contactPerson = new Contact();
                contactPerson.Name = "Mr. Nice Guy " + i;
                contactPerson.Age = i;
                contactPerson.Address = new Address();
                contactPerson.Address.Street = "Lieblingsstraße " + i;
                contactPerson.Address.City = "Dolle Stadt " + i;
                returnList.Add(contactPerson);
            }

            return returnList;
        }
    }
}
