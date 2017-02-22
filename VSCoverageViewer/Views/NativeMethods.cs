using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace VSCoverageViewer.Views
{
    [Flags]
    internal enum WindowStyles : uint
    {
        None = 0,

        /// <summary>
        /// The style bits indicating if a window has an icon with context menu on the left side of the title bar.
        /// </summary>
        SysMenu = 0x80000,
    }

    /// <summary>
    /// Contains native, p/invoke functions.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Get window long special index for the window style flags.
        /// </summary>
        internal const int GWL_STYLE = -16;


        /// <summary>
        /// Clears a set of flags from a window's styles.
        /// </summary>
        /// <param name="wnd">The window to operate on.</param>
        /// <param name="styles">The flags to clear.</param>
        public static void ClearWindowStyles(Window wnd, WindowStyles styles)
        {
            var hwndSource = PresentationSource.FromVisual(wnd) as HwndSource;

            if (hwndSource != null)
            {
                IntPtr hwnd = hwndSource.Handle;
                var wndStyleFlags = (WindowStyles)GetWindowLong(hwnd, GWL_STYLE);

                wndStyleFlags &= ~styles;

                if (SetWindowLong(hwnd, GWL_STYLE, (uint)wndStyleFlags) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }


        // See: MSDN - GetWindowLong function
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms633584.aspx
        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        // See MSDN - SetWindowLong function
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591.aspx
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    }
}
