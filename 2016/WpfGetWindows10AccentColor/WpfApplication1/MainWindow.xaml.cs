using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // https://gist.github.com/paulcbetts/3c6aedc9f0cd39a77c37
            var accentColor = new SolidColorBrush(AccentColorSet.ActiveSet["SystemAccent"]);
            this.Code.Background = accentColor;
            this.Code.Text = "AccentColorSet Immersive 'SystemAccent' " + accentColor.Color.ToString();

            // Available in .NET 4.5
            this.SystemProperties.Background = SystemParameters.WindowGlassBrush;
            this.SystemProperties.Text = "SystemParameters.WindowGlassBrush " + ((SolidColorBrush)SystemParameters.WindowGlassBrush).Color.ToString();

            // Registry undocumented! http://pinvoke.net/default.aspx/dwmapi/DwmGetColorizationParameters.html
        }
        
    }

    // TODO: Add a listener for WM_SETTINGCHANGE to detect changes of the active color scheme automatically.
    //   Add a listener for WM_SETTINGCHANGE and trigger an event, like ActiveSetChanged.
    class AccentColorSet
    {
        public static AccentColorSet[] AllSets
        {
            get
            {
                if (_allSets == null)
                {
                    UInt32 colorSetCount = UXTheme.GetImmersiveColorSetCount();

                    List<AccentColorSet> colorSets = new List<AccentColorSet>();
                    for (UInt32 i = 0; i < colorSetCount; i++)
                    {
                        colorSets.Add(new AccentColorSet(i, false));
                    }

                    AllSets = colorSets.ToArray();
                }

                return _allSets;
            }
            private set
            {
                _allSets = value;
            }
        }

        public static AccentColorSet ActiveSet
        {
            get
            {
                UInt32 activeSet = UXTheme.GetImmersiveUserColorSetPreference(false, false);
                ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                return _activeSet;
            }
            private set
            {
                if (_activeSet != null) _activeSet.Active = false;

                value.Active = true;
                _activeSet = value;
            }
        }

        public Boolean Active { get; private set; }

        public Color this[String colorName]
        {
            get
            {
                IntPtr name = IntPtr.Zero;
                UInt32 colorType;

                try
                {
                    name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                    colorType = UXTheme.GetImmersiveColorTypeFromName(name);
                    if (colorType == 0xFFFFFFFF) throw new InvalidOperationException();
                }
                finally
                {
                    if (name != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(name);
                        name = IntPtr.Zero;
                    }
                }

                return this[colorType];
            }
        }

        public Color this[UInt32 colorType]
        {
            get
            {
                UInt32 nativeColor = UXTheme.GetImmersiveColorFromColorSetEx(this._colorSet, colorType, false, 0);
                //if (nativeColor == 0)
                //    throw new InvalidOperationException();
                return Color.FromArgb(
                    (Byte)((0xFF000000 & nativeColor) >> 24),
                    (Byte)((0x000000FF & nativeColor) >> 0),
                    (Byte)((0x0000FF00 & nativeColor) >> 8),
                    (Byte)((0x00FF0000 & nativeColor) >> 16)
                    );
            }
        }

        AccentColorSet(UInt32 colorSet, Boolean active)
        {
            this._colorSet = colorSet;
            this.Active = active;
        }

        static AccentColorSet[] _allSets;
        static AccentColorSet _activeSet;

        UInt32 _colorSet;

        // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
        //   Since there is currently no known way to retrieve all possible color names,
        //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
        public List<String> GetAllColorNames()
        {
            List<String> allColorNames = new List<String>();
            for (UInt32 i = 0; i < 0xFFF; i++)
            {
                IntPtr typeNamePtr = UXTheme.GetImmersiveColorNamedTypeByIndex(i);
                if (typeNamePtr != IntPtr.Zero)
                {
                    IntPtr typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                    allColorNames.Add(Marshal.PtrToStringUni(typeName));
                }
            }

            return allColorNames;
        }

        static class UXTheme
        {
            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveUserColorSetPreference(Boolean forceCheckRegistry, Boolean skipCheckOnFail);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorFromColorSetEx(UInt32 immersiveColorSet, UInt32 immersiveColorType,
                Boolean ignoreHighContrast, UInt32 highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(UInt32 index);
        }
    }

}
