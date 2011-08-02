namespace System.Web.TestUtil {
    using System;
    using System.Globalization;

    public static class UnitTestHelper {
        public static bool EnglishBuildAndOS {
            get {
                bool englishBuild = String.Equals(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                bool englishOS = String.Equals(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                return englishBuild && englishOS;
            }
        }
    }
}
