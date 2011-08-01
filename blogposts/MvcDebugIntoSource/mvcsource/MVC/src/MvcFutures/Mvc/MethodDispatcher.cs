namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    // The methods in this class don't perform error checking; that is the responsibility of the
    // caller.
    internal sealed class MethodDispatcher {

        private delegate object MethodExecutor(object taget, object[] parameters);
        private delegate void VoidMethodExecutor(object target, object[] parameters);

        private MethodExecutor _executor;

        public MethodDispatcher(MethodInfo methodInfo) {
            _executor = GetExecutor(methodInfo);
            MethodInfo = methodInfo;
        }

        public MethodInfo MethodInfo {
            get;
            private set;
        }

        public object Execute(object target, object[] parameters) {
            return _executor(target, parameters);
        }

        private static MethodExecutor GetExecutor(MethodInfo methodInfo) {
            // Parameters to executor
            ParameterExpression targetParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            List<Expression> parameters = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++) {
                ParameterInfo paramInfo = paramInfos[i];
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            UnaryExpression targetCast = (!methodInfo.IsStatic) ? Expression.Convert(targetParameter, methodInfo.ReflectedType) : null;
            MethodCallExpression methodCall = methodCall = Expression.Call(targetCast, methodInfo, parameters);

            // methodCall is "((TTarget) target) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void)) {
                Expression<VoidMethodExecutor> lambda = Expression.Lambda<VoidMethodExecutor>(methodCall, targetParameter, parametersParameter);
                VoidMethodExecutor voidExecutor = lambda.Compile();
                return WrapVoidAction(voidExecutor);
            }
            else {
                // must coerce methodCall to match ActionExecutor signature
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<MethodExecutor> lambda = Expression.Lambda<MethodExecutor>(castMethodCall, targetParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        private static MethodExecutor WrapVoidAction(VoidMethodExecutor executor) {
            return delegate(object target, object[] parameters) {
                executor(target, parameters);
                return null;
            };
        }

    }
}
