namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public abstract class ParameterDescriptor : ICustomAttributeProvider {

        private static readonly EmptyParameterBindingInfo _emptyBindingInfo = new EmptyParameterBindingInfo();

        public abstract ActionDescriptor ActionDescriptor {
            get;
        }

        public virtual ParameterBindingInfo BindingInfo {
            get {
                return _emptyBindingInfo;
            }
        }

        public abstract string ParameterName {
            get;
        }

        public abstract Type ParameterType {
            get;
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public virtual object[] GetCustomAttributes(bool inherit) {
            return GetCustomAttributes(typeof(object), inherit);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }

            return (object[])Array.CreateInstance(attributeType, 0);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public virtual bool IsDefined(Type attributeType, bool inherit) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }

            return false;
        }

        private sealed class EmptyParameterBindingInfo : ParameterBindingInfo {
        }

    }
}
