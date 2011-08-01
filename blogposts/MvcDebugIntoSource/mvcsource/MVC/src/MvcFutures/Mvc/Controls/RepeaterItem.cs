namespace Microsoft.Web.Mvc.Controls {
    using System.Web.Mvc;
    using System.Web.UI;

    public class RepeaterItem : Control, IDataItemContainer, IViewDataContainer {
        private object _dataItem;
        private int _itemIndex;

        public RepeaterItem(int itemIndex, object dataItem) {
            _itemIndex = itemIndex;
            _dataItem = dataItem;
        }

        public object DataItem {
            get {
                return _dataItem;
            }
        }

        public int DataItemIndex {
            get {
                return _itemIndex;
            }
        }

        public int DisplayIndex {
            get {
                return _itemIndex;
            }
        }

        public ViewDataDictionary ViewData {
            get;
            set;
        }
    }
}
