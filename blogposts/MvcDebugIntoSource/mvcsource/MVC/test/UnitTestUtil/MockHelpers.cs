namespace System.Web.TestUtil {
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Moq;
    using Moq.Language.Flow;

    public static class MockHelpers {
        // this is only requires until Moq can support property setters
        public static IExpect ExpectSetProperty<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> property, TResult value) where T : class {
            // get the property info
            var oldLambdaExpr = property as LambdaExpression;
            var memberExpr = oldLambdaExpr.Body as MemberExpression;
            var propInfo = memberExpr.Member as PropertyInfo;

            // now gen a call to the setter
            var setter = propInfo.GetSetMethod();
            var paramExpr = Expression.Parameter(typeof(T), null);
            var newCallExpr = Expression.Call(paramExpr, setter, Expression.Constant(value, typeof(TResult)));
            var newLambdaExpr = Expression.Lambda<Action<T>>(newCallExpr, paramExpr);
            return mock.Expect(newLambdaExpr);
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this HttpContextBase httpContext) {
            return httpContext.Response.SwitchResponseMockOutputToStringBuilder();
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this HttpResponseBase response) {
            return Mock.Get(response).SwitchResponseMockOutputToStringBuilder();
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this IMock<HttpResponseBase> responseMock) {
            var sb = new StringBuilder();
            responseMock.Expect(response => response.Write(It.IsAny<string>())).Callback<string>(output => sb.Append(output));
            return sb;
        }

    }

    // helper class for making sure that we're performing culture-invariant string conversions
    public class CultureReflector : IFormattable {
        string IFormattable.ToString(string format, IFormatProvider formatProvider) {
            CultureInfo cInfo = (CultureInfo)formatProvider;
            return cInfo.ThreeLetterISOLanguageName;
        }
    }
}
