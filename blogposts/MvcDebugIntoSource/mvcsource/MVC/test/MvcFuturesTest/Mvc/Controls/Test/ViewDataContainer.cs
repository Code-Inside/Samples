namespace Microsoft.Web.Mvc.Controls.Test {
    using System.Web.Mvc;
    using System.Web.UI;

    public class ViewDataContainer : Control, IViewDataContainer {
        private ViewDataDictionary _viewData;

        #region IViewDataContainer Members
        public ViewDataDictionary ViewData {
            get {
                return _viewData;
            }
            set {
                _viewData = value;
            }
        }
        #endregion
    }
}
