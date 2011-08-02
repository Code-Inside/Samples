namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class ViewContext : ControllerContext {

        // parameterless constructor used for mocking
        public ViewContext() {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
            Justification = "The virtual property setters are only to support mocking frameworks, in which case this constructor shouldn't be called anyway.")]
        public ViewContext(ControllerContext controllerContext, IView view, ViewDataDictionary viewData, TempDataDictionary tempData)
            : base(controllerContext) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if (view == null) {
                throw new ArgumentNullException("view");
            }
            if (viewData == null) {
                throw new ArgumentNullException("viewData");
            }
            if (tempData == null) {
                throw new ArgumentNullException("tempData");
            }

            View = view;
            ViewData = viewData;
            TempData = tempData;
        }

        public virtual IView View {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "The property setter is only here to support mocking this type and should not be called at runtime.")]
        public virtual ViewDataDictionary ViewData {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "The property setter is only here to support mocking this type and should not be called at runtime.")]
        public virtual TempDataDictionary TempData {
            get;
            set;
        }

    }
}
