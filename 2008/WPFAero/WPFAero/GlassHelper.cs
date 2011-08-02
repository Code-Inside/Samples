using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;

namespace WPFAero
{
    public class GlassHelper
    {
        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            if (!DwmIsCompositionEnabled())
                return false;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("The Window must be shown before extending glass.");

            // Set the background to transparent from both the WPF and Win32 perspectives
            window.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            MARGINS margins = new MARGINS(margin);
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
            return true;
        }


        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();
    }

    
struct MARGINS
{
  public MARGINS(Thickness t)
  {
    Left = (int)t.Left;
    Right = (int)t.Right;
    Top = (int)t.Top;
    Bottom = (int)t.Bottom;
  }
  public int Left;
  public int Right;
  public int Top;
  public int Bottom;
}

}
