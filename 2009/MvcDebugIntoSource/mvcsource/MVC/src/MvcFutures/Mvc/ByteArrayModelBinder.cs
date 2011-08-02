namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;

    // Register via a call in Global.asax.cs to 
    // ModelBinders.Binders.Add(typeof(byte[]), new ByteArrayModelBinder());
    public class ByteArrayModelBinder : IModelBinder {
        public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            ValueProviderResult valueResult;
            bindingContext.ValueProvider.TryGetValue(bindingContext.ModelName, out valueResult);

            // case 1: there was no <input ... /> element containing this data
            if (valueResult == null) {
                return null;
            }

            string value = valueResult.AttemptedValue;

            // case 2: there was an <input ... /> element but it was left blank
            if (String.IsNullOrEmpty(value)) {
                return null;
            }

            // Future proofing. If the byte array is actually an instance of System.Data.Linq.Binary
            // then we need to remove these quotes put in place by the ToString() method.
            string realValue = value.Replace("\"", String.Empty);
            return Convert.FromBase64String(realValue);
        }
    }

}
