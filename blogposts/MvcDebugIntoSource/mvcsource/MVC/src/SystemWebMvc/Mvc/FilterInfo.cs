namespace System.Web.Mvc {
    using System.Collections.Generic;

    public class FilterInfo {

        private List<IActionFilter> _actionFilters = new List<IActionFilter>();
        private List<IAuthorizationFilter> _authorizationFilters = new List<IAuthorizationFilter>();
        private List<IExceptionFilter> _exceptionFilters = new List<IExceptionFilter>();
        private List<IResultFilter> _resultFilters = new List<IResultFilter>();

        public IList<IActionFilter> ActionFilters {
            get {
                return _actionFilters;
            }
        }

        public IList<IAuthorizationFilter> AuthorizationFilters {
            get {
                return _authorizationFilters;
            }
        }

        public IList<IExceptionFilter> ExceptionFilters {
            get {
                return _exceptionFilters;
            }
        }

        public IList<IResultFilter> ResultFilters {
            get {
                return _resultFilters;
            }
        }

    }
}
