namespace System.Web.Mvc {
    using System.Web.Routing;

    public class AjaxHelper<TModel> : AjaxHelper where TModel : class {
        private ViewDataDictionary<TModel> _viewData;

        public AjaxHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : this(viewContext, viewDataContainer, RouteTable.Routes) {
        }

        public AjaxHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
            : base(viewContext, viewDataContainer, routeCollection) {

            _viewData = new ViewDataDictionary<TModel>(viewDataContainer.ViewData);
        }

        public new ViewDataDictionary<TModel> ViewData {
            get {
                return _viewData;
            }
        }
    }
}
