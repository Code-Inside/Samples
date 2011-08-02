namespace Sys.Mvc {
    using System;
    using Sys.Net;
    using System.DHTML;

    [Record]
    public sealed class AjaxOptions {
        public string Confirm;
        public string HttpMethod;
        public InsertionMode InsertionMode;
        public string LoadingElementId;
        public CancellableAjaxEventHandler OnBegin;
        public CancellableAjaxEventHandler OnComplete;
        public AjaxEventHandler OnFailure;
        public AjaxEventHandler OnSuccess;
        public string UpdateTargetId;
        public string Url;
    }
}
