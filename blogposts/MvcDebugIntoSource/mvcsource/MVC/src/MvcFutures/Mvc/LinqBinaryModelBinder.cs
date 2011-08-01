namespace Microsoft.Web.Mvc {
    using System.Data.Linq;
    using System.Web.Mvc;
    
    // Register via a call in Global.asax.cs to 
    // ModelBinders.Binders.Add(typeof(Binary), new LinqBinaryModelBinder());
    public class LinqBinaryModelBinder : ByteArrayModelBinder {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            byte[] byteValue = (byte[])base.BindModel(controllerContext, bindingContext);
            if (byteValue == null) {
                return null;
            }

            return new Binary(byteValue);
        }
    }
}
