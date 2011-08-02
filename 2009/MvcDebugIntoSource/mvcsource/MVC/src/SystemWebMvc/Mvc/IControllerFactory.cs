namespace System.Web.Mvc {
    using System.Web.Routing;

    public interface IControllerFactory {
        IController CreateController(RequestContext requestContext, string controllerName);
        void ReleaseController(IController controller);
    }
}
