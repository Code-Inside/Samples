namespace System.Web.TestUtil {
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Diagnostics;
    using System.Reflection;

    public static class ConfigHelper {
        private static readonly Dictionary<string, ConfigurationSection> _configSections =
            new Dictionary<string, ConfigurationSection>();
        private static readonly MockConfigSystem _mockConfigSystem = new MockConfigSystem();
        private static readonly FieldInfo _configSystemField =
            typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static | BindingFlags.NonPublic);
        private static IInternalConfigSystem _originalConfigSystem;

        public static void OverrideSection(string sectionName, ConfigurationSection section) {
            _configSections[sectionName] = section;

            if (_originalConfigSystem == null) {
                // Ensure ConfigurationManager is initialized by reading something from config
                object o = ConfigurationManager.ConnectionStrings;

                // Replace config implementation with mock
                _originalConfigSystem = (IInternalConfigSystem)_configSystemField.GetValue(null);
                Debug.Assert(_originalConfigSystem != null);
                _configSystemField.SetValue(null, _mockConfigSystem);
            }
        }

        public static void Revert() {
            if (_originalConfigSystem != null) {
                _configSections.Clear();
                // Revert ConfigurationManager to original state
                _configSystemField.SetValue(null, _originalConfigSystem);
                _originalConfigSystem = null;
            }
        }

        private class MockConfigSystem : IInternalConfigSystem {
            #region IInternalConfigSystem Members
            object IInternalConfigSystem.GetSection(string configKey) {
                if (_configSections.ContainsKey(configKey)) {
                    return _configSections[configKey];
                }
                else {
                    return _originalConfigSystem.GetSection(configKey);
                }
            }

            void IInternalConfigSystem.RefreshConfig(string sectionName) {
                throw new NotImplementedException();
            }

            bool IInternalConfigSystem.SupportsUserConfig {
                get {
                    throw new NotImplementedException();
                }
            }

            #endregion
        }
    }
}
