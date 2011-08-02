namespace Sys.Mvc {
    using System.DHTML;
    using Sys.Net;

    public class AjaxContext {
        private InsertionMode _insertionMode;
        private DOMElement _loadingElement;
        private WebRequestExecutor _response;
        private WebRequest _request;
        private DOMElement _updateTarget;

        public AjaxContext(WebRequest request, DOMElement updateTarget, DOMElement loadingElement, InsertionMode insertionMode) {
            _request = request;
            _updateTarget = updateTarget;
            _loadingElement = loadingElement;
            _insertionMode = insertionMode;
        }

        public string Data {
            get {
                if (_response != null) {
                    return _response.ResponseData;
                }
                else {
                    return null;
                }
            }
        }

        public InsertionMode InsertionMode {
            get {
                return _insertionMode;
            }
        }

        public DOMElement LoadingElement {
            get {
                return _loadingElement;
            }
        }

        public WebRequestExecutor Response {
            get {
                return _response;
            }
            set {
                _response = value;
            }
        }

        public WebRequest Request {
            get {
                return _request;
            }
        }

        public DOMElement UpdateTarget {
            get {
                return _updateTarget;
            }
        }
    }
}
