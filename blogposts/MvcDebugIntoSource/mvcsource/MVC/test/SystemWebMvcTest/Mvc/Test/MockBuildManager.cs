namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Reflection;

    // Custom mock IBuildManager since the mock framework doesn't support mocking internal types
    public class MockBuildManager : IBuildManager {
        private Assembly[] _referencedAssemblies;

        private string _expectedVirtualPath;
        private Type _expectedBaseType;
        private object _instanceResult;
        private Exception _createInstanceFromVirtualPathException;

        public MockBuildManager(Assembly[] referencedAssemblies) {
            _referencedAssemblies = referencedAssemblies;
        }

        public MockBuildManager(Exception createInstanceFromVirtualPathException) {
            _createInstanceFromVirtualPathException = createInstanceFromVirtualPathException;
        }

        public MockBuildManager(string expectedVirtualPath, Type expectedBaseType, object instanceResult) {
            _expectedVirtualPath = expectedVirtualPath;
            _expectedBaseType = expectedBaseType;
            _instanceResult = instanceResult;
        }

        #region IBuildManager Members
        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType) {
            if (_createInstanceFromVirtualPathException != null) {
                throw _createInstanceFromVirtualPathException;
            }

            if ((_expectedVirtualPath == virtualPath) && (_expectedBaseType == requiredBaseType)) {
                return _instanceResult;
            }
            throw new InvalidOperationException("Unexpected call to IBuildManager.CreateInstanceFromVirtualPath()");
        }

        ICollection IBuildManager.GetReferencedAssemblies() {
            return _referencedAssemblies;
        }
        #endregion
    }
}
