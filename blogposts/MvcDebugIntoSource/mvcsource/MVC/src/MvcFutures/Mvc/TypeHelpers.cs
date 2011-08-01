namespace Microsoft.Web.Mvc {
    using System;
    using System.Reflection;

    internal static class TypeHelpers {

        public static bool TypeAllowsNullValue(Type type) {
            // reference types allow null values
            if (!type.IsValueType) {
                return true;
            }

            // nullable value types allow null values
            // code lifted from System.Nullable.GetUnderlyingType()
            if (type.IsGenericType && !type.IsGenericTypeDefinition && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) {
                return true;
            }

            // no other types allow null values
            return false;
        }

        public static bool TypeIsParameterlessDelegate(Type type) {
            if (type.IsSubclassOf(typeof(Delegate))) {
                MethodInfo invokeMethod = type.GetMethod("Invoke", Type.EmptyTypes);
                if (invokeMethod != null) {
                    return true;
                }
            }
            return false;
        }

    }
}
