namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public class FileCollectionModelBinder : IModelBinder {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            if (bindingContext == null) {
                throw new ArgumentNullException("bindingContext");
            }

            List<HttpPostedFileBase> files = GetFiles(controllerContext.HttpContext.Request.Files, bindingContext.ModelName);
            if (files.Count == 0) {
                return null;
            }

            switch (GetCollectionType(bindingContext.ModelType)) {
                case CollectionType.Array:
                    return files.ToArray();

                case CollectionType.Collection:
                    return new Collection<HttpPostedFileBase>(files);

                case CollectionType.List:
                    return files;

                default:
                    throw new InvalidOperationException(String.Format(
                        CultureInfo.CurrentUICulture, MvcResources.Common_ModelBinderDoesNotSupportModelType, bindingContext.ModelType.FullName));
            }
        }

        private static HttpPostedFileBase ChooseFileContentOrNull(HttpPostedFileBase theFile) {
            bool fileContainsContent = (theFile.ContentLength != 0 || !String.IsNullOrEmpty(theFile.FileName));
            return (fileContainsContent) ? theFile : null;
        }

        private static CollectionType GetCollectionType(Type targetType) {
            if (targetType == typeof(IEnumerable<HttpPostedFileBase>) || targetType == typeof(HttpPostedFileBase[])) {
                return CollectionType.Array;
            }
            if (targetType == typeof(ICollection<HttpPostedFileBase>) || targetType == typeof(Collection<HttpPostedFileBase>)) {
                return CollectionType.Collection;
            }
            if (targetType == typeof(IList<HttpPostedFileBase>) || targetType == typeof(List<HttpPostedFileBase>)) {
                return CollectionType.List;
            }
            return CollectionType.Unknown;
        }

        private static List<HttpPostedFileBase> GetFiles(HttpFileCollectionBase fileCollection, string key) {
            // first, check for any files matching the key exactly
            List<HttpPostedFileBase> files = fileCollection.AllKeys
                .Select((thisKey, index) => (String.Equals(thisKey, key, StringComparison.OrdinalIgnoreCase)) ? index : -1)
                .Where(index => index >= 0)
                .Select(index => fileCollection[index])
                .ToList();

            if (files.Count == 0) {
                // then check for files matching key[0], key[1], etc.
                for (int i = 0; ; i++) {
                    string subKey = String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", key, i);
                    HttpPostedFileBase thisFile = fileCollection[subKey];
                    if (thisFile == null) {
                        break;
                    }
                    files.Add(thisFile);
                }
            }

            // if an <input type="file" /> element was rendered on the page but the user did not select a file before posting
            // the form, null out that entry in the list.
            List<HttpPostedFileBase> filteredFiles = files.ConvertAll((Converter<HttpPostedFileBase, HttpPostedFileBase>)ChooseFileContentOrNull);
            return filteredFiles;
        }

        public static void RegisterBinder(ModelBinderDictionary binders) {
            FileCollectionModelBinder binder = new FileCollectionModelBinder();

            binders[typeof(HttpPostedFileBase[])] = binder;
            binders[typeof(IEnumerable<HttpPostedFileBase>)] = binder;
            binders[typeof(ICollection<HttpPostedFileBase>)] = binder;
            binders[typeof(IList<HttpPostedFileBase>)] = binder;
            binders[typeof(Collection<HttpPostedFileBase>)] = binder;
            binders[typeof(List<HttpPostedFileBase>)] = binder;
        }

        private enum CollectionType {
            Array,
            Collection,
            List,
            Unknown
        }

    }
}
