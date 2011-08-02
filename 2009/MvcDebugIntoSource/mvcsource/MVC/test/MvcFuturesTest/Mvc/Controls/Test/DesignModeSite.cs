namespace Microsoft.Web.Mvc.Controls.Test {
    using System;
    using System.ComponentModel;

    public class DesignModeSite : ISite {
        #region ISite Members
        IComponent ISite.Component {
            get {
                throw new NotImplementedException();
            }
        }

        IContainer ISite.Container {
            get {
                throw new NotImplementedException();
            }
        }

        bool ISite.DesignMode {
            get {
                return true;
            }
        }

        string ISite.Name {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region IServiceProvider Members
        object IServiceProvider.GetService(Type serviceType) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
