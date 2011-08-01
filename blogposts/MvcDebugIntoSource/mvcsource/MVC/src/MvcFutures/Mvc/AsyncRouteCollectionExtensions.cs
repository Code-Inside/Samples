namespace Microsoft.Web.Mvc {
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AsyncRouteCollectionExtensions {

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url) {
            return MapAsyncRoute(routes, name, url, null /* defaults */, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url, object defaults) {
            return MapAsyncRoute(routes, name, url, defaults, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url, object defaults, object constraints) {
            return MapAsyncRoute(routes, name, url, defaults, constraints, null /* namespaces */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url, string[] namespaces) {
            return MapAsyncRoute(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces) {
            return MapAsyncRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapAsyncRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces) {
            Route route = RouteCollectionExtensions.MapRoute(routes, name, url, defaults, constraints, namespaces);
            route.RouteHandler = new MvcAsyncRouteHandler();
            return route;
        }

    }
}
