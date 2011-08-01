using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.UI.MobileControls;

namespace MvcBinding.Models
{
    public class PersonModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
         if (controllerContext == null) {  
             throw new ArgumentNullException("controllerContext");  
         }  
         if (bindingContext == null) {  
             throw new ArgumentNullException("bindingContext");  
         }

            NameValueCollection collection = controllerContext.RequestContext.HttpContext.Request.Form;

            Person returnValue = new Person();
            returnValue.Id = Guid.NewGuid();
            returnValue.Prename = "Modelbinder: " + collection["Prename"];
            returnValue.Surname = "Modelbinder: " + collection["Surname"];
            int age = 0;
            int.TryParse(collection["Age"], out age);
            returnValue.Age = age;

            return returnValue;
        }

    }
}
