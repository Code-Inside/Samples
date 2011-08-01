namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public abstract class ActionDescriptor : ICustomAttributeProvider {

        private static readonly ActionSelector[] _emptySelectors = new ActionSelector[0];

        public abstract string ActionName {
            get;
        }

        public abstract ControllerDescriptor ControllerDescriptor {
            get;
        }

        public abstract object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters);

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

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "This method may perform non-trivial work.")]
        public virtual FilterInfo GetFilters() {
            return new FilterInfo();
        }

        public abstract ParameterDescriptor[] GetParameters();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "This method may perform non-trivial work.")]
        public virtual ICollection<ActionSelector> GetSelectors() {
            return _emptySelectors;
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public virtual bool IsDefined(Type attributeType, bool inherit) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }

            return false;
        }

    }
}
