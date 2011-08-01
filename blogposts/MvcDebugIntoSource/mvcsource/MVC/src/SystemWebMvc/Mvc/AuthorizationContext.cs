namespace System.Web.Mvc {

    public class AuthorizationContext : ControllerContext {

        // parameterless constructor used for mocking
        public AuthorizationContext() {
        }

        public AuthorizationContext(ControllerContext controllerContext)
            : base(controllerContext) {
        }

        public ActionResult Result {
            get;
            set;
        }

    }
}
