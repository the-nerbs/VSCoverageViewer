using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VSCoverageViewer.Models;
using VSCoverageViewer.ViewModels;

namespace VSCoverageViewer.Views
{
    /// <summary>
    /// Interaction logic for ReportConfigurationDlg.xaml
    /// </summary>
    public partial class ReportConfigurationDlg : Window
    {
        internal ReportConfigurationViewModel ViewModel
        {
            get
            {
                Debug.Assert(DataContext is ReportConfigurationViewModel);
                return (ReportConfigurationViewModel)DataContext;
            }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationDlg"/>.
        /// </summary>
        public ReportConfigurationDlg()
            : this(new ReportConfigurationModel())
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationDlg"/>.
        /// </summary>
        /// <param name="configuration">The model to bind to.</param>
        internal ReportConfigurationDlg(ReportConfigurationModel configuration)
        {
            InitializeComponent();

            SourceInitialized += HandleSourceInitialized;

            ViewModel.Owner = this;
            ViewModel.Model = configuration;
        }


        /// <summary>
        /// Performs source initialization.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleSourceInitialized(object sender, EventArgs e)
        {
            // probably doesn't matter, but unsubscribe just to be safe.
            SourceInitialized -= HandleSourceInitialized;

            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                // remove the title bar icon.
                const int GWL_STYLE = -16;
                const uint WS_SYSMENU = 0x80000;

                IntPtr hwnd = hwndSource.Handle;
                uint styleFlags = NativeMethods.GetWindowLong(hwnd, GWL_STYLE);

                styleFlags &= ~WS_SYSMENU;

                if (NativeMethods.SetWindowLong(hwnd, GWL_STYLE, styleFlags) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        /// <summary>
        /// Handles the "Create Report" button being clicked.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void CreateReportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }


        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        }
    }
}
